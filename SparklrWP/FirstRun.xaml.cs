using Microsoft.Phone.Controls;
using System;

namespace SparklrWP
{
    public partial class FirstRun : PhoneApplicationPage
    {
        public FirstRun()
        {
            InitializeComponent();
            HelloAni.Begin();

        }

        private void OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));

        }
    }
}
