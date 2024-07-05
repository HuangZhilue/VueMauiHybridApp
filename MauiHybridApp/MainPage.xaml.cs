namespace MauiHybridApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void BlazorWebView_BlazorWebViewInitialized(
        object sender,
        Microsoft.AspNetCore.Components.WebView.BlazorWebViewInitializedEventArgs e
    )
    {
#if ANDROID
        // TODO https://github.com/dotnet/maui/issues/884#issuecomment-1760299780
        e.WebView.SetWebChromeClient(new CustomMauiWebChromeClient());
#endif
    }
}
