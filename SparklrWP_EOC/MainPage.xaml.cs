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
        public SparklrLib.SparklrClient Sparklr = new SparklrLib.SparklrClient();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void log(object val)
        {
            Console.Dispatcher.BeginInvoke(() =>
            {
                Console.Items.Add(val.GetType() == typeof(String)?new TextBlock() { Text = (String)val, TextWrapping = TextWrapping.Wrap }:val);
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
                    Sparklr.Login("EaterOfCorpses", "pass", (lea) =>
                    {
                        if (lea.IsSuccessful)
                        {
                            log("Login was succesfull! authToken: " + lea.AuthToken + " UserId: " + lea.UserId);
                            Sparklr.getBeaconStream((eargs) =>
                            {
                                if (eargs.IsSuccessful)
                                {
                                    log("Success! dash items: " + eargs.Object.data.length);
                                    foreach (SparklrLib.Objects.Responses.Beacon.Timeline item in eargs.Object.data.timeline)
                                    {
                                        log(item.message);
                                    }
                                }
                                else
                                {
                                    log("Getting dash failed: " + eargs.Error.Message);
                                }
                            });
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
