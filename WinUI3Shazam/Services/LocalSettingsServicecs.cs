using ShazamCore.Helpers;
using WinUI3Shazam.Contracts.Services;
using WinUI3Shazam.Models;

namespace WinUI3Shazam.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultApplicationDataFolder = "WinUI3Shazam";
#if DEBUG
    private const string _defaultLocalSettingsFile = "LocalSettings_Debug.json";
#else
    private const string _defaultLocalSettingsFile = "LocalSettings.json";
#endif

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;      

    // If this class doesn't know how to access file system (but do), I would inject IFileService fileService
    public LocalSettingsService()
    {
        // C:\Users\peter\AppData\Local\WinUI3Shazam
        _applicationDataFolder = Path.Combine(_localApplicationData, _defaultApplicationDataFolder);
        // C:\Users\peter\AppData\Local\WinUI3Shazam\LocalSettings.json
        SettingsFilePath = Path.Combine(_applicationDataFolder, _defaultLocalSettingsFile);
    }

    // Only one LocalSettingsService and one AppSettings in the app
    public AppSettings AppSettings { get; private set; } = new AppSettings();
    public string SettingsFilePath { get; }

    // Called on app start to populate AppSettings
    public void InitializeAppSettings()
    {
        if (!Directory.Exists(_applicationDataFolder))
        {
            Directory.CreateDirectory(_applicationDataFolder);
        }

        if (!File.Exists(SettingsFilePath))
        {
            AppSettings = new AppSettings();
            SaveAppSettings();
            return;
        }

        AppSettings? appSettings = null;
        try
        {
            appSettings = JsonHelper.DeserializeFromFile<AppSettings?>(SettingsFilePath);

            // Note: empty / invalid SelectedSongUrl will cause HyperLink binding to crash!
            if (!Uri.TryCreate(appSettings!.SelectedShazamTabSongUrl, UriKind.Absolute, out Uri? uri))
            {
                appSettings.SelectedShazamTabSongUrl = Helpers.Constants.YouTubeHomeUrl;
            }
            if (!Uri.TryCreate(appSettings!.SelectedAzureTabSongUrl, UriKind.Absolute, out uri))
            {
                appSettings.SelectedAzureTabSongUrl = Helpers.Constants.YouTubeHomeUrl;
            }
        }
        catch (Exception)
        {
        }
        AppSettings = appSettings ?? new AppSettings();
    }   

    // Usually callled on app exit
    public void SaveAppSettings()
    {
        try
        {
            JsonHelper.SaveAsJsonToFile(AppSettings, SettingsFilePath);
        }
        catch (Exception)
        {
        }
    }
}
