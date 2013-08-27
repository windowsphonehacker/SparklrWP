using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using SparklrLib;
using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace SparklrWP
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();

            App.Client = new SparklrClient();
            if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
            {
                string username = "";
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("username", out username);
                usernameBox.Text = username;
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                byte[] passbyte = null;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue("password", out passbyte);
                passbyte = ProtectedData.Unprotect(passbyte, null);
                passwordBox.Password = Encoding.UTF8.GetString(passbyte, 0, passbyte.Length);
                rememberBox.IsChecked = true;
            }
            App.BackgroundTask = new Utils.Task();
        }

        private bool postcallback(string jsonData)
        {
            Dispatcher.BeginInvoke(() => MessageBox.Show(jsonData));
            return true;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;
            App.Client.Login(usernameBox.Text, passwordBox.Password, loginargs => Dispatcher.BeginInvoke(() =>
            {
                GlobalLoading.Instance.IsLoading = false;
                if (!loginargs.IsSuccessful)
                {
                    if (loginargs.Response != null && loginargs.Response.StatusCode == HttpStatusCode.NotFound)
                    {
                        MessageBox.Show("Wrong username or password");
                    }
                    else
                    {
                        MessageBox.Show("Something horrible happend, try again later!", "Sorry", MessageBoxButton.OK);
#if DEBUG
                        MessageBox.Show(loginargs.Error.Message);
#endif
                    }
                }
                else
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("username");
                    }
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("password");
                    }
                    if (rememberBox.IsChecked == true)
                    {
                        IsolatedStorageSettings.ApplicationSettings.Add("password", ProtectedData.Protect(Encoding.UTF8.GetBytes(passwordBox.Password), null));
                        IsolatedStorageSettings.ApplicationSettings.Add("username", usernameBox.Text);
                    }
                    IsolatedStorageSettings.ApplicationSettings.Save();
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }));

        }

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Ani1.Begin();



            button.IsEnabled = false;
            button1.IsEnabled = false;
            button2.IsEnabled = false;



        }

        private void Signup_click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("The Sign Up Page Will Open In Internet Explorer, Is That Ok?", "Sparklr", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                new WebBrowserTask() { Uri = new Uri("http://sparklr.me") }.Show();
            }
        }

        private void about_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MessageBox.Show("Sparklr Branding © Jonathan Warner \n\n Application Development Team: \n\n Marocco2 (design!)\n jessenic (code!)\n EaterOfCorpses (code-design!)\n TheInterframe (code-design!)\n\n Big Thanks to Jonathan!", "About Sparklr WP V1.0", MessageBoxButton.OK);
        }

        private void checkEnter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                loginButton_Click(sender, null);
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

            if (button1.IsEnabled == false)
            {
                e.Cancel = true;
                //Close the PopUp Window
                ani2.Begin();
                button.IsEnabled = true;
                button1.IsEnabled = true;
                button2.IsEnabled = true;
            }
            else
            {
                NavigationService.GoBack();

            }


        }
    }
}