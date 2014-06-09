
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace SparklrForWindowsPhone.Pages
{
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
        }

        private void RadAutoComplete_GotFocus(object sender, RoutedEventArgs e)
        {
            this.ApplicationBar.IsVisible = false;
        }

        private void RadAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        {
            this.ApplicationBar.IsVisible = true;
        }
    }
}
