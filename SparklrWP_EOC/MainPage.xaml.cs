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

namespace SparklrWP_EOC
{
    public partial class MainPage : PhoneApplicationPage
    {
        public SparklrLib.SparklrClientEOC Sparklr = new SparklrLib.SparklrClientEOC();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void log(object val)
        {
            Console.Dispatcher.BeginInvoke(() =>
            {
                Console.Items.Add(val);
            });
            
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Sparklr.Login("Nope", "this is wrong!", (le) =>
            {
                if (le.IsSuccessful)
                {
                    log("Login was succesfull! authToken: " + le.AuthToken + " UserId: " + le.UserId);
                }
                else
                {
                    log("Login failed, Error: " + le.Error.Message);
                    Sparklr.Login("EaterOfCorpses", "500Please", (lea) =>
                    {
                        if (lea.IsSuccessful)
                        {
                            log("Login was succesfull! authToken: " + lea.AuthToken + " UserId: " + lea.UserId);
                        }
                        else
                        {
                            log("Login failed, Error: " + lea.Error.Message);
                        }
                    });
                }
            });
        }
    }
}
