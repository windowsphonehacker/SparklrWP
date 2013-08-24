using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.IO;

namespace SparklrWP
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loginBrowser.Navigate(new Uri("https://sparklr.me/#/signin"), new byte[] { }, "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)");
        }
        string Cookies = "";
        string logintoken = "";

        private void loginBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            //MessageBox.Show("Navigated to " + loginBrowser.Source.ToString());
            //GlobalLoading.Instance.IsLoading = false;
            if (loginBrowser.Source.Host == "sparklr.me" /* && and needs more conditions */)
            {
                //MessageBox.Show("FEATURE LOGIN");
                var cc = WebBrowserExtensions.GetCookies(loginBrowser);
                //hideBrowser(true);
                //loginBrowser.Navigating -= loginBrowser_Navigating;
                //loginBrowser.Navigated -= loginBrowser_Navigated;
                //loginBrowser.Navigate(new Uri("about:blank"));

                StringBuilder sb = new StringBuilder();
                foreach (Cookie c in cc)
                {
                    sb.Append(c.ToString());
                    sb.Append("; ");
                    if (c.Name.Equals("D"))
                    {
                        logintoken = c.Value.Split(',')[1];
                    }
                }
                Cookies = sb.ToString();
            }
            else
            {
                //hideBrowser(false);
            }
        }

        private void loginBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            //GlobalLoading.Instance.IsLoading = true;
        }
        private void requestTest(string url, string data)
        {
            // Create a HttpWebRequest.
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.Headers["User-Agent"] = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0";
            request.Headers["Referer"] = loginBrowser.Source.ToString();
            request.Headers["Cookie"] = Cookies;
            request.Headers["X-Data"] = data;
            request.Headers["X-X"] = logintoken;
            // Send the request.
            request.BeginGetResponse(new AsyncCallback(ReadCallback), request);
        }
        private void ReadCallback(IAsyncResult ar)
        {
            WebResponse response;
            try
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                response = request.EndGetResponse(ar);
            }
            catch (WebException ex)
            {
                response = ex.Response;
            }
            catch (Exception)
            {
                return;
            }
            string str = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                str = sr.ReadToEnd();
                this.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(str);
                });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Would you like to post a test message?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                requestTest("http://sparklr.me/work/post", "{\"body\":\"Testing from SparklrWP\"}");
            }
        }

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Ani1.Begin();

        }

        private void Signup_click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Go to Sparklr.me On Your Browser to Sign Up", "SparklrWP", MessageBoxButton.OK);

        }

        private void about_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Sparklr Branding (C) Jonathan Warner \n\n Application Development Team: Marocco2, jessenic, EaterOfCorpses And TheInterframe\n\n Big Thanks to Jonathan!", "About Sparklr WP V1.0", MessageBoxButton.OK);

        }
    }
}