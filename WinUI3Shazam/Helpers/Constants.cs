namespace WinUI3Shazam.Helpers;

public static class Constants
{
    public static readonly string AppConfigFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\WinUI3ShazamConfig.json";
    public static readonly string YouTubeHomeUrl = "https://www.youtube.com";
    public static readonly Uri YouTubeHomeUri = new Uri(YouTubeHomeUrl);
}
