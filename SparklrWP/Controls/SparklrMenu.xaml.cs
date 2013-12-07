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
            
        }
        /*
       bool popupVisible = false;
         private void Notification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SparklrTextBlock control = sender as SparklrTextBlock;

            if (control != null)
            {
                NotificationViewModel m = (NotificationViewModel)control.DataContext;
                if (m.NavigationUri != null)
                    (Application.Current.RootVisual as PhoneApplicationFrame).NavigationService.Navigate(m.NavigationUri);
            }
        }
         protected override void OnNavigatedTo(NavigationEventArgs e)
         {
             base.(Application.Current.RootVisual as PhoneApplicationFrame).OnNavigatedTo(e);
             if (this.(Application.Current.RootVisual as PhoneApplicationFrame).NavigationContext.QueryString.ContainsKey("notification") && e.NavigationMode == NavigationMode.New)
             {
                 BorderNotification_Tap(this, new System.Windows.Input.GestureEventArgs());
             }
            
         }
             protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (popupVisible)
            {
                NotificationDisappear.Begin();
                popupVisible = false;
                e.Cancel = true;
            }
        }
        private void BorderNotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!popupVisible)
            {
                
                NotificationAppear.Begin();
                popupVisible = true;
            }
        }
          */
    }

}
