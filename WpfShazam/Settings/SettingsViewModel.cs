﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    public string SettingsFilePath => _localsettingsService.SettingsFilePath;
    [ObservableProperty]
    string _settingsChangedNote = string.Empty;
    [ObservableProperty]
    bool _isGrpcService;    
    [ObservableProperty]
    bool _isWebApiViaAuth;
    [ObservableProperty]
    string? _webApiUrl; 
    [ObservableProperty]
    bool _isGrpc;
    [ObservableProperty]
    string? _grpcUrl;

    public void OnSettingsTabActivated()
    {
        OnPropertyChanged(nameof(AppSettings));

        IsGrpcService = _localsettingsService.AppSettings.IsGrpcService;
        IsWebApiViaAuth = _localsettingsService.AppSettings.IsWebApiViaAuth;
        UpdateWebApiUrl(IsWebApiViaAuth);
    }

    partial void OnIsGrpcServiceChanged(bool value)
    {
        IsGrpcService = _localsettingsService.AppSettings.IsGrpcService = value;
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
