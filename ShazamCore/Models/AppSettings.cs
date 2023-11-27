namespace ShazamCore.Models
{
    public class AppSettings
    {
        public const string ShazamTabName = "ShazamTab";
        public const string AzureTabName = "AzureTab";
        public const string SqlServerTabName = "SqlServerTab";
        public const string AboutTabName = "AboutTab";

        // Shown in device combo box
        public string SelectedDeviceName { get; set; } = string.Empty;
        // Used for Shazam API
        public string SelectedDeviceID { get; set; } = string.Empty;
        // true if SQL Server properly installed and configured
        public bool IsSqlServerEnabled { get; set; }
        // Song info section on the right
        public bool IsSongInfoSectionVisible { get; set; } = true;
        // The following are saved on app shutdown, instead of dynamically on changed
        //
        // Selected tab
        public string SelectedTabName { get; set; } = ShazamTabName;
        // SongInfo.SongUrl
        public string SelectedSongUrl { get; set; } = string.Empty;
        // Auth or no-auth Web API
        public bool IsWebApiViaAuth { get; set; }
    }
}
