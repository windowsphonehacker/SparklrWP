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

namespace SparklrWP
{
    public partial class FirstRun : PhoneApplicationPage
    {
        public FirstRun()
        {
            InitializeComponent();
            HelloAni.Begin();
            
        }

        private void OK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            
        }
    }
}
