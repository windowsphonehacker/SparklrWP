using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using SparklrLib;
using SparklrLib.Objects;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Navigation;


namespace SparklrWP.Pages
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            //#if DEBUG
            App.logger.setParameter(usernameBox.Text.ToString(), "Username");


            //#endif
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (App.logger.hasCriticalLogged())
            {
                if (MessageBox.Show("Looks Like You Had A Crash The Last Time You Used The App. Would You Like To Send A Bug Report?", "Somthings Wrong Here...", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    App.logger.emailReport();
                    App.logger.clearEventsFromLog();
                }
            }

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("firstruncheck"))
            {
                App.logger.log(LogLevel.info, "First Time/First Run");

                NavigationService.Navigate(new Uri("/Pages/FirstRunPage.xaml", UriKind.Relative));
            }
            else
            {
                if (NavigationContext.QueryString.ContainsKey("logout"))
                {
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("authkey"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("authkey");
                    }
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("userid"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("userid");
                    }
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("username");
                    }
                    if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove("password");
                    }
                    IsolatedStorageSettings.ApplicationSettings.Save();
                    App.Client = new SparklrClient();
                    while (NavigationService.BackStack.Any())
                    {
                        NavigationService.RemoveBackEntry();
                    }
                }


                if (IsolatedStorageSettings.ApplicationSettings.Contains("username"))
                {
                    string username = "";
                    IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("username", out username);
                    usernameBox.Text = username;

                    if (IsolatedStorageSettings.ApplicationSettings.Contains("password"))
                    {
                        byte[] passbyte = null;
                        IsolatedStorageSettings.ApplicationSettings.TryGetValue("password", out passbyte);
                        passbyte = ProtectedData.Unprotect(passbyte, null);
                        passwordBox.Password = Encoding.UTF8.GetString(passbyte, 0, passbyte.Length);
                        rememberBox.IsChecked = true;
                    }
                }
                CheckLoginButtonEnabled();
            }
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loadingOverlay.StartLoading();
            GlobalLoading.Instance.IsLoading = true;
            LoginEventArgs loginargs = await App.Client.LoginAsync(usernameBox.Text, passwordBox.Password);

            loadingOverlay.FinishLoading();
            GlobalLoading.Instance.IsLoading = false;
            if (!loginargs.IsSuccessful)
            {
                if (loginargs.Response != null && loginargs.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    App.logger.log(LogLevel.warn, "Wrong Info Entered");
                    MessageBox.Show("Wrong username or password");
                }
                else
                {
                    App.logger.log(LogLevel.error, "Unknown Error While Logging In");
                    App.logger.log(loginargs.Error);

                    MessageBox.Show("Something horrible happend, try again later!", "Sorry", MessageBoxButton.OK);
#if DEBUG
                    MessageBox.Show(loginargs.Error.Message);


#endif
                }
            }
            else
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("authkey"))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("authkey");
                }
                if (IsolatedStorageSettings.ApplicationSettings.Contains("userid"))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("userid");
                }
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
                    IsolatedStorageSettings.ApplicationSettings.Add("authkey", ProtectedData.Protect(Encoding.UTF8.GetBytes(loginargs.AuthToken), null));
                    IsolatedStorageSettings.ApplicationSettings.Add("userid", loginargs.UserId);
                    IsolatedStorageSettings.ApplicationSettings.Add("username", usernameBox.Text);
                }
                IsolatedStorageSettings.ApplicationSettings.Save();
                if (App.LoginReturnUri != null)
                {
                    App.RemoveBackEntryOnNavigate = true;
                    NavigationService.Navigate(App.LoginReturnUri);
                    App.LoginReturnUri = null;
                }
                else if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    App.RemoveBackEntryOnNavigate = true;
                    NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
                }
            }
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
            if (MessageBox.Show("The sign up page will open in Internet Explorer, is that ok?", "Sparklr*", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                new WebBrowserTask() { Uri = new Uri("http://sparklr.me") }.Show();
            }
        }

        private void about_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
        }

        private void checkEnter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && CheckLoginButtonEnabled())
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
            else if (NavigationService.CanGoBack)
            {
                //We don't want anyone getting pass the login screen without logging in first
                if (MessageBox.Show("Do you want to exit the app? Unsaved changes will be lost.", "Confirmation",
                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    while (NavigationService.BackStack.Any())
                    {
                        NavigationService.RemoveBackEntry();
                    }
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private bool CheckLoginButtonEnabled()
        {
            if (string.IsNullOrEmpty(usernameBox.Text) || string.IsNullOrEmpty(passwordBox.Password))
            {
                loginButton.IsEnabled = false;
                return false;
            }
            else
            {
                loginButton.IsEnabled = true;
                return true;
            }
        }

        private void usernameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckLoginButtonEnabled();
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckLoginButtonEnabled();
        }
    }

}
