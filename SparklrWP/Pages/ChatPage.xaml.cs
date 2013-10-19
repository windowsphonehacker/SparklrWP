using Microsoft.Phone.Controls;
using SparklrWP.Controls;
using SparklrWP.Utils;
using SparklrWP.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class ChatPage : PhoneApplicationPage
    {
        ChatViewModel model;
        bool dataLoaded = false;

        public ChatPage()
        {
            InitializeComponent();
            loadingOverlay.Visibility = System.Windows.Visibility.Visible;
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


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!dataLoaded)
            {
                string idValue = "";
                if (NavigationContext.QueryString.TryGetValue("id", out idValue))
                {
                    int id = 0;
                    if (Int32.TryParse(idValue, out id))
                    {
                        model = new ChatViewModel()
                        {
                            From = id
                        };
                        model.LoadingFinished += model_LoadingFinished;
                        model.LoadMessages();
                        this.DataContext = model;

                    }
                    else
                    {
                        MessageBox.Show("Something got messed up. Please go back and try again...");
                    }
                }
                else
                {
                    MessageBox.Show("Something got messed up. Please go back and try again...");
                }
            }
            else
            {
                if (loadingOverlay.Visibility == System.Windows.Visibility.Visible)
                    loadingOverlay.FinishLoading();
            }

            if (this.NavigationContext.QueryString.ContainsKey("notification") && e.NavigationMode == NavigationMode.New)
            {
                BorderNotification_Tap(this, new System.Windows.Input.GestureEventArgs());
            }
        }

        void model_LoadingFinished(object sender, EventArgs e)
        {
            if (loadingOverlay.Visibility == System.Windows.Visibility.Visible)
                loadingOverlay.FinishLoading();
        }

        private void ChatBubbleTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                chatBubbleTextBox.UpdateBinding();
                model.sendMessage();
                Focus();
            }
        }
    }
}