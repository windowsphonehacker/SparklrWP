using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SparklrLib;
using SparklrWP.Resources;
using SparklrWP.Utils;
using SparklrWP.ViewModels;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Resources;
using Telerik.Windows.Controls;

namespace SparklrWP
{
    public partial class App : Application
    {
        private static MainViewModel mainViewModel = null;
        public static WPClogger logger = new WPClogger(LogLevel.debug);

        public static SparklrClient Client = new SparklrClient();

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        
        {
            // Copy media to isolated storage.
            CopyToIsolatedStorage();
            Globals.LoggingFunction = logger.log;
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

#if DEBUG
            MemoryDiagnosticsHelper.Start(TimeSpan.FromMilliseconds(500), true);
#endif
            Client.CredentialsExpired += Client_CredentialsExpired;
            SparklrClient.NotificationsReceived += SparklrClient_NotificationsReceived;
            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }
        private void CopyToIsolatedStorage()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string[] files = new string[] { "easteregg.wma"};

                foreach (var _fileName in files)
                {
                    if (!storage.FileExists(_fileName))
                    {
                        string _filePath = "Pages/" + _fileName;
                        StreamResourceInfo resource = Application.GetResourceStream(new Uri(_filePath, UriKind.Relative));

                        using (IsolatedStorageFileStream file = storage.CreateFile(_fileName))
                        {
                            int chunkSize = 4096;
                            byte[] bytes = new byte[chunkSize];
                            int byteCount;

                            while ((byteCount = resource.Stream.Read(bytes, 0, chunkSize)) > 0)
                            {
                                file.Write(bytes, 0, byteCount);
                            }
                        }
                    }
                }
            }
        }
        static internal bool SuppressNotifications = false;
        private int previousNotifications = 0;
        async void SparklrClient_NotificationsReceived(object sender, SparklrLib.Objects.NotificationEventArgs e)
        {
            //TODO: fully implement, e.g. navigate to Notifications on tap, show content of notification, etc.
            if (e.Notifications.Length > 0)
            {
                if (e.Notifications.Length != previousNotifications)
                {
                    if (!SuppressNotifications)
                    {
                        if (e.Notifications.Length == 1)
                        {
                            Helpers.Notify(await NotificationHelpers.Format(e.Notifications[0].type, e.Notifications[0].body, e.Notifications[0].from, App.Client));
                        }
                        else
                        {
                            Helpers.Notify(String.Format(AppResources.AppNotificationText, e.Notifications.Length));
                        }
                    }

                    previousNotifications = e.Notifications.Length;
                }

                if (mainViewModel != null)
                {
                    mainViewModel.UpdateNotifications(e.Notifications);
                }
            }
        }

        private void Client_CredentialsExpired(object sender, EventArgs e)
        {
            RootFrame.Dispatcher.BeginInvoke(() =>
            {
                LoginReturnUri = RootFrame.CurrentSource;
                RootFrame.Navigate(new Uri("/Pages/LoginPage.xaml", UriKind.Relative));
            });
        }

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        /// <returns>The MainViewModel object.</returns>
        public static MainViewModel MainViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (mainViewModel == null)
                    mainViewModel = new MainViewModel();

                return mainViewModel;
            }
        }


        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            if (App.logger.hasCriticalLogged())
            {
                if (MessageBox.Show(AppResources.AppCrashReportText, AppResources.AppCrashReportTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    App.logger.emailReport();
                    App.logger.clearEventsFromLog();
                }
                else
                {
                    App.logger.purgeLog();
                }
            }
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Ensure that application state is restored appropriately
            // TODO: Make sure that restoring doesn't screw with the update timer
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (RemoveBackEntryOnNavigate)
            {
                RemoveBackEntryOnNavigate = false;
                RootFrame.Dispatcher.BeginInvoke(() => RootFrame.RemoveBackEntry());
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains("authkey") && !App.Client.IsLoggedIn &&
                  IsolatedStorageSettings.ApplicationSettings.Contains("userid"))
            {
                byte[] authBytes = null;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue("authkey", out authBytes);
                authBytes = ProtectedData.Unprotect(authBytes, null);
                App.Client.ManualLogin(Encoding.UTF8.GetString(authBytes, 0, authBytes.Length),
                    (long)IsolatedStorageSettings.ApplicationSettings["userid"]);
            }
            else if (!e.Uri.ToString().Contains("/Pages/LoginPage.xaml") && !e.Uri.ToString().Contains("/Pages/FirstRunPage.xaml") && !App.Client.IsLoggedIn)
            {
                e.Cancel = true;
                LoginReturnUri = e.Uri;
                RootFrame.Dispatcher.BeginInvoke(() => RootFrame.Navigate(new Uri("/Pages/LoginPage.xaml", UriKind.Relative)));
            }
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                logger.log(LogLevel.critical, e.ExceptionObject);
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RadTransition transition = new RadTransition();
            transition.BackwardInAnimation = this.Resources["SparklrAnimationIn"] as RadFadeAnimation;
            transition.BackwardOutAnimation = this.Resources["SparklrAnimationOut"] as RadFadeAnimation;
            transition.ForwardInAnimation = this.Resources["SparklrAnimationIn"] as RadFadeAnimation;
            transition.ForwardOutAnimation = this.Resources["SparklrAnimationOut"] as RadFadeAnimation;
            transition.PlayMode = TransitionPlayMode.Consecutively;
            RadPhoneApplicationFrame frame = new RadPhoneApplicationFrame();
            frame.Transition = transition;
            RootFrame = frame;
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;
            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        private static string _aviaryApiKey;

        public static string AviaryApiKey
        {
            get
            {
                if (_aviaryApiKey == null)
                {
                    var resourceStream = Application.GetResourceStream(new Uri("aviaryapikey.txt", UriKind.Relative));

                    if (resourceStream != null)
                    {
                        using (Stream myFileStream = resourceStream.Stream)
                        {
                            if (myFileStream.CanRead)
                            {
                                //Will be disposed when the underlying stream is disposed, no using required
                                StreamReader myStreamReader = new StreamReader(myFileStream);
                                //read the content here
                                _aviaryApiKey = myStreamReader.ReadToEnd();
                            }
                        }

                    }

                }
                return _aviaryApiKey;
            }
        }

        public static Uri LoginReturnUri { get; set; }

        public static bool RemoveBackEntryOnNavigate { get; set; }


        /// <summary>
        /// Localize the given IApplicationBar by replacing it's strings with the corresponding values in AppResources.resx
        /// For example <shell:ApplicationBarIconButton Text="SampleText" /> would replace the Text attribute with AppResources.SampleText
        /// </summary>
        /// <param name="appBar">The IApplicationBar</param>
        public static void BuildLocalizedApplicationBar(IApplicationBar appBar)
        {
            if (appBar != null)
            {
                if (appBar.Buttons != null)
                {
                    foreach (ApplicationBarIconButton b in appBar.Buttons)
                    {
                        try
                        {
                            b.Text = AppResources.ResourceManager.GetString(b.Text);
                        }
                        catch
#if DEBUG
 (Exception ex)
                        {
                            App.logger.log("Failed to localize {0}: {1}", b.Text, ex.Message);
                        }
#else
                        {}
#endif
                    }
                }
                if (appBar.MenuItems != null)
                {
                    foreach (ApplicationBarMenuItem m in appBar.MenuItems)
                    {
                        try
                        {
                            m.Text = AppResources.ResourceManager.GetString(m.Text);
                        }
                        catch
#if DEBUG
 (Exception ex)
                        {
                            App.logger.log("Failed to localize {0}: {1}", m.Text, ex.Message);
                        }
#else
                        {}
#endif
                    }
                }
            }
        }
    }
}