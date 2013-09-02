using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Navigation;

namespace SparklrWP
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                DataContext = new PostViewModel(App.MainViewModel.Items[index]);
            }
        }

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

        private async void CommentTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Focus();
                PostViewModel p = this.DataContext as PostViewModel;
                GlobalLoading.Instance.IsLoading = true;
                SparklrLib.Objects.JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> response = await App.Client.PostCommentAsync(p.MainPost.AuthorId, p.MainPost.Id, CommentTextbox.Text);
                GlobalLoading.Instance.IsLoading = false;

                if (response == null || !response.IsSuccessful)
                    MessageBox.Show("We were unable to post your comment. Please try again later...", "Oops...", MessageBoxButton.OK);
                else
                {
                    refreshComments();
                    CommentTextbox.Text = "";
                }
            }
        }

        private async void SparklrText_Delete(object sender, System.EventArgs e)
        {
            ItemViewModel item = (sender as System.Windows.Controls.UserControl).DataContext as ItemViewModel;
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
    }
}