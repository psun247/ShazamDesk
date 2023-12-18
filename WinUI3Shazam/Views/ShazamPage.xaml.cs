using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Shazam.ViewModels;

namespace WinUI3Shazam.Views;

public sealed partial class ShazamPage : Page
{
    private bool _loadedCalled;

    public ShazamPage()
    {
        InitializeComponent();

        // Could do this in XAML. If Disabled (default), a new page is created when Shazam tab selected / activated
        NavigationCacheMode = NavigationCacheMode.Enabled;
        ViewModel = App.GetService<ShazamViewModel>();
        ViewModel.ShazamWebView2Control = ShazamWebView2Control;
        SongInfo.ViewModel = ViewModel.SongInfoViewModel;
        StatusBar.ViewModel = ViewModel;

        Loaded += ShazamPage_Loaded;
    }

    public ShazamViewModel ViewModel { get; }
    
    // Call every time it's selected
    private async void ShazamPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (!_loadedCalled)
        {
            // Note: sometimes CoreWebView2 remains null on first Loaded (not-null on 2nd selected / activated), so do this:            
            //          https://stackoverflow.com/questions/63116740/why-my-corewebview2-which-is-object-of-webview2-is-null
            await ShazamWebView2Control.EnsureCoreWebView2Async();

            if (ShazamWebView2Control.CoreWebView2 != null)
            {
                ShazamWebView2Control.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
                _loadedCalled = true;
            }
        }

        ViewModel.OnShazamTabActivated();
    }

    private void CoreWebView2_SourceChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs args)
    {
        ViewModel.CurrentVideoUrl = ShazamWebView2Control.Source.AbsoluteUri;
    }

    // Key="Enter"
    private void KeyboardAccelerator_OnInvoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender,
                                                Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        if (args.Element is TextBox textBox)
        {
            ViewModel.GoVideoUrl(textBox.Text);
            args.Handled = true;
        }
    }
}
