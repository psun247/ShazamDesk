using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3Shazam.ViewModels;

namespace WinUI3Shazam.Views;

public sealed partial class SqlServerPage : Page
{
    public SqlServerPage()
    {
        InitializeComponent();

        NavigationCacheMode = NavigationCacheMode.Enabled;
        ViewModel = App.GetService<SqlServerViewModel>();
        Loaded += SqlServerPage_Loaded;
    }

    public SqlServerViewModel ViewModel { get; }

    private void SqlServerPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.OnSqlServerTabActivated();
    }
}
