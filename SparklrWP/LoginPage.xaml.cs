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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Would you like to post a test message?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.Client.BeginRequest(postcallback, "work/post", "{\"body\":\"Testing from SparklrWP\"}");
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
            App.Client.Login(usernameBox.Text, passwordBox.Password);
        }
    }
}