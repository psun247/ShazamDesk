using WinUI3Shazam.Models;

namespace WinUI3Shazam.Contracts.Services;

public interface ILocalSettingsService
{
    AppSettings AppSettings { get; }
    string SettingsFilePath { get; }
    // Create or load from local store into AppSettings
    void InitializeAppSettings();
    // Save AppSettings to local store
    void SaveAppSettings();
}