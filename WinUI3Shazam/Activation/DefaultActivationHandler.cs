using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using ShazamCore.Services;
using WinUI3Shazam.Contracts.Services;
using WinUI3Shazam.Models;
using WinUI3Shazam.ViewModels;

namespace WinUI3Shazam.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame?.Content == null;
    }

    // Note: app's init code in here
    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        AppNotificationManager.Default.Register();

        // Could do this before in the calling chain, but NavigateTo() is the first one needing AppSettings
        var localsettingsService = App.GetService<ILocalSettingsService>();
        localsettingsService.InitializeAppSettings();
        string pageKey = localsettingsService.AppSettings.SelectedTabName switch
        {
            AppSettings.ShazamTabName => typeof(ShazamViewModel).FullName!,
            AppSettings.AzureTabName => typeof(AzureViewModel).FullName!,
            AppSettings.SqlServerTabName => typeof(SqlServerViewModel).FullName!,
            _ => typeof(ShazamViewModel).FullName!
        };

        // Must do this here (after localsettingsService's init) because ShazamViewModel.Initialize() would be too late
        // where MainWindow_Activated and page loaded event can happen at the same time!
        var azureService = App.GetService<IAzureService>();
        await azureService.CreateWebApiClientsAsync();

        _navigationService.NavigateTo(pageKey, args.Arguments);

        BaseViewModel.SendNotificationToast("WinUI3Shazam is up and running!");
    }
}
