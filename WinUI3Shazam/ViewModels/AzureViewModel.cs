using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShazamCore.Models;
using ShazamCore.Services;
using ShazamCore.Helpers;
using WinUI3Shazam.Helpers;
using WinUI3Shazam.Contracts.Services;

namespace WinUI3Shazam.ViewModels;

public partial class AzureViewModel : BaseViewModel
{
    private IAzureService _azureService;

    public AzureViewModel(ILocalSettingsService localsettingsService, IAzureService azureService)
                            : base(localsettingsService)
    {
        _azureService = azureService;
    }

    // Track for open with browser
    [ObservableProperty]
    string _currentVideoUrl = Constants.YouTubeHomeUrl;
    [ObservableProperty]
    ObservableCollection<SongInfo> _songInfoListFromAzure = new ObservableCollection<SongInfo>();
    [ObservableProperty]
    SongInfo? _selectedSongInfoFromAzure;
    // Set from AzurePage
    public WebView2 AzureWebView2Control { get; set; } = new WebView2();
    [ObservableProperty]
    bool _isDeleteAzureEnabled;

    public async void OnAzureTabActivated()
    {
        AppSettings.SelectedTabName = Models.AppSettings.AzureTabName;
        StatusMessage = "To listen to a song to identify, go back to Shazam tab";

        if (!_IsAzureTabInSync)
        {
            await LoadSongInfoListOnAzureTabAsync();

            // Auto-select SongInfoListFromAzure
            var songInfo = SongInfoListFromAzure.FirstOrDefault(x => x.SongUrl == AppSettings.SelectedAzureTabSongUrl);
            if (songInfo != null && songInfo != SelectedSongInfoFromAzure)
            {
                SelectedSongInfoFromAzure = songInfo;
            }

            _IsAzureTabInSync = true;
        }
        UpdateAzureTabButtons();
    }

    private async Task LoadSongInfoListOnAzureTabAsync()
    {
        try
        {
            StatusMessage = "Loading song list from Azure SQL DB via Web API...please wait";

            var list = await _azureService.GetAllSongInfoListAsync(AppSettings.IsWebApiViaAuth);
            SongInfoListFromAzure = new ObservableCollection<SongInfo>(list);

            StatusMessage = list.Count == 0 ? "No song found at Azure SQL DB via Web API" : "Song list loaded from Azure SQL DB via Web API";
        }
        catch (HttpRequestException ex)
        {
            // Note: leaving SongInfoListFromAzure alone on error seems reasonable (say, server unavailable for a while)            
            await HandleHttpRequestExceptionAsync(ex, AppSettings.IsWebApiViaAuth, _azureService);
        }
        catch (Exception ex)
        {
            // Note: leaving SongInfoListFromAzure alone on error seems reasonable (say, server unavailable for a while)            
            ErrorStatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task DeleteAzure()
    {
        bool confirmed = await ConfirmationDialogAsync(AzureWebView2Control,
                    $"Are you sure you want to delete '{SelectedSongInfoFromAzure}' from Azure SQL DB via Web API?",
                    "Yes", "No", "Cancel");
        if (!confirmed)
        {
            return;
        }

        try
        {
            // Note: Mouse.OverrideCursor = Cursors.Wait not available in WinUI, so use status message            
            StatusMessage = "Deleting song from Azure SQL DB via Web API...please wait";

            string error = await _azureService.DeleteSongInfoAsync(SelectedSongInfoFromAzure!.Id, AppSettings.IsWebApiViaAuth);
            if (error.IsBlank())
            {
                List<SongInfo> list = await _azureService.GetAllSongInfoListAsync(AppSettings.IsWebApiViaAuth);
                // Note: the following will cause SelectedSongInfoFromAzure in XAML clear binding (i.e. set SelectedSongInfoFromAzure to null),
                //          hence triggering OnSelectedSongInfoFromAzureChanged()'s 'value == null' logic 
                SongInfoListFromAzure = new ObservableCollection<SongInfo>(list);
                UpdateAzureTabButtons();

                StatusMessage = "Song deleted from Azure SQL DB via Web API";
            }
            else
            {
                ErrorStatusMessage = error;
            }
        }
        catch (HttpRequestException ex)
        {
            await HandleHttpRequestExceptionAsync(ex, AppSettings.IsWebApiViaAuth, _azureService);
            ErrorStatusMessage = ex.Message;
        }
        catch (ArgumentException ex)
        {
            // Note: was because OnSelectedSongInfoFromAzureChanged()'s 'value == null', but shouldn't get here anymore
            ErrorStatusMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorStatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task RefreshAzure()
    {
        await LoadSongInfoListOnAzureTabAsync();
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

    // Note: {x:Bind CoverUrl, Mode=TwoWay,..} with DataTemplate x:Key="ListViewTemplate" in AzurePage would cause  
    //          value.CoverUrl become Microsoft.UI.Xaml.Media.Imaging.BitmapImage, so ensure Mode=OneWay!
    partial void OnSelectedSongInfoFromAzureChanged(SongInfo? value)
    {
        if (value == null)
        {
            SongInfoViewModel.UpdateSongInfoSection(null, SongInfoViewModel.ReadyToListen, string.Empty);
            AzureWebView2Control.Source = Constants.YouTubeHomeUri;
            AppSettings.SelectedAzureTabSongSummary = "YouTube home";
            AppSettings.SelectedAzureTabSongUrl = Constants.YouTubeHomeUrl;
        }
        else
        {
            SongInfoViewModel.UpdateSongInfoSection(value.CoverUrl, value.ToString(), value.Lyrics);
            AzureWebView2Control.Source = new Uri(value.SongUrl);
            AppSettings.SelectedAzureTabSongSummary = value.ToString();
            AppSettings.SelectedAzureTabSongUrl = value.SongUrl;
        }
        UpdateAzureTabButtons();
    }

    private void UpdateAzureTabButtons()
    {
        IsDeleteAzureEnabled = SongInfoListFromAzure.Count > 0 && SelectedSongInfoFromAzure != null;
    }
}
