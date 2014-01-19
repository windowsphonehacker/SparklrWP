using Microsoft.Phone.Controls;
using SparklrWP.Resources;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;

namespace SparklrWP.Pages
{
    public partial class FirstRunPage : PhoneApplicationPage
    {
        public FirstRunPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                //Show the first run welcome message
                MessageBox.Show(
                    AppResources.FirstRunMsgBoxContent,
                    AppResources.FirstRunMsgBoxTitle, MessageBoxButton.OK, MessageBoxButton.OKCancel);
            }
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

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //Exit the app on back key press
            while (NavigationService.BackStack.Any())
            {
                NavigationService.RemoveBackEntry();
            }
            base.OnBackKeyPress(e);
        }
    }
}
