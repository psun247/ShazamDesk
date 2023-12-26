namespace WpfShazam.Settings;

// Note: this ILocalSettingsService is separate from the one in WinUI3Shazam project
public interface ILocalSettingsService
{
    AppSettings AppSettings { get; }
    string SettingsFilePath { get; }
    // Create or load from local store into AppSettings
    void InitializeAppSettings();
    // Save AppSettings to local store
    void SaveAppSettings();
}
