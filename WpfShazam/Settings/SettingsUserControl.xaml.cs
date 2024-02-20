using System;
using System.Windows.Controls;
using ShazamCore.Helpers;

namespace WpfShazam.Settings;

public partial class SettingsUserControl : UserControl
{    
    public SettingsUserControl()
    {
        InitializeComponent();

        Loaded += SettingsUserControl_Loaded;        
        SelectedShazamTabSongUrlHyperlink.RequestNavigate += RequestNavigate;
        SelectedAzureTabSongUrlHyperlink.RequestNavigate += RequestNavigate;
        SelectedSqlServerTabSongUrlHyperlink.RequestNavigate += RequestNavigate;
        WebApiUrlHyperlink.RequestNavigate += RequestNavigate;
    }

    private void SettingsUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel settingsViewModel)
        {
            settingsViewModel.OnSettingsTabActivated();
        }
    }

    // Note: shared by multiple hyerlinks
    private async void RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        try
        {
            await GeneralHelper.OpenWithBrowserAsync(e.Uri.AbsoluteUri);
        }
        catch (Exception)
        {
        }
    }
}
