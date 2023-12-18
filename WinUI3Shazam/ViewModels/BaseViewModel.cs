using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using CommunityToolkit.Mvvm.ComponentModel;
using ShazamCore.Services;
using WinUI3Shazam.Contracts.Services;

namespace WinUI3Shazam.ViewModels;

public partial class BaseViewModel : ObservableRecipient
{
    private static readonly Brush _BlackBrush = new SolidColorBrush(Colors.Black);
    private static readonly Brush _RedBrush = new SolidColorBrush(Colors.Red);
    
    private bool _isErrorStatusMessage;

    public BaseViewModel(ILocalSettingsService localsettingsService)
    {
        _localsettingsService = localsettingsService;
        SongInfoViewModel = new SongInfoViewModel(this);
    }

    protected ILocalSettingsService _localsettingsService;
    public Models.AppSettings AppSettings => _localsettingsService.AppSettings;

    public SongInfoViewModel SongInfoViewModel { get; }
    [ObservableProperty]
    string _statusMessage = string.Empty;
    [ObservableProperty]
    Brush _statusMessageBrush = _BlackBrush;

    // Flags for communicating between VMs (hence static)
    protected static bool _IsAzureTabInSync;
    protected static bool _IsSqlServerTabInSync;

    protected bool _isCommandBusy;
    public bool IsCommandNotBusy => !_isCommandBusy;

    // Handel text red color via DataTrigger with IsErrorStatusMessage (not necessarily an error message)
    protected string ErrorStatusMessage
    {
        set
        {
            _isErrorStatusMessage = true;
            StatusMessage = value;

            _isErrorStatusMessage = false;
        }
    }
    
    // https://xamlbrewer.wordpress.com/2022/03/09/a-dialog-service-for-winui-3/
    //
    // var confirmed = await App.MainRoot.ConfirmationDialogAsync(
    //            "What Pantone color do you prefer?",
    //        "Freedom Blue",
    //        "Energizing Yellow"
    //    );
    // 
    public static async Task<bool> ConfirmationDialogAsync(
                                         FrameworkElement element,
                                         string title,
                                         string yesButtonText,
                                         string noButtonText,
                                         string cancelButtonText)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = yesButtonText,
            SecondaryButtonText = noButtonText,
            CloseButtonText = cancelButtonText,
            XamlRoot = element.XamlRoot,
            RequestedTheme = element.ActualTheme
        };
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.None)
        {
            return false; // Could return null, if type is bool?
        }

        return (result == ContentDialogResult.Primary);
    }

    // In app init, require AppNotificationManager.Default.Register(), but it may fail on Windows 10 (see HandleInternalAsync())
    public static void SendNotificationToast(string message)
    {
        try
        {            
            // Title will be app name
            var toast = new AppNotificationBuilder()
                                .AddText(message)
                                .BuildNotification();
            AppNotificationManager.Default.Show(toast);            
        }
        catch (Exception)
        {            
        }
    }

    partial void OnStatusMessageChanging(string value)
    {
        StatusMessageBrush = _isErrorStatusMessage ? _RedBrush : _BlackBrush;
    }

    protected async Task HandleHttpRequestExceptionAsync(HttpRequestException ex, bool viaAuth, IAzureService azureService)
    {
        if (viaAuth && ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // 401 (Unauthorized) via auth when toke expires (60 minutes), so generate a new token                
            await azureService.UseNewAccessTokenAsync();
            ErrorStatusMessage = $"{ex.Message} New access token has been created.  Please click Refresh button to try again.";
        }
        else
        {
            ErrorStatusMessage = ex.Message;
        }
    }

}
