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
using Microsoft.Phone.Tasks;

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

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Ani1.Begin();

        }

        private void Signup_click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("Sparklr.me home page where you can sign up will opened in a browser", "SparklrWP", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                new WebBrowserTask() { Uri = new Uri("http://sparklr.me") }.Show();
            }
        }

        private void about_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Sparklr Branding (C) Jonathan Warner \n\n Application Development Team: Marocco2, jessenic, EaterOfCorpses And TheInterframe\n\n Big Thanks to Jonathan!", "About Sparklr WP V1.0", MessageBoxButton.OK);
        }
    }
}