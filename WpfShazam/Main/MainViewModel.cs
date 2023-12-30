using System;
using WpfShazam.About;
using WpfShazam.ChatGPT;
using WpfShazam.Shazam;
using WpfShazam.Azure;
using WpfShazam.SqlServer;
using WpfShazam.WinUI3;
using WpfShazam.Settings;

namespace WpfShazam.Main;

public class MainViewModel : BaseViewModel
{
    public MainViewModel(ILocalSettingsService localsettingsService)
                    : base(localsettingsService)
    {
        Version ver = Environment.Version;
        AppTitle = $"ChatGPT + Shazam (.NET {ver.Major}.{ver.Minor}.{ver.Build} runtime) by Peter Sun";
#if DEBUG
        AppTitle += " - Debug";
#endif
        ChatGPTViewModel = App.GetService<ChatGPTViewModel>();
        ShazamViewModel = App.GetService<ShazamViewModel>();
        AzureViewModel = App.GetService<AzureViewModel>();
        SqlServerViewModel = App.GetService<SqlServerViewModel>();
        WinUI3ViewModel = App.GetService<WinUI3ViewModel>();
        SettingsViewModel = App.GetService<SettingsViewModel>();            
        AboutViewModel = App.GetService<AboutViewModel>();
    }

    public string AppTitle { get; }
    public ChatGPTViewModel ChatGPTViewModel { get; }
    public ShazamViewModel ShazamViewModel { get; }
    public AzureViewModel AzureViewModel { get; }
    public SqlServerViewModel SqlServerViewModel { get; }
    public WinUI3ViewModel WinUI3ViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }        
    public AboutViewModel AboutViewModel { get; }

    // Could check 'busy' to reject shutdown
    public bool Shutdown()
    {            
        _localsettingsService.SaveAppSettings();

        return true;
    }
}
