using ShazamCore.Models;
using WinUI3Shazam.Contracts.Services;

namespace WinUI3Shazam.ViewModels;

public partial class SqlServerViewModel : BaseViewModel
{
    public SqlServerViewModel(ILocalSettingsService localsettingsService)
                                : base(localsettingsService)
    {
    }
   
    public void OnSqlServerTabActivated()
    {
        AppSettings.SelectedTabName = Models.AppSettings.SqlServerTabName;        
    }
}
