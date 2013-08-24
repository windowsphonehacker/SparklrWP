using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.IO;
using SparklrLib;

namespace SparklrWP
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            App.Client = new SparklrClient();
        }

        private bool postcallback(string jsonData)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(jsonData);
            });
            return true;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            App.Client.LoggedIn += Client_LoggedIn;
            App.Client.Login(usernameBox.Text, passwordBox.Password);
        }

        void Client_LoggedIn(object sender, SparklrClient.LoggedInEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (e.Error)
                {
                    MessageBox.Show("Error");
                }
                else
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            });

        }
    }
}