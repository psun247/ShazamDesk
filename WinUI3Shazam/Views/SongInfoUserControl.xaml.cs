using Microsoft.UI.Xaml.Controls;
using WinUI3Shazam.ViewModels;

namespace WinUI3Shazam.Views;

public sealed partial class SongInfoUserControl : UserControl
{    
    public SongInfoUserControl()
    {
        InitializeComponent();
    }

    // Set by parent. The new is just a place holder.
    public SongInfoViewModel ViewModel { get; set; } = new SongInfoViewModel(null);
}
