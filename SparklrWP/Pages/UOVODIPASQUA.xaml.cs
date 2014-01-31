using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;

namespace SparklrWP.Pages
{
    public partial class UOVODIPASQUA : PhoneApplicationPage
    {
        public UOVODIPASQUA()
        {
            InitializeComponent();
			MediaPlayer.Stop();
            Uri easteregg = new Uri("https://ec-media.soundcloud.com/uergVxEOv0RU.128.mp3");
			Song song = Song.FromUri("easteregg", easteregg);
            MediaPlayer.Play(song);
        }
    }
}
