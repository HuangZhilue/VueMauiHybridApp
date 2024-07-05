using Android.App;
using Android.Content;
using Android.Content.PM;

namespace MauiHybridApp;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize
        | ConfigChanges.Orientation
        | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout
        | ConfigChanges.SmallestScreenSize
        | ConfigChanges.Density
)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        // TODO https://github.com/dotnet/maui/issues/884#issuecomment-1760299780
        CustomActivityResultCallbackRegistry.InvokeCallback(requestCode, resultCode, data!);
    }
}
