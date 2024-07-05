using System.Diagnostics;
using System.Reflection;
using MauiHybridApp.Services;
using Microsoft.JSInterop;

namespace MauiHybridApp;

public partial class MainPage
{
    [JSInvokable]
    public static async Task<bool> DisplayAlertAsync(
        string title,
        string message,
        string cancel,
        string accept = null!
    )
    {
        bool answer = await ApplicationEx.DisplayAlertOnUIThreadAsync(
            title,
            message,
            accept,
            cancel
        );
        return answer;
    }

    [JSInvokable]
    public static void ToastMake(string message)
    {
        ApplicationEx.ToastMakeOnUIThread(message);
    }

    /// <summary>
    /// Check Permission Status
    /// </summary>
    /// <param name="permission">权限名称列表</param>
    /// <remarks>
    /// 具体权限名称请参考
    /// <see href="https://learn.microsoft.com/zh-cn/dotnet/maui/platform-integration/appmodel/permissions"/>
    /// </remarks>
    /// <returns>权限状态</returns>
    [JSInvokable]
    public static async Task<string> CheckPermissionStatusAsync(string permission)
    {
        try
        {
            // 根据传入的 permission，动态获取权限

            if (string.IsNullOrWhiteSpace(permission))
                return string.Empty;

            // 使用反射动态获取权限
            Type? permissionType = typeof(Permissions); // 假设Permissions是包含权限类型的类
            if (permissionType is null)
                return string.Empty;

            // 根据传入的 permission 字符串获取对应的权限属性
            MemberInfo? member = permissionType.GetMember(permission).FirstOrDefault();
            if (member is null)
                return "找不到对应的权限属性";

            // 动态获取权限属性的值
            object? permissionValue = Activator.CreateInstance((Type)member);
            if (permissionValue is null)
                return "找不到对应的权限属性";

            // 使用反射动态调用CheckStatusAsync方法
            MethodInfo? method = permissionType
                .GetMethod("CheckStatusAsync")
                ?.MakeGenericMethod(permissionValue.GetType());
            Debug.WriteLine(method?.ToString());
            if (method is null)
                return string.Empty;

            // 动态调用CheckStatusAsync方法
            PermissionStatus? status = await (Task<PermissionStatus>)method.Invoke(null, [])!;
            if (status is null)
                return string.Empty;

            return status?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    /// <summary>
    /// Request Permission
    /// </summary>
    /// <param name="permission">权限名称列表</param>
    /// <remarks>
    /// 具体权限名称请参考
    /// <see href="https://learn.microsoft.com/zh-cn/dotnet/maui/platform-integration/appmodel/permissions"/>
    /// </remarks>
    /// <returns>权限状态</returns>
    [JSInvokable]
    public static async Task<string> RequestPermissionAsync(string permission)
    {
        try
        {
            // 根据传入的 permission，动态获取权限

            if (string.IsNullOrWhiteSpace(permission))
                return string.Empty;

            // 使用反射动态获取权限
            Type? permissionType = typeof(Permissions); // 假设Permissions是包含权限类型的类
            if (permissionType is null)
                return string.Empty;

            // 根据传入的 permission 字符串获取对应的权限属性
            MemberInfo? member = permissionType.GetMember(permission).FirstOrDefault();
            if (member is null)
                return "找不到对应的权限属性";

            // 动态获取权限属性的值
            object? permissionValue = Activator.CreateInstance((Type)member);
            if (permissionValue is null)
                return "找不到对应的权限属性";

            // 使用反射动态调用RequestAsync方法
            MethodInfo? method = permissionType
                .GetMethod("RequestAsync")
                ?.MakeGenericMethod(permissionValue.GetType());
            Debug.WriteLine(method?.ToString());
            if (method is null)
                return string.Empty;

            // 动态调用RequestAsync方法
            PermissionStatus? status = await (Task<PermissionStatus>)method.Invoke(null, [])!;
            if (status is null)
                return string.Empty;

            return status?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [JSInvokable]
    public static async Task<string> CapturePhoto()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
            return string.Empty;

        FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

        if (photo is null)
            return string.Empty;

        using Stream sourceStream = await photo.OpenReadAsync();
        using MemoryStream memoryStream = new();

        await sourceStream.CopyToAsync(memoryStream);

        byte[] imageBytes = memoryStream.ToArray();
        string base64String = Convert.ToBase64String(imageBytes);

        // Determine image format and set the format identifier accordingly
        string formatIdentifier = GetImageFormatIdentifier(photo.FileName);
        string base64StringWithFormat = formatIdentifier + base64String;

        return base64StringWithFormat;
    }

    [JSInvokable]
    public static async Task<string> TakePhoto()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
            return string.Empty;

        FileResult? photo = await MediaPicker.Default.PickPhotoAsync();

        if (photo is null)
            return string.Empty;

        using Stream sourceStream = await photo.OpenReadAsync();
        using MemoryStream memoryStream = new();

        await sourceStream.CopyToAsync(memoryStream);

        byte[] imageBytes = memoryStream.ToArray();
        string base64String = Convert.ToBase64String(imageBytes);

        // Determine image format and set the format identifier accordingly
        string formatIdentifier = GetImageFormatIdentifier(photo.FileName);
        string base64StringWithFormat = formatIdentifier + base64String;

        return base64StringWithFormat;
    }

    private static string GetImageFormatIdentifier(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

        if (extension == ".jpg" || extension == ".jpeg")
        {
            return "data:image/jpeg;base64,";
        }
        else if (extension == ".png")
        {
            return "data:image/png;base64,";
        }
        else if (extension == ".gif")
        {
            return "data:image/gif;base64,";
        }
        else
        {
            return "data:image/jpeg;base64,"; // Default to JPEG format
        }
    }

    public static event EventHandler<string> CallFromJs = null!;

    [JSInvokable]
    public static async Task<string> JsCallDotNetCallJs()
    {
        CallFromJs?.Invoke(null!, "Message From .Net");
        return await Task.FromResult("JsCallDotNetCallJs Success").ConfigureAwait(false);
    }
}
