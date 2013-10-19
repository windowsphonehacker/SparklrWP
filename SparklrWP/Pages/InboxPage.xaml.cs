using Microsoft.Phone.Controls;
using SparklrWP.Controls;
using SparklrWP.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class InboxPage : PhoneApplicationPage
    {
        InboxViewModel model;
        public InboxPage()
        {
            InitializeComponent();
            model = new InboxViewModel();
            model.Load();
            this.DataContext = model;
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ConversationModel m = ((Grid)sender).DataContext as ConversationModel;

            if (m != null)
                NavigationService.Navigate(new Uri("/Pages/ChatPage.xaml?id=" + m.From.ToString(), UriKind.Relative));
        }

        private void RefreshIconButton_Click(object sender, System.EventArgs e)
        {
            model.Load();
        }

        #region Notification
        bool popupVisible = false;

        private void Notification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SparklrTextBlock control = sender as SparklrTextBlock;

            if (control != null)
            {
                NotificationViewModel m = (NotificationViewModel)control.DataContext;
                if (m.NavigationUri != null)
                    NavigationService.Navigate(m.NavigationUri);
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (this.NavigationContext.QueryString.ContainsKey("notification") && e.NavigationMode == NavigationMode.New)
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
        #endregion
    }
}