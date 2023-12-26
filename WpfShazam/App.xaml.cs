// Copyright(c) 2023-2024 Peter Sun
using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestoreWindowPlace;
using ShazamCore.Services;
using WpfShazam.Main;
using WpfShazam.Shazam;
using WpfShazam.Azure;
using WpfShazam.SqlServer;
using WpfShazam.WinUI3;
using WpfShazam.Settings;
using WpfShazam.About;

namespace WpfShazam;

public partial class App : Application
{
    private WindowPlace? _windowPlace;

    public App()
    {
        InitializeComponent();

        Host = 
        Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Services                            
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IAzureService, AzureService>();
            // Note: ensure to match your MS SQL Server installation and configuration        
            string connectionString = "Data Source=localhost\\SQLDev2019;Initial Catalog=WpfShazamDB2;Integrated Security=True;Encrypt=False;MultipleActiveResultSets=True";
            services.AddSingleton<SqlServerService>(x => new SqlServerService(connectionString));

            // ViewModels            
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ShazamViewModel>();
            services.AddSingleton<AzureViewModel>();
            services.AddSingleton<SqlServerViewModel>();
            services.AddSingleton<WinUI3ViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<AboutViewModel>();
        }).
        Build();
    }

    protected async override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var localsettingsService = App.GetService<ILocalSettingsService>();
            localsettingsService.InitializeAppSettings();

            // Must do this here (after localsettingsService's init) because ShazamViewModel.Initialize() would be too late
            // where MainWindow_Activated and page loaded event can happen at the same time!
            var azureService = App.GetService<IAzureService>();
            await azureService.CreateWebApiClientsAsync();
            
            var mainViewModel = App.GetService<MainViewModel>();
            var mainWindow = new MainWindow(mainViewModel);
            Current.MainWindow = mainWindow;
            SetupRestoreWindowPlace(mainWindow);
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), $"This app will exit on error ({ex.GetType()})");
            Current?.Shutdown();
        }
    }

    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host { get; }

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        try
        {
            _windowPlace?.Save();
        }
        catch (Exception)
        {
        }
    }

    private void SetupRestoreWindowPlace(MainWindow mainWindow)
    {
        string windowPlaceConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfShazamWindowPlace.config");
        _windowPlace = new WindowPlace(windowPlaceConfigFilePath);
        _windowPlace.Register(mainWindow);
    }
}
