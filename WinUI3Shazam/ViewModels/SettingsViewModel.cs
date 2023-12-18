using CommunityToolkit.Mvvm.ComponentModel;
using ShazamCore.Services;
using WinUI3Shazam.Contracts.Services;

namespace WinUI3Shazam.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private ILocalSettingsService _localsettingsService;
    private IAzureService _azureService;

    public SettingsViewModel(ILocalSettingsService localsettingsService, IAzureService azureService)
    {
        _localsettingsService = localsettingsService;
        _azureService = azureService;

        SettingsChangedNote = "Note: if above settings have been changed, they will be saved on app exit " +
                                    $"({localsettingsService.SettingsFilePath})";
    }

    public Models.AppSettings AppSettings => _localsettingsService.AppSettings;
    [ObservableProperty]
    string _settingsChangedNote = string.Empty;
    [ObservableProperty]
    bool _isWebApiViaAuth;
    [ObservableProperty]
    string? _webApiUrl;

    public void OnSettingsPageActivated()
    {
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
