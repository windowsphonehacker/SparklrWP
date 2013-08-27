using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class Profile : PhoneApplicationPage
    {
        public Profile()
        {
            InitializeComponent();
        }
        public bool dataLoaded = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (dataLoaded) return;
            dataLoaded = true;
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("userId", out selectedIndex))
            {
                int index = int.Parse(selectedIndex);
                profileBg.ImageSource = new BitmapImage(new Uri("http://d.sparklr.me/i/b" + index + ".jpg"));
                profilePanorama.Tag = new
                {
                    Image = new BitmapImage(new Uri("http://d.sparklr.me/i/" + index + ".jpg")),
                    Handle = "@..."
                };
                App.Client.GetUser(index, (usargs) =>
                {
                    if(usargs.IsSuccessful){
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            profilePanorama.Title = usargs.Object.name;
                            profilePanorama.Tag = profilePanorama.Tag = new
                            {
                                Image = new BitmapImage(new Uri("http://d.sparklr.me/i/" + index + ".jpg")),
                                Handle = "@" + usargs.Object.handle
                            };
                            bio.Text = usargs.Object.bio;
                        });
                    }
                });
            }
        }
    }
}