using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShazamCore.Helpers;
using ShazamCore.Models;
using ShazamCore.Services;
using WpfShazam.Main;

namespace WpfShazam.SongInfoPanel;

// Note: on app start, there may be many binding error messages in Debug Window
//          because SongInfoViewModel is not yet set as DataContext for SongInfoUserControl yet
public partial class SongInfoViewModel : ObservableRecipient
{
    private const string _ReadyToListen = "Ready to 'Listen to'. When a song is identified, its lyrics is queried and displayed (if found).";
    private const string _SongLyricsNotFound = "Not found";
    // In WinUI3, null will cause Image binging crash, but in WPF, it needs to be null for no image. Here use predefined image for null.
    private const string _NullImage = "/Assets/WpfShazam.ico";

    // If lyricsApiKey not working, get a new one at: https://genius.com/developers
    private LyricsService _lyricsService = new LyricsService("0LoCEwmokQ1E865SpcLGyVUJvcVYtWfAAsTndJPcz7vWWFxTbqflNXNvRyt5vJzZ");

    private ISongInfoPanelMessaging _songInfoPanelMessaging;

    public SongInfoViewModel(ISongInfoPanelMessaging songInfoPanelMessaging)
    {
        _songInfoPanelMessaging = songInfoPanelMessaging;
    }

    [ObservableProperty]
    Visibility _songInfoPanelVisibility = Visibility.Visible;
    [ObservableProperty]
    string? _songCoverUrl = _NullImage;
    [ObservableProperty]
    string _songInfoText = _ReadyToListen;
    [ObservableProperty]
    string? _songLyrics;

    public void UpdateSongInfoPanel(string? songCoverUrl, string? songInfoText, string? songLyrics)
    {
        SongCoverUrl = songCoverUrl ?? _NullImage;
        SongInfoText = songInfoText ?? _ReadyToListen;
        SongLyrics = songLyrics;
    }

    public async Task<bool> UpdateSongInfoPanelAsync(VideoInfo videoInfo)
    {
        bool updated = true;
        try
        {
            SongCoverUrl = videoInfo.CoverUrl;
            SongInfoText = videoInfo.ToString();

            string lyrics = await _lyricsService.GetLyricsAsync(videoInfo.Song, videoInfo.Artist);
            if (lyrics.IsNotBlank())
            {
                SongLyrics = lyrics;
            }
            else
            {
                SongLyrics = _SongLyricsNotFound;
            }
        }
        catch (Exception)
        {
            updated = false;
        }
        return updated;
    }

    [RelayCommand]
    private void CopySongInfo()
    {
        var sb = new StringBuilder();
        if (SongInfoText != _ReadyToListen)
        {
            sb.AppendLine(SongInfoText);
            sb.AppendLine(string.Empty);

            if (!string.IsNullOrWhiteSpace(SongCoverUrl))
            {
                sb.AppendLine(SongCoverUrl);
                sb.AppendLine(string.Empty);
            }
        }
        if (!string.IsNullOrWhiteSpace(SongLyrics) && SongLyrics != _SongLyricsNotFound)
        {
            sb.Append(SongLyrics);
        }

        string result = sb.ToString();
        if (result.IsNotBlank())
        {
            Clipboard.SetText(result);

            _songInfoPanelMessaging.NotifyCopiedToClipboard("Song info copied to clipboard");
        }
    }

    [RelayCommand]
    private void ExpandOrCollapseSongInfoPanel()
    {
        SongInfoPanelVisibility = (SongInfoPanelVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

        // See BaseViewModel's ISongInfoPanelMessaging / SongInfoPanelVisibleChanged()
        if (_songInfoPanelMessaging != null)
        {
            _songInfoPanelMessaging.SongInfoPanelVisibleChanged(SongInfoPanelVisibility == Visibility.Visible);
        }
    }

    partial void OnSongCoverUrlChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SongInfoText = _ReadyToListen;
        }
    }
}
