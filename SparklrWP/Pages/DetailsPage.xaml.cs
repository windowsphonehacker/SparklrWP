using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SparklrWP.Controls;
using SparklrWP.Utils;
using SparklrWP.ViewModels;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
            if (CommentButton == null)
            {
                CommentButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            }
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (this.NavigationContext.QueryString.ContainsKey("notification") && e.NavigationMode == NavigationMode.New)
            {
                BorderNotification_Tap(this, new System.Windows.Input.GestureEventArgs());
            }
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                DataContext = new PostViewModel(App.MainViewModel.Items[index]);
            }
            else if (NavigationContext.QueryString.TryGetValue("id", out selectedIndex))
            {
                int id = -1;
                if (Int32.TryParse(selectedIndex, out id))
                {
                    DataContext = new PostViewModel(new PostItemViewModel(id));
                }
                else
                {
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
#endif
                }
            }
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


        private void refreshComments()
        {
            (DataContext as PostViewModel).RefreshComments();
        }

        private async void HeartClick(object sender, System.EventArgs e)
        {
            PostViewModel p = this.DataContext as PostViewModel;
            if (!p.Liked)
            {
                GlobalLoading.Instance.IsLoading = true;
                SparklrLib.Objects.JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> response = await App.Client.LikePostAsync(p.MainPost.AuthorId, p.MainPost.Id);
                GlobalLoading.Instance.IsLoading = false;

                if (response == null || !response.IsSuccessful)
                    MessageBox.Show("We were unable to like this post. Please try again later...", "Oops...", MessageBoxButton.OK);
                else
                    refreshComments();
            }
            else
            {
                GlobalLoading.Instance.IsLoading = true;
                SparklrLib.Objects.JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> response = await App.Client.DeleteCommentAsync(p.LikeID);
                GlobalLoading.Instance.IsLoading = false;

                if (response == null || !response.IsSuccessful)
                    MessageBox.Show("We were unable unlike this post. Please try again later...", "Oops...", MessageBoxButton.OK);
                else
                    refreshComments();
            }
        }

        private void CommentTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PostComment();
            }
        }

        private async void PostComment()
        {
            CommentButton.IsEnabled = false;
            Focus();
            PostViewModel p = this.DataContext as PostViewModel;
            GlobalLoading.Instance.IsLoading = true;
            SparklrLib.Objects.JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> response = await App.Client.PostCommentAsync(p.MainPost.AuthorId, p.MainPost.Id, CommentTextbox.Text);
            GlobalLoading.Instance.IsLoading = false;

            if (response == null || !response.IsSuccessful)
            {
                MessageBox.Show("We were unable to post your comment. Please try again later...", "Oops...", MessageBoxButton.OK);
                CommentButton.IsEnabled = true;
            }
            else
            {
                refreshComments();
                CommentTextbox.Text = "";
            }
        }

        private async void SparklrText_Delete(object sender, System.EventArgs e)
        {
            PostItemViewModel item = (sender as System.Windows.Controls.UserControl).DataContext as PostItemViewModel;
            if (item != null)
            {
                GlobalLoading.Instance.IsLoading = true;
                SparklrLib.Objects.JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> response = await App.Client.DeleteCommentAsync(item.Id);
                GlobalLoading.Instance.IsLoading = false;

                if (response == null || !response.IsSuccessful)
                    MessageBox.Show("We were unable to delete this comment. Please try again later.", "Oops...", MessageBoxButton.OK);
                else
                    refreshComments();
            }
        }

        private void RepostButton_Click(object sender, System.EventArgs e)
        {
            Coding4Fun.Toolkit.Controls.InputPrompt prompt = new Coding4Fun.Toolkit.Controls.InputPrompt();
            prompt.Title = "Repost";
            prompt.Message = "If you want you can add a message to the repost.";
            prompt.Completed += prompt_Completed;
            prompt.Show();
        }

        private async void prompt_Completed(object sender, Coding4Fun.Toolkit.Controls.PopUpEventArgs<string, Coding4Fun.Toolkit.Controls.PopUpResult> e)
        {
            if (e.PopUpResult == Coding4Fun.Toolkit.Controls.PopUpResult.Ok)
            {
                PostViewModel p = this.DataContext as PostViewModel;
                SparklrLib.Objects.JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> response = await App.Client.Repost(p.MainPost.Id, e.Result ?? "");
                if (response.IsSuccessful)
                {
                    Helpers.Notify("Success!", "The post has been reposted.");
                }
                else
                {
                    MessageBox.Show("Something went wrong...", "...and we were unable to repost. Please try again later.", MessageBoxButton.OK);
                }
            }
        }

        private void CommentButton_Click(object sender, System.EventArgs e)
        {
            PostComment();
        }

        private void CommentTextbox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CommentButton.IsEnabled = CommentTextbox.Text.Length > 0;
        }
    }
}