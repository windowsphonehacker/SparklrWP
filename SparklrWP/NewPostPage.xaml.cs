using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;


namespace SparklrWP
{
    public partial class NewPostPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        Stream PhotoStr;
        public NewPostPage()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask() { ShowCamera = true };
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
           

        }

        private void postButton_Click(object sender, EventArgs e)
        {
            if (messageBox.Text == "" && PhotoStr == null)
            {
                MessageBox.Show("You need to say something! You can't post an empty message!", "Sorry", MessageBoxButton.OK);
            }
            else
            {
                GlobalLoading.Instance.IsLoading = true;
                App.Client.Post(messageBox.Text, PhotoStr, (args) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        GlobalLoading.Instance.IsLoading = false;
                        if (!args.IsSuccessful)
                        {
                            MessageBox.Show("Something horrible happend!\nWe couldn't post your message" + (PhotoStr==null?"":" and photo") + " try again later!", "Sorry", MessageBoxButton.OK);
                        }
                        else
                        {
                            if (NavigationService.CanGoBack)
                            {
                                NavigationService.GoBack();
                            }
                            else
                            {
                                MessageBox.Show("Your status has been posted!", "Yay!", MessageBoxButton.OK);

                                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

                            }
                        }
                    });
                });
            }
        }

        private void attachButton_Click(object sender, EventArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;
            photoChooserTask.Show();
          

        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                PhotoStr = e.ChosenPhoto;

                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                PicThumbnail.Source = bmp;
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        private void PicThumbnail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (MessageBox.Show("Do you want to remove the attached image?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PhotoStr = null;
                PicThumbnail.Source = null;
            }
        }

    }
}