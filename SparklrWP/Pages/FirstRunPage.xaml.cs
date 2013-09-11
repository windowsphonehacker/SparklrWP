using Microsoft.Phone.Controls;
using System;
using System.IO.IsolatedStorage;

namespace SparklrWP.Pages
{
    public partial class FirstRunPage : PhoneApplicationPage
    {
        public FirstRunPage()
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
                NavigationService.Navigate(new Uri("/Pages/LoginPage.xaml", UriKind.Relative));
            }
        }
    }
}
