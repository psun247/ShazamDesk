// Copyright(c) 2023-2024 Peter Sun
using System;
using System.Windows;
using WpfShazam.Settings;

namespace WpfShazam.Main;

public partial class MainWindow : Window
{
    private MainViewModel _mainViewModel;

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();

        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
        DataContext = _mainViewModel = mainViewModel;
    }

    // Note: don't need to handle tab click (TabControlName.SelectionChanged)
    //          because each has Loaded event (e.g. ShazamUserControl_Loaded)
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var localsettingsService = App.GetService<ILocalSettingsService>();
            switch (localsettingsService.AppSettings.SelectedTabName)
            {
                // Because of TabControl's SelectedIndex="-1", all '.IsSelected = true' now works properly.                
                case AppSettings.ShazamTabName:                    
                    ShazamTabItem.IsSelected = true;
                    break;
                case AppSettings.AzureTabName:
                    AzureTabItem.IsSelected = true;
                    break;
                case AppSettings.SqlServerTabName:
                    SqlServerTabItem.IsSelected = true;
                    break;
                case AppSettings.WinUI3TabName:
                    WinUI3TabItem.IsSelected = true;
                    break;
                case AppSettings.AboutTabName:
                    AboutTabItem.IsSelected = true;
                    break;
            };
        }
        catch (Exception)
        {
        }
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            if (!_mainViewModel.Shutdown())
            {
                // Busy, try to close later
                e.Cancel = true;
            }
        }
        catch (Exception)
        {
        }
    }
}
