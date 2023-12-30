using System;
using System.Windows.Controls;
using ShazamCore.Helpers;

namespace WpfShazam.WinUI3;

public partial class WinUI3UserControl : UserControl
{
    private bool _isFirstLoaded;

    public WinUI3UserControl()
    {
        InitializeComponent();

        Loaded += WinUI3UserControl_Loaded;
        WinUI3Hyperlink.RequestNavigate += WinUI3Hyperlink_RequestNavigate;
    }

    private void WinUI3UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is WinUI3ViewModel winui3ViewModel)
        {
            if (!_isFirstLoaded)
            {
                _isFirstLoaded = true;                
                return;
            }

            winui3ViewModel.OnWinUI3TabActivated();
        }
    }

    private async void WinUI3Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        try
        {
            await GeneralHelper.OpenWithBrowserAsync("https://github.com/psun247/ShazamDesk");
        }
        catch (Exception)
        {
        }
    }
}