using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.IO;

namespace SparklrWP
{
    public partial class NewPostPage : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        string PhotoStr;
        public NewPostPage()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask() { ShowCamera = true };
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
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
            }, "work/post",
            "{\"body\":\"" + messageBox.Text + "\"" + (PhotoStr == null ? "" : ",\"img\":true") +
#if DEBUG
 ",\"network\":2" + //Development network
#endif
 "}", PhotoStr); //TODO: use JSON.NET for this
        }

        private void attachButton_Click(object sender, EventArgs e)
        {
            photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MessageBox.Show(e.ChosenPhoto.Length.ToString());
                using (MemoryStream ms = new MemoryStream())
                {
                    e.ChosenPhoto.CopyTo(ms);
                    PhotoStr = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
                }

                //Code to display the photo on the page in an image control named myImage.
                //System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                //bmp.SetSource(e.ChosenPhoto);
                //myImage.Source = bmp;
            }
        }
    }
}