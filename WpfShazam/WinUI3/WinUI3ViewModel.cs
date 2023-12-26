using WpfShazam.Main;
using WpfShazam.Settings;

namespace WpfShazam.WinUI3;

public partial class WinUI3ViewModel : BaseViewModel
{
    public WinUI3ViewModel(ILocalSettingsService localsettingsService)
                            : base(localsettingsService)
    {
    }

    public void OnWinUI3TabActivated()
    {
        AppSettings.SelectedTabName = AppSettings.WinUI3TabName;
    }
}