using System.Windows.Controls;

namespace WpfShazam.Azure;

public partial class AzureUserControl : UserControl
{
    private bool _isFirstLoaded;

    public AzureUserControl()
    {
        InitializeComponent();

        Loaded += AzureUserControl_Loaded;
    }

    private async void AzureUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is AzureViewModel azureViewModel)
        {
            if (!_isFirstLoaded)
            {
                _isFirstLoaded = true;

                azureViewModel.Initialize();
                SongInfo.DataContext = azureViewModel.SongInfoViewModel;
                return;
            }

            await azureViewModel.OnAzureTabActivated();
        }
    }
}