using System.Windows.Controls;

namespace WpfShazam.Shazam;

public partial class ShazamUserControl : UserControl
{
    private bool _isFirstLoaded;

    public ShazamUserControl()
    {
        InitializeComponent();

        Loaded += ShazamUserControl_Loaded;
    }

    private void ShazamUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ShazamViewModel shazamViewModel)
        {
            if (!_isFirstLoaded)
            {
                _isFirstLoaded = true;

                shazamViewModel.Initialize();
                SongInfo.DataContext = shazamViewModel.SongInfoViewModel;
                return;
            }

            shazamViewModel.OnShazamTabActivated();
        }
    }
}
