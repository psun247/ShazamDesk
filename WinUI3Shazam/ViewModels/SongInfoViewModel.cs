using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShazamCore.Helpers;
using ShazamCore.Models;
using ShazamCore.Services;

namespace WinUI3Shazam.ViewModels;

public partial class SongInfoViewModel : ObservableObject
{
    public const string ReadyToListen = "Ready to 'Listen to'. When a song is identified, its lyrics is queried and displayed (if found).";
    private const string _SongLyricsNotFound = "Not found";
    // In WinUI3, null will cause Image binging crash, but in WPF, it needs to be null for no image. Here use predefined image for null.
    private const string _NullImage = "/Assets/WinUI3Shazam.ico";

    // If lyricsApiKey not working, get a new one at: https://genius.com/developers
    private LyricsService _lyricsService = new LyricsService("0LoCEwmokQ1E865SpcLGyVUJvcVYtWfAAsTndJPcz7vWWFxTbqflNXNvRyt5vJzZ");

    private BaseViewModel _baseViewModel;

    public SongInfoViewModel(BaseViewModel? baseViewModel)
    {
        // Note: the ! is just to avoid compile warning, and I know I won't use a null baseViewModel
        _baseViewModel = baseViewModel!;
    }    

    [ObservableProperty]
    Visibility _songInfoSectionVisibility = Visibility.Visible;
    [ObservableProperty]
    string? _songCoverUrl = _NullImage;
    [ObservableProperty]
    string _songInfoText = ReadyToListen;
    [ObservableProperty]
    string? _songLyrics;

    public void UpdateSongInfoSection(string? songCoverUrl, string songInfoText, string? songLyrics)
    {
        SongCoverUrl = songCoverUrl ?? _NullImage;
        SongInfoText = songInfoText;
        SongLyrics = songLyrics;
    }

    public async Task<bool> UpdateSongInfoSectionAsync(VideoInfo videoInfo)
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
        if (SongInfoText != ReadyToListen)
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
            var package = new DataPackage();
            package.SetText(result);
            Clipboard.SetContent(package);

            BaseViewModel.SendNotificationToast("Song info copied to clipboard");
        }
    }    

    partial void OnSongCoverUrlChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SongInfoText = ReadyToListen;
        }
    }
}
