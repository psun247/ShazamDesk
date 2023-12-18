using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Shazam.ViewModels;

namespace WinUI3Shazam.Views;

public sealed partial class AzurePage : Page
{
    private bool _loadedCalled;

    public AzurePage()
    {
        InitializeComponent();

        // Could do this in XAML. If Disabled (default), a new page is created when Azure tab selected / activated
        NavigationCacheMode = NavigationCacheMode.Enabled;
        ViewModel = App.GetService<AzureViewModel>();
        ViewModel.AzureWebView2Control = AzureWebView2Control;
        SongInfo.ViewModel = ViewModel.SongInfoViewModel;
        StatusBar.ViewModel = ViewModel;

        Loaded += AzurePage_Loaded;
    }

    public AzureViewModel ViewModel { get; }

    // Call every time it's selected
    private async void AzurePage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {        
        if (!_loadedCalled)
        {
            // Note: sometimes CoreWebView2 remains null on first Loaded (not-null on 2nd selected / activated), so do this:            
            //          https://stackoverflow.com/questions/63116740/why-my-corewebview2-which-is-object-of-webview2-is-null
            await AzureWebView2Control.EnsureCoreWebView2Async();

            if (AzureWebView2Control.CoreWebView2 != null)
            {
                AzureWebView2Control.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
                _loadedCalled = true;     
            }
        }

        ViewModel.OnAzureTabActivated();
    }

    private void CoreWebView2_SourceChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs args)
    {        
        ViewModel.CurrentVideoUrl = AzureWebView2Control.Source.AbsoluteUri;
    }
}
