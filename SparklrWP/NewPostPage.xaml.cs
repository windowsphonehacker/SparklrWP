using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SparklrWP
{
    public partial class NewPostPage : PhoneApplicationPage
    {
        public NewPostPage()
        {
            InitializeComponent();
        }

        private void postButton_Click(object sender, EventArgs e)
        {
            App.Client.BeginRequest((string str) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (str.ToLower().Contains("error"))
                    {
                        MessageBox.Show(str, "Error", MessageBoxButton.OK);
                    }
                    else
                    {
                        if (NavigationService.CanGoBack)
                        {
                            NavigationService.GoBack();
                        }
                        else
                        {
                            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                        }
                    }
                });
                return true;
            }, "work/post", "{\"body\":\""+messageBox.Text+"\"}"); //TODO: use JSON.NET for this
        }

        private void attachButton_Click(object sender, EventArgs e)
        {

        }
    }
}