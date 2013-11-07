using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using System;

namespace SparklrWP.Pages
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }
#if RELEASEDEVELOPERS
        private void TextBlock_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/UOVODIPASQUA.xaml", UriKind.Relative));
        }
#endif

    }
}