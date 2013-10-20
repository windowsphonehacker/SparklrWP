using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SparklrWP.Controls;
using SparklrWP.Utils;
using SparklrWP.Utils.Extensions;
using SparklrWP.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SparklrWP.Controls
{
    public partial class SparklrMenu : UserControl
    {
        
        public SparklrMenu()
        {
            InitializeComponent();
        }

        private void home_Click(object sender, EventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        private void friends_Click(object sender, RoutedEventArgs e)
        {
            
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/MainPage.xaml?page=1", UriKind.Relative));
        }

        private void inbox_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/InboxPage.xaml", UriKind.Relative));
        }

        private void network_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Pages/MainPage.xaml?page=3", UriKind.Relative));
        }
       
        
    }

}
