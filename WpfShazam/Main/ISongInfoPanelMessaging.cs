namespace WpfShazam.Main;

public interface ISongInfoPanelMessaging
{
    void SongInfoPanelVisibleChanged(bool visible);
    void NotifyCopiedToClipboard(string message);
}