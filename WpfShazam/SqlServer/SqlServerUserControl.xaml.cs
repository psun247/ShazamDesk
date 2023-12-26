using System.Windows.Controls;

namespace WpfShazam.SqlServer;

public partial class SqlServerUserControl : UserControl
{
    private bool _isFirstLoaded;

    public SqlServerUserControl()
    {
        InitializeComponent();

        Loaded += SqlServerUserControl_Loaded;
    }

    private void SqlServerUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SqlServerViewModel sqlServerViewModel)
        {
            if (!_isFirstLoaded)
            {
                _isFirstLoaded = true;

                sqlServerViewModel.Initialize();
                SongInfo.DataContext = sqlServerViewModel.SongInfoViewModel;
                return;
            }

            sqlServerViewModel.OnSqlServerTabActivated();
        }
    }
}