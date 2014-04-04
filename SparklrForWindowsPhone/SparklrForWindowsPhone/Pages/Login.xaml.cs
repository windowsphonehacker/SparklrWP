using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SparklrForWindowsPhone.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private void OnBackKey(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Kills the app
            App.Current.Terminate();
        }
    }
}