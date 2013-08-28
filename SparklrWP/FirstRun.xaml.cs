using Microsoft.Phone.Controls;
using System;
using System.IO.IsolatedStorage;

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
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("firstruncheck"))
            {
                IsolatedStorageSettings.ApplicationSettings.Add("firstruncheck", true);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            }
        }
    }
}
