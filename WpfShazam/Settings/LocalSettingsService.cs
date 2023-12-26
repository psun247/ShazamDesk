﻿using System;
using System.IO;
using ClientServerShared;
using ShazamCore.Helpers;

namespace WpfShazam.Settings
{
    public class LocalSettingsService : ILocalSettingsService
    {
        private const string _defaultApplicationDataFolder = "WpfShazam";
#if DEBUG
        private const string _defaultLocalSettingsFile = "WpfShazamSettings_Debug.json";
#else
    private const string _defaultLocalSettingsFile = "WpfShazamSettings.json";
#endif

        private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private readonly string _applicationDataFolder;

        // If this class doesn't know how to access file system (but do), I would inject IFileService fileService
        public LocalSettingsService()
        {
            // C:\Users\peter\AppData\Local\WpfShazam
            _applicationDataFolder = Path.Combine(_localApplicationData, _defaultApplicationDataFolder);
            // C:\Users\peter\AppData\Local\WpfShazam\WpfShazamSettings.json
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

                if (appSettings != null)
                {
                    // Note: empty / invalid SelectedSongUrl will cause HyperLink binding to crash!                    
                    appSettings.ShazamTab.SelectedSongUrl = ValidOrDefault(appSettings.ShazamTab.SelectedSongUrl);
                    appSettings.AzureTab.SelectedSongUrl = ValidOrDefault(appSettings.AzureTab.SelectedSongUrl);
                    appSettings.SqlServerTab.SelectedSongUrl = ValidOrDefault(appSettings.SqlServerTab.SelectedSongUrl);
                }
            }
            catch (Exception)
            {
            }
            AppSettings = appSettings ?? new AppSettings();
        }

        private string ValidOrDefault(string url)
        {
            string result = url;
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                result = Constants.YouTubeHomeUrl;
            }
            return result;
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
}