using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;
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

namespace WpfShazam.SqlServer;

public partial class SqlServerViewModel : BaseViewModel
{
    private SqlServerService _sqlServerService;

    public SqlServerViewModel(ILocalSettingsService localsettingsService, SqlServerService sqlServerService)
                            : base(localsettingsService)
    {
        _sqlServerService = sqlServerService;
    }
    
    [ObservableProperty]
    string _currentVideoUrl = Constants.YouTubeHomeUrl;
    [ObservableProperty]
    ObservableCollection<SongInfo> _songInfoListFromSqlServer = new ObservableCollection<SongInfo>();
    [ObservableProperty]
    SongInfo? _selectedSongInfoFromSqlServer;
    public WebView2 SqlServerWebView2Control { get; private set; } = new();
    [ObservableProperty]
    bool _isDeleteSqlServerEnabled;
    public string SwitchModeButtonText => AppSettings.SqlServerTab.IsSqlServerEnabled ? "Switch to Demo Mode" : "Switch to SQL Server Mode";

    public void Initialize()
    {
        SqlServerWebView2Control = new WebView2
        {
            Name = "SqlServerWebView2",
            Source = Constants.YouTubeHomeUri,
        };
        OnPropertyChanged(nameof(SqlServerWebView2Control));
        SqlServerWebView2Control.SourceChanged += (s, e) =>
        {
            CurrentVideoUrl = SqlServerWebView2Control.Source.AbsoluteUri;
        };
        OnPropertyChanged(nameof(SqlServerWebView2Control));
        
        SongInfoViewModel.SongInfoPanelVisibility = AppSettings.SqlServerTab.IsSongInfoPanelVisible ?
                                                        Visibility.Visible : Visibility.Collapsed;
    }

    public void OnSqlServerTabActivated()
    {
        AppSettings.SelectedTabName = AppSettings.SqlServerTabName;        

        if (!_IsSqlServerTabInSync)
        {
            if (!LoadSongInfoListOnSqlServerTab())
            {
                // Ensure demo mode
                DemoModeBindSongInfoListFromSqlServer();
                AppSettings.SqlServerTab.IsSqlServerEnabled = false;
                OnPropertyChanged(nameof(SwitchModeButtonText));
            }

            // Auto-select SelectedSongInfoFromSqlServer
            var songInfo = SongInfoListFromSqlServer.FirstOrDefault(x => x.SongUrl == AppSettings.SqlServerTab.SelectedSongUrl);
            if (songInfo != null && songInfo != SelectedSongInfoFromSqlServer)
            {
                SelectedSongInfoFromSqlServer = songInfo;
            }

            _IsSqlServerTabInSync = true;
        }
        UpdateSqlServerTabButtons();
    }


    private bool LoadSongInfoListOnSqlServerTab()
    {
        return LoadSongInfoListOnSqlServerTab(AppSettings.SqlServerTab.IsSqlServerEnabled);
    }

    private bool LoadSongInfoListOnSqlServerTab(bool isSqlServerEnabled)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (isSqlServerEnabled)
            {
                SongInfoListFromSqlServer = new ObservableCollection<SongInfo>(_sqlServerService.GetAllSongInfoList());
            }
            else
            {
                // Could use a check-box to drive this when no SQL Server support / or just for UI development!
                DemoModeBindSongInfoListFromSqlServer();
            }

            StatusMessage = isSqlServerEnabled ? "SQL Server mode" : "Demo mode (predefined read-only list)";
            return true;
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            // e.g. Login failed for user 'xyz'
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
        return false;
    }

    partial void OnSelectedSongInfoFromSqlServerChanged(SongInfo? value)
    {
        if (value == null)
        {
            SongInfoViewModel.UpdateSongInfoPanel(null, null, string.Empty);
            SqlServerWebView2Control.Source = Constants.YouTubeHomeUri;
            AppSettings.SqlServerTab.SelectedSongSummary = "YouTube home";
            AppSettings.SqlServerTab.SelectedSongUrl = Constants.YouTubeHomeUrl;
        }
        else
        {
            SongInfoViewModel.UpdateSongInfoPanel(value.CoverUrl, value.ToString(), value.Lyrics);
            SqlServerWebView2Control.Source = new Uri(value.SongUrl);
            AppSettings.SqlServerTab.SelectedSongSummary = value.ToString();
            AppSettings.SqlServerTab.SelectedSongUrl = value.SongUrl;
        }
        UpdateSqlServerTabButtons();
    }

    [RelayCommand]
    private void DeleteSqlServer()
    {
        try
        {
            if (SelectedSongInfoFromSqlServer != null && !string.IsNullOrWhiteSpace(SelectedSongInfoFromSqlServer.CoverUrl))
            {
                if (MessageBox.Show("Are you sure you want to delete the selected song info?", "Confirmation",
                               MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                {
                    return;
                }

                if (_sqlServerService.DeleteSongInfo(SelectedSongInfoFromSqlServer.SongUrl))
                {
                    SongInfoListFromSqlServer = new ObservableCollection<SongInfo>(_sqlServerService.GetAllSongInfoList());
                    UpdateSqlServerTabButtons();
                    StatusMessage = "Song info deleted from SQL Server DB";
                }
                else
                {
                    ErrorStatusMessage = "Song info not found in SQL Server DB";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorStatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void SwitchMode()
    {
        try
        {
            if (AppSettings.SqlServerTab.IsSqlServerEnabled)
            {
                if (MessageBox.Show("Switching to Demo mode will display a predefined song list. Are you sure you want to switch?", "Confirmation",
                                   MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show("Switching to SQL Server mode requires SQL Server installed and configured properly (also check Data\\SqlServerContext.cs). " +
                                    "Are you sure you are ready and want to switch?", "Confirmation",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            if (LoadSongInfoListOnSqlServerTab(!AppSettings.SqlServerTab.IsSqlServerEnabled))
            {
                AppSettings.SqlServerTab.IsSqlServerEnabled = !AppSettings.SqlServerTab.IsSqlServerEnabled;
                UpdateSqlServerTabButtons();
                OnPropertyChanged(nameof(SwitchModeButtonText));
            }
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

    private void UpdateSqlServerTabButtons()
    {
        IsDeleteSqlServerEnabled = AppSettings.SqlServerTab.IsSqlServerEnabled &&
                                    SongInfoListFromSqlServer.Count > 0 && SelectedSongInfoFromSqlServer != null;
    }

    private void DemoModeBindSongInfoListFromSqlServer()
    {
        // Collect data from DebugDumpVideoInfo() to build SongInfoList:            
        // Artist: Chicago            
        // Song: You're the Inspiration           
        // CoverUrl: https://is1-ssl.mzstatic.com/image/thumb/Music116/v4/5a/3f/6f/5a3f6f72-28ef-c659-6bc5-43489c9147d8/5059460176041.jpg/400x400cc.jpg

        SongInfoListFromSqlServer = new ObservableCollection<SongInfo>
            {
                new SongInfo
                {
                    Artist = "Chicago",
                    Description = "You're the Inspiration",
                    CoverUrl = "https://is1-ssl.mzstatic.com/image/thumb/Music116/v4/5a/3f/6f/5a3f6f72-28ef-c659-6bc5-43489c9147d8/5059460176041.jpg/400x400cc.jpg",
                    Lyrics = "[Verse 1: Peter Cetera]\r\nYou know our love was meant to be\r\nThe kind of love that lasts forever\r\nAnd I want you here with me\r\nFrom tonight until the end of time\r\nYou should know, everywhere I go\r\nAlways on my mind, in my heart\r\nIn my soul, baby\r\n\r\n[Chorus: All]\r\nYou're the meaning in my life\r\nYou're the inspiration\r\nYou bring feeling to my life\r\nYou're the inspiration\r\nWanna have you near me\r\nI wanna have you hear me sayin'\r\n\"No one needs you more than I need you\"\r\n\r\n[Verse 2: Peter Cetera]\r\nAnd I know, yes, I know that it's plain to see\r\nSo in love when we're together\r\nNow I know (Now I know)\r\nThat I need you here with me\r\nFrom tonight until the end of time\r\nYou should know  (Yes, you need to know)\r\nEverywhere I go\r\nYou're always on my mind, you're in my heart\r\nIn my soul\r\n[Chorus: All]\r\nYou're the meaning in my life\r\nYou're the inspiration\r\nYou bring feeling to my life\r\nYou're the inspiration\r\nWanna have you near me\r\nI wanna have you hear me sayin'\r\n\"No one needs you more than I need you\"\r\n\r\n[Bridge: All]\r\nWanna have you near me\r\nI wanna have you hear me sayin'\r\n\"No one needs you more than I need you\"\r\n(No one needs you more)\r\n\r\n[Chorus: All]\r\nYou're the meaning in my life (Oh-oh-oh-oh-oh)\r\nYou're the inspiration (Oh)\r\nYou bring feeling to my life (Oh-oh-oh-oh-oh)\r\nYou're the inspiration\r\n\r\n[Outro: All]\r\nWhen you love somebody (Oh) 'til the end of time\r\nWhen you love somebody\r\nAlways on my mind\r\nNo one needs you more than I\r\nWhen you love somebody 'til the end of time\r\nWhen you love somebody\r\nAlways on my mind",
                    SongUrl = "https://youtu.be/CRfy1yorkec",
                },
                new SongInfo
                {
                    Artist = "Jennifer Warnes & Bill Medley",
                    Description = "(I've Had) The Time of My Life",
                    CoverUrl = "https://is1-ssl.mzstatic.com/image/thumb/Music124/v4/8c/78/99/8c7899fc-733e-e133-f901-43fb58837948/mzi.akzlzhny.jpg/400x400cc.jpg",
                    Lyrics = "[Chorus: Bill Medley & Jennifer Warnes]\r\nNow I've had the time of my life\r\nNo, I never felt like this before\r\nYes, I swear it's the truth\r\nAnd I owe it all to you\r\n'Cause I've had the time of my life\r\n\r\n\r\n[Verse 1: Bill Medley, Jennifer Warnes & Both]\r\nI've been waiting for so long\r\nNow I've finally found someone to stand by me\r\nWe saw the writing on the wall\r\nAs we felt this magical fantasy\r\n\r\n\r\n\r\n\r\n\r\n[Pre-Chorus: Bill Medley, Jennifer Warnes & Both]\r\nJust remember\r\nYou're the one thing\r\nI can't get enough of\r\nSo I'll tell you something\r\nThis could be love\r\nBecause\r\n[Chorus: Bill Medley & Jennifer Warnes, Bill Medley]\r\nI've had the time of my life\r\nNo, I never felt this way before\r\nYes, I swear it's the truth\r\nAnd I owe it all to you\r\nHey, baby\r\n\r\n[Verse 2: Jennifer Warnes & Bill Medley]\r\n\r\nI want you more than you'll ever know\r\nSo we'll just let it go\r\nDon't be afraid to lose control\r\nNo\r\n\r\n\r\n[Pre-Chorus: Bill Medley, Jennifer Warnes & Both]\r\nJust remember\r\nYou're the one thing\r\nI can't get enough of\r\nSo I'll tell you something\r\nThis could be love\r\nBecause\r\n[Chorus: Bill Medley & Jennifer Warnes, Bill Medley]\r\nI've had the time of my life\r\nNo, I never felt this way before\r\nYes, I swear it's the truth (I swear)\r\nAnd I owe it all to you\r\n'Cause I had the time of my life\r\nAnd I've searched through every open door\r\n'Til I found the truth\r\nAnd I owe it all to you\r\n\r\n[Instrumental Break]\r\n\r\n[Bridge: Bill Medley, Jennifer Warnes & Both]\r\nNow I've had the time of my life\r\nNo, I never felt this way before (Never felt this way)\r\nYes, I swear it's the truth\r\nAnd I owe it all to you\r\n\r\n[Chorus: Bill Medley & Jennifer Warnes, Jennifer Warnes, Bill Medley]\r\nI've had the time of my life\r\nNo, I never felt this way before (Never felt this way)\r\nYes, I swear it's the truth (It's the truth)\r\nAnd I owe it all to you\r\n'Cause I had the time of my life (The time of my life)\r\nAnd I've searched through every open door\r\n'Til I found the truth\r\nAnd I owe it all to you",
                    SongUrl = "https://youtu.be/MOlzxfBSvp4"
                },
                new SongInfo
                {
                    Artist = "Céline Dion",
                    Description = "A New Day Has Come (Radio Remix)",
                    CoverUrl = "https://is1-ssl.mzstatic.com/image/thumb/Music114/v4/be/f0/4c/bef04c1f-0b13-5407-280f-e23180dcb011/886447917275.jpg/400x400cc.jpg",
                    Lyrics = "[Intro]\r\nA new day (Ah-ah)\r\nA new day (Ah-ah)\r\n\r\n[Verse 1]\r\nI was waiting for so long\r\nFor a miracle to come\r\nEveryone told me to be strong\r\nHold on, and don't shed a tear\r\nThrough the darkness and good times\r\nI knew I'd make it through\r\nAnd the world thought I had it all\r\nBut I was waiting for you\r\n\r\n[Pre-Chorus]\r\nHush, now\r\nI see a light in the sky\r\nOh, it's almost blinding me\r\nI can't believe I've been touched by an angel with love\r\n\r\n[Chorus]\r\nLet the rain come down and wash away my tears\r\nLet it fill my soul and drown my fears\r\nLet it shatter the walls for a new sun\r\nA new day has come\r\n(Oh, oh, oh)\r\n[Verse 2]\r\nWhere it was dark, now there’s light\r\nWhere there was pain, now there's joy\r\nWhere there was weakness, I found my strength\r\nAll in the eyes of a boy\r\n\r\n[Pre-Chorus]\r\nHush, now\r\nI see a light in the sky\r\nOh, it's almost blinding me\r\nI can't believe I've been touched by an angel with love\r\n\r\n[Chorus]\r\nLet the rain come down and wash away my tears\r\nLet it fill my soul and drown my fears (And drown my fears)\r\nLet it shatter the walls for a new sun\r\nA new day has\r\nLet the rain come down and wash away my tears\r\nLet it fill my soul and drown my fears\r\nLet it shatter the walls for a new sun\r\nA new day has come (Oh, a light, ah-ah)\r\n\r\n[Post-Chorus]\r\nHush, now (Ah-ah), I see a light in your eyes\r\nAll in the eyes of a boy (Ah-ah, a new day)\r\nI can't believe I've been touched by an angel with love (A new day)\r\nI can't believe I've been touched by an angel with love (A new day)\r\n(A new day) Oh\r\n[Outro]\r\nHush, now (Ah-ah)\r\nA new day (Ah-ah)\r\nHush, now (Ah-ah)\r\nA new day (Ah-ah)",
                    SongUrl = "https://youtu.be/NaGLVS5b_ZY"
                },
            };
    }
}

