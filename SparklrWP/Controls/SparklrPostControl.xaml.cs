using SparklrWP.Utils;
using SparklrWP.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
namespace SparklrWP.Controls
{
    public sealed partial class SparklrPostControl : UserControl, IDisposable
    {
        public static DependencyProperty PostProperty = DependencyProperty.Register("Post", typeof(PostItemViewModel), typeof(SparklrPostControl), new PropertyMetadata(new PropertyChangedCallback(PostPropertyChanged)));

        /// <summary>
        /// Overrides the load animated gif setting
        /// </summary>
        public bool ForceGIFLoading
        {
            get
            {
                return mainControl.ForceGIFLoading;
            }
            set
            {
                mainControl.ForceGIFLoading = value;
            }
        }

        private static void PostPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrPostControl control = d as SparklrPostControl;
            if (e.NewValue is PostItemViewModel) control.Post = (PostItemViewModel)e.NewValue;
        }

        public SparklrPostControl()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            if (Post != null)
                Post.PropertyChanged -= Post_PropertyChanged;
        }

        public PostItemViewModel _post;
        public PostItemViewModel Post
        {
            get
            {
                return _post;
            }
            set
            {
                if (_post != value)
                {
                    if (Post != null)
                        Post.PropertyChanged -= Post_PropertyChanged;

                    _post = value;
                    Post.PropertyChanged += Post_PropertyChanged;
                    mainControl.Text = value.Message;
                    mainControl.ImageLocation = value.ImageUrl;
                    notesText.Text = value.CommentCount.ToString();
                    textNetwork.Text = "> " + NetworkHelpers.FormatNetworkName(value.Network);
                    //TODO: Rewrite to not block the UI and use caching (use extendedimagecontrol)
                    imageProfile.Source = new BitmapImage(new Uri("http://d.sparklr.me/i/t" + value.AuthorId + ".jpg"));
                    textName.Text = "@" + value.AuthorName;
                }
            }
        }

        void Post_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Message":
                    mainControl.Text = Post.Message;
                    break;

                case "ImageUrl":
                    mainControl.ImageLocation = Post.ImageUrl;
                    break;

                case "CommentCount":
                    notesText.Text = Post.CommentCount.ToString();
                    break;

                case "Network":
                    textNetwork.Text = "> " + NetworkHelpers.FormatNetworkName(Post.Network);
                    break;

                case "AuthorName":
                    textName.Text = "@" + Post.AuthorName;
                    break;
            }
        }

        private void Like_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }
    }
}
