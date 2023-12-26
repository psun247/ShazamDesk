using CommunityToolkit.Mvvm.ComponentModel;
using ShazamCore.Services;

namespace WpfShazam.Settings;

public partial class SettingsViewModel : ObservableRecipient
{
    private ILocalSettingsService _localsettingsService;
    private IAzureService _azureService;

    public SettingsViewModel(ILocalSettingsService localsettingsService, IAzureService azureService)
    {
        _localsettingsService = localsettingsService;
        _azureService = azureService;

        SettingsChangedNote = $"Changed settings will be saved on app exit ({localsettingsService.SettingsFilePath})";
    }

    public AppSettings AppSettings => _localsettingsService.AppSettings;
    [ObservableProperty]
    string _settingsChangedNote = string.Empty;
    [ObservableProperty]
    bool _isWebApiViaAuth;
    [ObservableProperty]
    string? _webApiUrl;

    public void OnSettingsTabActivated()
    {
        OnPropertyChanged(nameof(AppSettings));

        IsWebApiViaAuth = _localsettingsService.AppSettings.IsWebApiViaAuth;
        UpdateWebApiUrl(IsWebApiViaAuth);
    }

    partial void OnIsWebApiViaAuthChanged(bool value)
    {
        IsWebApiViaAuth = _localsettingsService.AppSettings.IsWebApiViaAuth = value;
        UpdateWebApiUrl(IsWebApiViaAuth);
    }

    private void UpdateWebApiUrl(bool isWebApiViaAuth)
    {
        if (isWebApiViaAuth)
        {
            WebApiUrl = _azureService.WebApiUrlAuth;
        }
        else
        {
            WebApiUrl = _azureService.WebApiUrlNoAuth;
        }
    }
}
