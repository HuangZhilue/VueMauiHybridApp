// TODO https://github.com/dotnet/maui/issues/884#issuecomment-1760299780

using Android.App;
using Android.Content;
using Android.Provider;
using Android.Webkit;
using Uri = Android.Net.Uri;
using WebView = Android.Webkit.WebView;

namespace MauiHybridApp;

// Microsoft.Maui.Storage.FileProvider is trimmed out in Release and not sure how to keep it in place on Android, so copied it and added our own custom one, which works in Debug & Release
// TODO: Figure out how to do this properly w/o hardcoding a custom file provider (see: https://stackoverflow.com/questions/75380344/)
[ContentProvider(
    ["${applicationId}.custom.fileProvider"],
    Name = "microsoft.maui.essentials.custom.fileProvider",
    Exported = false,
    GrantUriPermissions = true
)]
[MetaData(
    "android.support.FILE_PROVIDER_PATHS",
    Resource = "@xml/microsoft_maui_essentials_fileprovider_file_paths"
)]
internal class CustomFileProvider : AndroidX.Core.Content.FileProvider { }

public class CustomMauiWebChromeClient : WebChromeClient
{
    public override bool OnShowFileChooser(
        WebView? webView,
        IValueCallback? filePathCallback,
        FileChooserParams? fileChooserParams
    )
    {
        if (filePathCallback is null)
            return base.OnShowFileChooser(webView, filePathCallback, fileChooserParams);

        //OnShowFileChooserAsync(filePathCallback, fileChooserParams).FireAndForget();
        Task.Run(() => OnShowFileChooserAsync(filePathCallback, fileChooserParams!)); // 使用 Task.Run 来在后台线程执行异步操作

        return true;
    }

    private static async Task OnShowFileChooserAsync(
        IValueCallback filePathCallback,
        FileChooserParams fileChooserParams
    )
    {
        // If capture attribute is used, then don't show file chooser, but decide whether to take picture or video using accept-type below
        var initIntent = fileChooserParams.IsCaptureEnabled
            ? new Intent()
            : fileChooserParams.CreateIntent();

        var chooserIntent = Intent.CreateChooser(initIntent, "File Chooser");

        Uri photoContainerUri = default!;

        // Add camera/video intents only IF we have permission to use the camera
        //if (await MauiAppServices.CheckAndRequestCameraPermission())
        // 检查并请求相机权限
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }

        if (status == PermissionStatus.Granted)
        {
            var extraInitialIntents = new List<Intent>();

            var acceptTypes = ParseAcceptTypes(fileChooserParams.GetAcceptTypes()!);

            if (acceptTypes.HasFlag(FileContentType.Image))
            {
                var cameraIntent = new Intent(MediaStore.ActionImageCapture);

                // We setup a container for the captured image to be written to so we can get the full resolution image, otherwise it will be a low resolution thumbnail
                // https://developer.android.com/reference/android/provider/MediaStore#ACTION_IMAGE_CAPTURE
                photoContainerUri = SetupPhotoCaptureContainer();

                cameraIntent.PutExtra(MediaStore.ExtraOutput, photoContainerUri);

                extraInitialIntents.Add(cameraIntent);
            }

            if (acceptTypes.HasFlag(FileContentType.Video))
            {
                var videoIntent = new Intent(MediaStore.ActionVideoCapture);
                extraInitialIntents.Add(videoIntent);
            }

            if (extraInitialIntents.Count > 0)
                chooserIntent?.PutExtra(Intent.ExtraInitialIntents, extraInitialIntents.ToArray());
        }

        var requestCode = default(int);

        requestCode = CustomActivityResultCallbackRegistry.RegisterActivityResultCallback(
            (result, data) =>
            {
                CustomActivityResultCallbackRegistry.UnregisterActivityResultCallback(requestCode);

                OnActivityResult(filePathCallback, result, data, photoContainerUri);
            }
        );

        Platform.CurrentActivity?.StartActivityForResult(chooserIntent, requestCode);
    }

    private static FileContentType ParseAcceptTypes(string[] acceptTypes)
    {
        FileContentType type = default;

        // When the accept attribute isn't provided GetAcceptTypes returns array with single element of empty string, indicating "*": [ "" ]
        if (
            acceptTypes switch
            {
                [""] => true,
                _ => false
            }
        )
        {
            type ^= FileContentType.Any;
            return type;
        }

        // TODO: Do we need to identifiy specific extensions (i.e. jpg, png, etc)?
        if (acceptTypes.Any(e => e.StartsWith("image/", StringComparison.OrdinalIgnoreCase)))
            type ^= FileContentType.Image;

        if (acceptTypes.Any(e => e.StartsWith("video/", StringComparison.OrdinalIgnoreCase)))
            type ^= FileContentType.Video;

        // TODO: Check for doc?

        return type;
    }

    [Flags]
    private enum FileContentType
    {
        Any = Image | Video,
        Image = 1 << 1,
        Video = 1 << 2,
    }

    private static Uri SetupPhotoCaptureContainer()
    {
        var imagePrefix = string.Format("img_{0:yyyyMMdd_HHmmss}", DateTime.Now);
        var imageFileName = imagePrefix + ".jpg";

        // When using IMAGE_CAPTURE and passing in uri as "file://", even though we can determine the file exists, we don't have permission to read it's contents later for some reason. Additionally, we are only able to
        // get the camera app to write to the public external picture directory, but won't work for any app directories. Using FileProvider solves both of these issues, but still using public external picture directory
        // so that the user can see and re-upload these images later if they decide to (local app directories would be hidden to them).
        var dir = global::Android.OS.Environment.GetExternalStoragePublicDirectory(
            global::Android.OS.Environment.DirectoryPictures
        );

        var imageFile = new Java.IO.File(dir, imageFileName);

        return AndroidX.Core.Content.FileProvider.GetUriForFile(
            global::Android.App.Application.Context,
            global::Android.App.Application.Context.PackageName + ".custom.fileProvider",
            imageFile
        )!;
    }

    private static void OnActivityResult(
        IValueCallback filePathCallback,
        Result resultCode,
        Intent data,
        Uri mediaExtraOutputUri
    )
    {
        filePathCallback?.OnReceiveValue(ParseResult(resultCode, data, mediaExtraOutputUri));
    }

    private static Uri[] ParseResult(Result resultCode, Intent data, Uri mediaExtraOutputUri)
    {
        // return FileChooserParams.ParseResult((int)resultCode, data);
        // `FileChooserParams.ParseResult` doesn't support returning multiple files, so we decide to parse the result manually below, which also adds support for `ACTION_IMAGE_CAPTURE` results

        var results = new List<Uri>();

        if (resultCode == Result.Ok)
        {
            if (data?.Extras is not null)
                foreach (string k in data.Extras.KeySet() ?? [])
                {
                    using Java.Lang.Object? itm = data.Extras.Get(k);

                    /* Leaving for historical reasons, but when using `ACTION_IMAGE_CAPTURE` w/o `EXTRA_OUTPUT`, the image data is passed back in the extra data as a Bitmap, but it is a low resolution thumbnail,
                     * and we'd much rather have the fullsize image
                     * Note: this code was not completed, just explorative
                    if (itm is Bitmap b)
                    {
                        var imageFileName = string.Format("img_{0:yyyyMMdd_HHmmss}_", DateTime.Now);
                        var storageDir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);

                        int size = b.RowBytes * b.Height;
                        var byteBuffer = ByteBuffer.Allocate(size);
                        b.CopyPixelsToBuffer(byteBuffer);

                        // 76800
                        var bts = new byte[byteBuffer.Capacity()];

                        byteBuffer.Rewind();
                        byteBuffer.Get(bts);

                        var fp = System.IO.Path.Combine(storageDir.AbsolutePath, imageFileName + $"1.png");
                        File.WriteAllBytes(fp, bts);

                        using (var stream = new MemoryStream())
                        {
                            b.Compress(Bitmap.CompressFormat.WebpLossless, 0, stream);

                            // 12847
                            var bytes = stream.ToArray();
                            
                            fp = System.IO.Path.Combine(storageDir.AbsolutePath, imageFileName + $"2.webp");
                            File.WriteAllBytes(fp, bytes);
                        }
                    }
                    */
                }

            if (data?.Data is not null)
            {
                results.Add(data.Data);
            }
            else if (data?.ClipData?.ItemCount > 0)
            {
                for (var i = 0; i < data.ClipData.ItemCount; i++)
                {
                    ClipData.Item? item = data.ClipData.GetItemAt(i);
                    results.Add(item!.Uri!);
                }
            }
            else if (mediaExtraOutputUri is not null && DoesUriContentExists(mediaExtraOutputUri))
            {
                results.Add(mediaExtraOutputUri);
            }
        }

        return [.. results];
    }

    private static bool DoesUriContentExists(Uri contentUri)
    {
        if (contentUri is null)
            return false;

        // https://stackoverflow.com/a/16336319/10388359
        Android.Database.ICursor? cursor = Platform.CurrentActivity?.ContentResolver?.Query(
            contentUri,
            [MediaStore.IMediaColumns.Size],
            null,
            null,
            null
        );

        if (cursor != null)
            while (cursor.MoveToNext())
            {
                var colIndex = cursor.GetColumnIndex(MediaStore.IMediaColumns.Size);
                var size = cursor.GetInt(colIndex);

                if (size > 0)
                    return true;
            }

        return false;
    }
}
