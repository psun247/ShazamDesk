using Microsoft.UI.Xaml.Controls;
using WinUI3Shazam.Services;
using WinUI3Shazam.ViewModels;

namespace WinUI3Shazam.Views;

public sealed partial class StatusBarUserControl : UserControl
{    
    public StatusBarUserControl()
    {
        InitializeComponent();
    }

    // Set by parent
    public BaseViewModel ViewModel { get; set; } = new BaseViewModel(new LocalSettingsService());
}
