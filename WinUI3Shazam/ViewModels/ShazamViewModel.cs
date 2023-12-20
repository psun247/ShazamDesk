using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ShazamCore.Models;
using ShazamCore.Services;
using ShazamCore.Helpers;
using WinUI3Shazam.Helpers;
using WinUI3Shazam.Contracts.Services;

namespace WinUI3Shazam.ViewModels;

public partial class ShazamViewModel : BaseViewModel
{
    private const string _ListenToButtonText = "Listen to";
    private const string _DefaultListenToMessage = "Select a microphone or speaker to 'Listen to' while a song is being played (in this app or another)";
    private const int IDENTIFY_TIMEOUT = 25000;
    private static readonly HttpClient _HttpClient = new() { Timeout = TimeSpan.FromSeconds(6) }; // 3 would be too short for Listen()

    private IAzureService _azureService;
    private DeviceService _deviceService;    
    private VideoInfo? _lastVideoInfo;    
    private CancellationTokenSource? _cancelTokenSource;
    private bool _userCanceledListen;
    
    public ShazamViewModel(ILocalSettingsService localsettingsService, IAzureService azureService)
                        : base(localsettingsService)
    {
        _azureService = azureService;
        _deviceService = new DeviceService(_HttpClient);        

        SetCommandBusy(false);
        ListenButtonText = _ListenToButtonText;               
    }
        
    // Set from ShazamPage
    public WebView2 ShazamWebView2Control { get; set; } = new WebView2();
    // Video address in the textbox (whenever navigated to)
    [ObservableProperty]
    string _currentVideoUrl = Constants.YouTubeHomeUrl;
    [ObservableProperty]
    List<DeviceSetting> _deviceSettingList = new List<DeviceSetting>();
    // "Listen To" or "Cancel"
    [ObservableProperty]
    string _listenButtonText = string.Empty;
    [ObservableProperty]
    bool _isProgressOn;
    [ObservableProperty]
    DeviceSetting? _selectedDeviceSetting;
    [ObservableProperty]
    bool _isAddAzureEnabled;
    
    public void Initialize()
    {
        ReloadDeviceList(isAppStartup: true);
    }

    public void OnShazamTabActivated()
    {
        AppSettings.SelectedTabName = Models.AppSettings.ShazamTabName;
        StatusMessage = _DefaultListenToMessage;

        UpdateShazamTabButtons();
    }

    private void ReloadDeviceList(bool isAppStartup)
    {
        try
        {
            List<DeviceInfo> deviceInfoList = _deviceService.GetDeviceList();
            DeviceSettingList = deviceInfoList.Select(x => new DeviceSetting { DeviceName = x.DeviceName, DeviceID = x.DeviceID }).ToList();
            // Note: leave SelectedDeviceSetting as null to force the user to select a right device
            SelectedDeviceSetting = DeviceSettingList.FirstOrDefault(x => x.DeviceID == AppSettings.SelectedDeviceID);
            if (isAppStartup)
            {
                StatusMessage = _DefaultListenToMessage;
            }
            else
            {
                StatusMessage = "Reloaded device list";
            }
        }
        catch (Exception ex)
        {
            ErrorStatusMessage = ex.Message;
        }
    }

    // Note: with 'async Task', Listen button will be automatically disabled when the command is being executed,
    //          hence leaving it as 'async void'        
    [RelayCommand]
    private async void ListenOrCancel()
    {
        if (SelectedDeviceSetting == null || SelectedDeviceSetting.DeviceID.IsBlank())
        {
            ErrorStatusMessage = "Please select a device";
            return;
        }

        if (_isCommandBusy)
        {
            // Cause to throw OperationCanceledException in Listen() on a previously created
            // _cancelTokenSource in this method                
            _userCanceledListen = true;
            _cancelTokenSource?.Cancel();
            return;
        }

        try
        {
            StatusMessage = $"Listening to '{SelectedDeviceSetting.DeviceName}'...please wait";
            ListenButtonText = "Cancel";
            _userCanceledListen = false;

            _cancelTokenSource = new CancellationTokenSource();
#pragma warning disable 4014
            // disable without await, and it's OK because we want to _cancelTokenSource.Cancel() on timeout
            Task.Delay(IDENTIFY_TIMEOUT).ContinueWith((_) =>
            {
                // Cause to throw OperationCanceledException in Listen()
                _cancelTokenSource?.Cancel();
            });

            SetCommandBusy(true);
            ShowProgress(true);

            Tuple<VideoInfo?, string> result = await _deviceService.Listen(SelectedDeviceSetting, _cancelTokenSource);
            _cancelTokenSource = null;
            VideoInfo? videoInfo = result.Item1;
            if (videoInfo != null)
            {
                DebugDumpVideoInfo(videoInfo);

                // Note: Stopping video here (instead of before Listen() call) means
                // I can listen to and identify my own video in the embedded video player!
                // But it would stop the current and show a list of similar videos
                StopCurrentVideo();

                BindWebView2Control(videoInfo.YouTubeWebSiteSearch);

                if (await SongInfoViewModel.UpdateSongInfoSectionAsync(videoInfo))
                {
                    // Hang on this for SQL Server
                    _lastVideoInfo = videoInfo;

                    UpdateShazamTabButtons();
                    StatusMessage = $"Identified as '{videoInfo}'";
                }
            }
            else
            {
                // See OperationCanceledException in Listen() for info
                ErrorStatusMessage = result.Item2.IsBlank() && _userCanceledListen ? "Canceled" : "Timed out";
            }
        }
        catch (Exception ex)
        {
            ErrorStatusMessage = ex.Message;
        }

        ShowProgress(false);
        SetCommandBusy(false);
        ListenButtonText = _ListenToButtonText;
    }

    [RelayCommand]
    private void ReloadDeviceList()
    {
        ReloadDeviceList(isAppStartup: false);
    }

    // Note: using AddAzureAsync() won't bind (maybe a bug with 'Async')
    [RelayCommand]
    private async Task AddAzure()
    {
        if (_lastVideoInfo == null)
        {
            return;
        }

        try
        {            
            // Note: Mouse.OverrideCursor = Cursors.Wait not available in WinUI, so use status message
            StatusMessage = "Adding song to Azure SQL DB via Web API...please wait";

            var songInfo = new SongInfo
            {
                Artist = _lastVideoInfo.Artist,
                Description = _lastVideoInfo.Song,
                CoverUrl = _lastVideoInfo.CoverUrl,
                Lyrics = SongInfoViewModel.SongLyrics,
                SongUrl = CurrentVideoUrl
            };

            string error = await _azureService.AddSongInfoAsync(songInfo, AppSettings.IsWebApiViaAuth);
            if (error.IsBlank())
            {
                _IsAzureTabInSync = false;
                StatusMessage = "Song added to Azure SQL DB via Web API";
            }
            else
            {
                ErrorStatusMessage = error;
            }
        }
        catch (HttpRequestException ex)
        {
            await HandleHttpRequestExceptionAsync(ex, AppSettings.IsWebApiViaAuth, _azureService);
        }
        catch (Exception ex)
        {
            ErrorStatusMessage = ex.Message;
        }        
    }
    
    [RelayCommand]
    private async Task OpenInExternalBrowser()
    {
        string? error = await GeneralHelper.OpenWithBrowserAsync(CurrentVideoUrl);
        if (error != null)
        {
            ErrorStatusMessage = error;
        }
    }
    
    public void GoVideoUrl(string url)
    {
        try
        {
            BindWebView2Control(url);
            CurrentVideoUrl = url;
        }
        catch (Exception ex)
        {
            ErrorStatusMessage = ex.Message;
        }
    }

    private void UpdateShazamTabButtons()
    {
        IsAddAzureEnabled = _lastVideoInfo != null;        
    }

    private void ShowProgress(bool isProgressOn)
    {
        if (SelectedDeviceSetting != null)
        {
            IsProgressOn = isProgressOn;
        }
    }

    private void DebugDumpVideoInfo(VideoInfo videoInfo)
    {
        Debug.WriteLine("****VideoInfo");
        Debug.WriteLine($"Artist: {videoInfo.Artist}");
        Debug.WriteLine($"Song: {videoInfo.Song}");
        Debug.WriteLine($"CoverUrl: {videoInfo.CoverUrl}");
    }

    // partial method hook (after / device selected)
    partial void OnSelectedDeviceSettingChanged(DeviceSetting? value)
    {
        if (value != null)
        {
            AppSettings.SelectedDeviceName = value.DeviceName;
            AppSettings.SelectedDeviceID = value.DeviceID;

            StatusMessage = $"Selected listening device '{value.DeviceName}'";
        }
    }

    partial void OnCurrentVideoUrlChanged(string value)
    {
        AppSettings.SelectedShazamTabSongUrl = value;
    }

    private void SetCommandBusy(bool isCommandBusy)
    {
        _isCommandBusy = isCommandBusy;
        OnPropertyChanged(nameof(IsCommandNotBusy));
    }

    private void BindWebView2Control(string youTubeWebView2Source)
    {
        if (ShazamWebView2Control.Source == null || ShazamWebView2Control.Source.AbsoluteUri != youTubeWebView2Source)
        {
            ShazamWebView2Control.Source = new Uri(youTubeWebView2Source, UriKind.RelativeOrAbsolute);
        }
    }

    // blankUriOnly to avoid UI flickering on app exit
    private void StopCurrentVideo(bool blankUriOnly = false)
    {
        if (ShazamWebView2Control != null)
        {
            // Only stop the playing (if going on), but can't pause the video though (would be nice) because it's a website uri
            Uri uri = ShazamWebView2Control.Source;
            ShazamWebView2Control.Source = Constants.YouTubeHomeUri;
            if (!blankUriOnly)
            {
                ShazamWebView2Control.Source = uri;
            }
        }
    }    
}