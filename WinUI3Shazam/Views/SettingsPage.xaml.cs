using Microsoft.UI.Xaml.Controls;

using WinUI3Shazam.ViewModels;

namespace WinUI3Shazam.Views;

public sealed partial class SettingsPage : Page
{
    // Note: NavigationCacheMode="Disabled" (default), so this is created every time it's displayed
    public SettingsPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<SettingsViewModel>();
        ViewModel.OnSettingsPageActivated();        
    }
    
    public SettingsViewModel ViewModel { get; }
}
