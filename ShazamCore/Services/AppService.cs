using ShazamCore.Models;
using ShazamCore.Helpers;

namespace ShazamCore.Services
{
    public class AppService
    {
        private string _appConfigFilePath = string.Empty;

        public AppService(string appConfigFilePath)
        {
            _appConfigFilePath = appConfigFilePath;
            LoadAppSettings();
        }

        public AppSettings AppSettings { get; private set; } = new AppSettings();

        public void UpdateDeviceinfo(string selectedDeviceName, string selectedDeviceID)
        {
            AppSettings.SelectedDeviceName = selectedDeviceName;
            AppSettings.SelectedDeviceID = selectedDeviceID;
            SaveAppSettings();
        }

        public void UpdateSqlServerEnabled(bool isSqlServerEnabled)
        {
            AppSettings.IsSqlServerEnabled = isSqlServerEnabled;
            SaveAppSettings();
        }

        public void UpdateSongInfoSectionVisibility(bool isVisible)
        {
            AppSettings.IsSongInfoSectionVisible = isVisible;
            SaveAppSettings();
        }

        public void SaveAppSettings()
        {
            try
            {
                JsonHelper.SaveAsJsonToFile(AppSettings, _appConfigFilePath);
            }
            catch (Exception)
            {
            }
        }

        private void LoadAppSettings()
        {
            AppSettings? appSettings = null;
            try
            {
                appSettings = JsonHelper.DeserializeFromFile<AppSettings?>(_appConfigFilePath);
            }
            catch (Exception)
            {
            }
            AppSettings = appSettings ?? new AppSettings();
        }
    }
}
