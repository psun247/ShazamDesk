using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ShazamCore.Services;
using WpfShazam.Shazam;
using WpfShazam.Azure;
using WpfShazam.SqlServer;
using WpfShazam.Settings;
using WpfShazam.SongInfoPanel;

namespace WpfShazam.Main;

public partial class BaseViewModel : ObservableRecipient, ISongInfoPanelMessaging
{
    public BaseViewModel(ILocalSettingsService localsettingsService)
    {
        _localsettingsService = localsettingsService;
        SongInfoViewModel = new SongInfoViewModel(this);
    }

    protected string _ViaGrpcServiceOrWebAPI => AppSettings.IsGrpcService ?
                                                    "via gRPC service" : $"via Web API ({AppSettings.WebApiAuthInfo})";    
    protected ILocalSettingsService _localsettingsService;
    public AppSettings AppSettings => _localsettingsService.AppSettings;

    public SongInfoViewModel SongInfoViewModel { get; }
    [ObservableProperty]
    string _statusMessage = string.Empty;
    [ObservableProperty]
    bool _isErrorStatusMessage;

    // Flags for communicating between VMs (hence static)
    protected static bool _IsAzureTabInSync;
    protected static bool _IsSqlServerTabInSync;

    protected bool _isCommandBusy;
    public bool IsCommandNotBusy => !_isCommandBusy;
    public string ViaWebApiOrGrpcInfo => AppSettings.IsGrpcService ? "via gRPC service" : "via Web API";

    // Handle text red color via DataTrigger with IsErrorStatusMessage (not necessarily an error message)
    protected string ErrorStatusMessage
    {
        set
        {
            IsErrorStatusMessage = true;
            StatusMessage = value;

#pragma warning disable MVVMTK0034
            // Set it back to false (without binding, hence _isErrorStatusMessage) for next StatusMessage
            // (see OnStatusMessageChanging())
            _isErrorStatusMessage = false;
        }
    }

    // ISongInfoPanelMessaging (also see SongInfoViewModel.ExpandOrCollapseSongInfoPanel())
    public void SongInfoPanelVisibleChanged(bool visible)
    {
        if (this is ShazamViewModel)
        {
            AppSettings.ShazamTab.IsSongInfoPanelVisible = visible;
        }
        else if (this is AzureViewModel)
        {
            AppSettings.AzureTab.IsSongInfoPanelVisible = visible;
        }
        else if (this is SqlServerViewModel)
        {
            AppSettings.SqlServerTab.IsSongInfoPanelVisible = visible;
        }
    }

    // ISongInfoPanelMessaging
    public void NotifyCopiedToClipboard(string message)
    {
        StatusMessage = message;
    }

    partial void OnStatusMessageChanging(string value)
    {
        if (!IsErrorStatusMessage)
        {
            // 'IsErrorStatusMessage = false;' doesn't work since IsErrorStatusMessage is already false
            OnPropertyChanged(nameof(IsErrorStatusMessage));
        }
    }

    protected async Task HandleHttpRequestExceptionAsync(HttpRequestException ex, IAzureService azureService)
    {
        if (!AppSettings.IsGrpcService && AppSettings.IsWebApiViaAuth && ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // 401 (Unauthorized) via auth when toke expires (60 minutes), so generate a new token                
            await azureService.UseNewAccessTokenAsync();
            ErrorStatusMessage = $"{ex.Message} New access token has been created.  Please click Refresh button to try again.";
        }
        else if (ex.InnerException != null)
        {
            ErrorStatusMessage = $"{ex.Message} ({ex.InnerException.Message})";
        }
        else
        {
            ErrorStatusMessage = ex.Message;
        }
    }
}
