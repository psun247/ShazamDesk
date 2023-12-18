using WinUI3Shazam.Helpers;

namespace WinUI3Shazam.Models;

// Note: this is used in WinUI3 only (ShazamCore.Models.AppSettings for WPF)
public class AppSettings
{
    public const string ShazamTabName = "ShazamTab";
    public const string AzureTabName = "AzureTab";
    public const string SqlServerTabName = "SqlServerTab";

    // Shown in device combo box
    public string SelectedDeviceName { get; set; } = string.Empty;
    // Used for Shazam API
    public string SelectedDeviceID { get; set; } = string.Empty;
    // Selected tab
    public string SelectedTabName { get; set; } = ShazamTabName;
    // From UI
    public string SelectedShazamTabSongUrl { get; set; } = Constants.YouTubeHomeUrl;
    // SongInfo.ToString()
    public string SelectedAzureTabSongSummary { get; set; } = string.Empty;
    // SongInfo.SongUrl
    public string SelectedAzureTabSongUrl { get; set; } = Constants.YouTubeHomeUrl;
    // Auth or no-auth Web API
    public bool IsWebApiViaAuth { get; set; }
}
