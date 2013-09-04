using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
namespace SparklrWP.Controls
{
    public partial class SparklrPostControl : UserControl
    {
        public static DependencyProperty PostProperty = DependencyProperty.Register("Post", typeof(ItemViewModel), typeof(SparklrPostControl),new PropertyMetadata(new PropertyChangedCallback(PostPropertyChanged)));

        private static void PostPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrPostControl control = d as SparklrPostControl;
            if(e.NewValue is ItemViewModel) control.Post = (ItemViewModel)e.NewValue;
        }

        public SparklrPostControl()
        {
            InitializeComponent();
        }



        public ItemViewModel _post;
        public ItemViewModel Post
        {
            get
            {
                return _post;
            }
            set {

                _post = value;
                mainControl.Text = value.Message;
                mainControl.ImageLocation = value.ImageUrl;
                bttnNotes.Content = value.CommentCount;
                textNetwork.Text = "> /" + value.Network;
                imageProfile.Source = new BitmapImage(new Uri("http://d.sparklr.me/i/t" + value.AuthorId + ".jpg"));
                textName.Text = "@" + value.From;
            }
        }
    }
}
