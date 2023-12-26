using ClientServerShared;
using System.Text.Json.Serialization;

namespace WpfShazam.Settings;

public class AppSettings
{
    public const string ShazamTabName = "ShazamTab";
    public const string AzureTabName = "AzureTab";
    public const string SqlServerTabName = "SqlServerTab";
    public const string WinUI3TabName = "WinUI3Tab";
    public const string AboutTabName = "AboutTab";

    // Auth or no-auth Azure Web API for the whole app
    public bool IsWebApiViaAuth { get; set; }
    [JsonIgnore]
    public string WebApiAuthInfo => IsWebApiViaAuth ? "JWT token" : "no auth";
    // Selected tab
    public string SelectedTabName { get; set; } = ShazamTabName;
    public ShazamTabSettings ShazamTab { get; set; } = new ShazamTabSettings();
    public AzureTabSettings AzureTab { get; set; } = new AzureTabSettings();
    public SqlServerTabSettings SqlServerTab { get; set; } = new SqlServerTabSettings();
}

public class ShazamTabSettings
{
    // Shown in device combo box
    public string SelectedDeviceName { get; set; } = string.Empty;
    // Used for Shazam API
    public string SelectedDeviceID { get; set; } = string.Empty;
    public string SelectedSongUrl { get; set; } = Constants.YouTubeHomeUrl;
    public bool IsSongInfoPanelVisible { get; set; } = true;
    [JsonIgnore]
    public string IsSongInfoPanelVisibleText => IsSongInfoPanelVisible ? "Yes" : "No";
}

public class AzureTabSettings
{
    // SongInfo.ToString()
    public string SelectedSongSummary { get; set; } = string.Empty;
    // SongInfo.SongUrl
    public string SelectedSongUrl { get; set; } = Constants.YouTubeHomeUrl;
    public bool IsSongInfoPanelVisible { get; set; } = true;
    [JsonIgnore]
    public string IsSongInfoPanelVisibleText => IsSongInfoPanelVisible ? "Yes" : "No";
}

public class SqlServerTabSettings
{
    // SQL Server or Demo mode
    public bool IsSqlServerEnabled { get; set; }
    public string IsSqlServerEnabledText => IsSqlServerEnabled ? "Yes" : "No";
    public string SelectedSongSummary { get; set; } = string.Empty;
    public string SelectedSongUrl { get; set; } = Constants.YouTubeHomeUrl;
    public bool IsSongInfoPanelVisible { get; set; } = true;
    [JsonIgnore]
    public string IsSongInfoPanelVisibleText => IsSongInfoPanelVisible ? "Yes" : "No";
}