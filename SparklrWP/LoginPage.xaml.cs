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
using System.IO.IsolatedStorage;
using System.Security.Cryptography;

namespace SparklrWP
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            App.Client = new SparklrClient();
            if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
            {
                string username = "";
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("username", out username);
                usernameBox.Text = username;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                byte[] passbyte = null;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue("password", out passbyte);
                passbyte = ProtectedData.Unprotect(passbyte, null);
                passwordBox.Password = Encoding.UTF8.GetString(passbyte, 0, passbyte.Length);
                rememberBox.IsChecked = true;
            }
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
            GlobalLoading.Instance.IsLoading = true;
            App.Client.LoggedIn += Client_LoggedIn;
            App.Client.Login(usernameBox.Text, passwordBox.Password);
        }

        void Client_LoggedIn(object sender, SparklrClient.LoggedInEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                GlobalLoading.Instance.IsLoading = false;
                if (e.Error)
                {
                    MessageBox.Show("Error");
                }
                else
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("username");
                    }
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("password");
                    }
                    if (rememberBox.IsChecked == true)
                    {
                        IsolatedStorageSettings.ApplicationSettings.Add("password", ProtectedData.Protect(Encoding.UTF8.GetBytes(passwordBox.Password), null));
                        IsolatedStorageSettings.ApplicationSettings.Add("username", usernameBox.Text);
                    }
                    IsolatedStorageSettings.ApplicationSettings.Save();
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
            MessageBox.Show("Sparklr Branding (C) Jonathan Warner \n\n Application Development Team: Marocco2 (design!), jessenic (code!), EaterOfCorpses (code-design!) And TheInterframe (code-design!)\n\n Big Thanks to Jonathan!", "About Sparklr WP V1.0", MessageBoxButton.OK);
        }
    }
}