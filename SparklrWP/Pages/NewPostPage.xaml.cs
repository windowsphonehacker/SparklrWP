extern alias AviarySDKDLL;
using AviarySDKDLL::AviarySDK;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SparklrLib.Objects;
using SparklrWP.Resources;
using SparklrWP.Utils;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace SparklrWP.Pages
{
    public partial class NewPostPage : PhoneApplicationPage, IDisposable
    {
        readonly PhotoChooserTask _photoChooserTask;
        Stream _photoStr;
        bool setfocus = false;

        public NewPostPage()
        {
            InitializeComponent();
            App.BuildLocalizedApplicationBar(ApplicationBar);
            _photoChooserTask = new PhotoChooserTask() { ShowCamera = true };
            _photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            if (sendButton == null)
            {
                sendButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string content = "";
            string network = "0";

            if (NavigationContext.QueryString.TryGetValue("content", out content))
            {
                messageBox.Text = String.Format("{0} ", HttpUtility.UrlDecode(content));
                setfocus = true;
            }

            if (NavigationContext.QueryString.TryGetValue("network", out network))
            {
                NetworkTextBox.Text = NetworkHelpers.FormatNetworkName(network);
            }
        }

        private async void postButton_Click(object sender, EventArgs e)
        {
            sendButton.IsEnabled = false;
            if (messageBox.Text == "" && _photoStr == null)
            {
                Utils.Helpers.Notify(AppResources.NewPostEmptyMsgWarning);
                App.logger.log(LogLevel.warn, "MessageBox Left Empty");
            }
            else
            {
                GlobalLoading.Instance.IsLoading = true;
                SparklrEventArgs args = await App.Client.PostAsync(messageBox.Text, NetworkHelpers.UnformatNetworkName(NetworkTextBox.Text), _photoStr);
                GlobalLoading.Instance.IsLoading = false;

                if (!args.IsSuccessful)
                {
                    MessageBox.Show(_photoStr == null ? AppResources.NewPostCouldNotPostMsg : AppResources.NewPostCouldNotPostMsgPic, AppResources.NewPostCouldNotPostMsgTitle, MessageBoxButton.OK);
                }
                else
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {
                        Utils.Helpers.Notify(AppResources.NewPostSuccessTitle, AppResources.NewPostSuccessText);

                        NavigationService.GoBack();
                    }
                }
            }
            sendButton.IsEnabled = true;
        }

        private void attachButton_Click(object sender, EventArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;
            if (_photoStr != null)
            {
                if (MessageBox.Show(AppResources.NewPostRemoveImageText, AppResources.NewPostRemoveImageTitle, MessageBoxButton.OKCancel) ==
                    MessageBoxResult.OK)
                {
                    _photoStr.Dispose();
                    _photoStr = null;
                    SetThumbnail(null);

                    AttachPictureHintTextBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _photoChooserTask.Show();
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            GlobalLoading.Instance.IsLoading = true;
            if (e.TaskResult == TaskResult.OK)
            {
                _photoStr = e.ChosenPhoto;

                var bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                SetThumbnail(bmp);

                AttachPictureHintTextBlock.Visibility = Visibility.Collapsed;
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        private void PicThumbnail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var aviaryTask = new AviaryTask(_photoStr);
            aviaryTask.Completed += aviaryTask_Completed;
            aviaryTask.Show();
        }

        void aviaryTask_Completed(object sender, AviaryTaskResultArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;
            if (e.AviaryResult == AviaryResult.OK)
            {
                _photoStr.Dispose();
                _photoStr = new MemoryStream();
                SetThumbnail(e.PhotoResult);

                e.PhotoResult.SaveJpeg(_photoStr, e.PhotoResult.PixelWidth, e.PhotoResult.PixelHeight, 0, 100);
                _photoStr.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                aviaryTask_Error(e.Exception);
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        void SetThumbnail(ImageSource imgSource)
        {
            if (imgSource != null)
            {
                PicThumbnail.Source = imgSource;
                EditBorder.Visibility = Visibility.Visible;
                sendButton.IsEnabled = true;
            }
            else
            {
                PicThumbnail.Source = null;
                EditBorder.Visibility = Visibility.Collapsed;
                sendButton.IsEnabled = messageBox.Text.Length > 0;
            }
        }

        void aviaryTask_Error(Exception ex)
        {
            if (ex == null)
                return;
            MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK);

            if (ex.Message == AviaryError.StreamNull)
            {
                // Input stream can't be null
            }
            else if (ex.Message == AviaryError.FeaturesEmpty)
            {
                // Features list determines which tools are exposed in the Aviary editor and cannot be null or empty
                //
            }
            else if (ex.Message == AviaryError.ImageBig)
            {
                // The image cannot exceed 8 mega pixels
                //
            }
            else if (ex.Message == AviaryError.AdjustmentsEmpty)
            {
                // The adjustment array passed into Photo Genius Apply is not valid.
                // The array must be of 4 float values and the array can't be empty or null
                //
            }
            else
            {
                // This is to handle any error thrown by the system
                //
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Fix for CA1001
            if (_photoStr != null)
                _photoStr.Close();
        }

        private void messageBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            sendButton.IsEnabled = messageBox.Text.Length > 0 || _photoStr != null;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (setfocus)
            {
                messageBox.Select(messageBox.Text.Length, 0);
                messageBox.Focus();
                setfocus = false;
            }
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
                titleContainer.Visibility = Visibility.Collapsed;
            else
                titleContainer.Visibility = Visibility.Visible;
        }
    }
}