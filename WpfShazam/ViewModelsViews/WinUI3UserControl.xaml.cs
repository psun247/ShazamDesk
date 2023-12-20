using System;
using System.Windows.Controls;
using ShazamCore.Helpers;

namespace WpfShazam.ViewModelsViews
{    
    public partial class WinUI3UserControl : UserControl
    {
        public WinUI3UserControl()
        {
            InitializeComponent();

            WinUI3Hyperlink.RequestNavigate += WinUI3Hyperlink_RequestNavigate;
        }

        private async void WinUI3Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {            
            try
            {
                await GeneralHelper.OpenWithBrowserAsync("https://github.com/psun247/ShazamDesk");
            }
            catch (Exception)
            {
            }
        }
    }
}
