using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;
using System;

namespace SparklrWP.Pages
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void TextBlock_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            new AudioTrack(new Uri("easteregg.wma", UriKind.Relative),
                    "Easter Egg :)",
                    "Easter Egg :)",
                    "Easter Egg :)",
                    null);

            BackgroundAudioPlayer.Instance.Play();
        }
    }
}