using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientServerShared;
using ShazamCore.Models;
using ShazamCore.Helpers;
using ShazamCore.Services;
using WpfShazam.Main;
using WpfShazam.Settings;

namespace WpfShazam.Azure;

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
    public WebView2 AzureWebView2Control { get; private set; } = new WebView2();
    [ObservableProperty]
    bool _isDeleteAzureEnabled;

    public void Initialize()
    {        
        AzureWebView2Control = new WebView2
        {
            Name = "AzureWebView2",
            Source = Constants.YouTubeHomeUri,
        };
        AzureWebView2Control.SourceChanged += (s, e) =>
        {            
            CurrentVideoUrl = AzureWebView2Control.Source.AbsoluteUri;
        };
        OnPropertyChanged(nameof(AzureWebView2Control));
        
        SongInfoViewModel.SongInfoPanelVisibility = AppSettings.AzureTab.IsSongInfoPanelVisible ?
                                                       Visibility.Visible : Visibility.Collapsed;
    }

    public async Task OnAzureTabActivated()
    {
        AppSettings.SelectedTabName = AppSettings.AzureTabName;        

        if (!_IsAzureTabInSync)
        {
            await LoadSongInfoListOnAzureTabAsync();

            // Auto-select SongInfoListFromAzure
            var songInfo = SongInfoListFromAzure.FirstOrDefault(x => x.SongUrl == AppSettings.AzureTab.SelectedSongUrl);
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
            Mouse.OverrideCursor = Cursors.Wait;

            StatusMessage = $"Loading song list from Azure SQL DB via Web API ({AppSettings.WebApiAuthInfo})...please wait";

            var list = await _azureService.GetAllSongInfoListAsync(AppSettings.IsWebApiViaAuth);
            SongInfoListFromAzure = new ObservableCollection<SongInfo>(list);

            StatusMessage = list.Count == 0 ? $"No song found at Azure SQL DB via Web API ({AppSettings.WebApiAuthInfo})" : 
                                                $"Song list loaded from Azure SQL DB via Web API ({AppSettings.WebApiAuthInfo})";
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
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    [RelayCommand]
    private async Task DeleteAzure()
    {
        if (MessageBox.Show("Are you sure you want to delete the selected song info from Azure SQL DB via Web API?", "Confirmation",
                           MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            Mouse.OverrideCursor = Cursors.Wait;
            
            StatusMessage = $"Deleting song from Azure SQL DB via Web API ({AppSettings.WebApiAuthInfo})...please wait";

            string error = await _azureService.DeleteSongInfoAsync(SelectedSongInfoFromAzure!.Id, AppSettings.IsWebApiViaAuth);
            if (error.IsBlank())
            {
                List<SongInfo> list = await _azureService.GetAllSongInfoListAsync(AppSettings.IsWebApiViaAuth);
                // Note: the following will cause SelectedSongInfoFromAzure in XAML clear binding (i.e. set SelectedSongInfoFromAzure to null),
                //          hence triggering OnSelectedSongInfoFromAzureChanged()'s 'value == null' logic 
                SongInfoListFromAzure = new ObservableCollection<SongInfo>(list);
                UpdateAzureTabButtons();

                StatusMessage = $"Song deleted from Azure SQL DB via Web API ({AppSettings.WebApiAuthInfo})";
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
        finally
        {
            Mouse.OverrideCursor = null;
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
    
    partial void OnSelectedSongInfoFromAzureChanged(SongInfo? value)
    {
        if (value == null)
        {
            SongInfoViewModel.UpdateSongInfoPanel(null, null, string.Empty);
            AzureWebView2Control.Source = Constants.YouTubeHomeUri;
            AppSettings.AzureTab.SelectedSongSummary = "YouTube home";
            AppSettings.AzureTab.SelectedSongUrl = Constants.YouTubeHomeUrl;
        }
        else
        {
            SongInfoViewModel.UpdateSongInfoPanel(value.CoverUrl, value.ToString(), value.Lyrics);
            AzureWebView2Control.Source = new Uri(value.SongUrl);
            AppSettings.AzureTab.SelectedSongSummary = value.ToString();
            AppSettings.AzureTab.SelectedSongUrl = value.SongUrl;
        }
        UpdateAzureTabButtons();
    }

    private void UpdateAzureTabButtons()
    {
        IsDeleteAzureEnabled = SongInfoListFromAzure.Count > 0 && SelectedSongInfoFromAzure != null;
    }
}

