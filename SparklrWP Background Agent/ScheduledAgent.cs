using Microsoft.Phone.Info;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using SparklrLib;
using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Beacon;
using SparklrWP.Utils;
using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
namespace SparklrWP_Background_Agent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// <summary>
        /// Debugs memory usage. Will be ignored in the Release setting.
        /// </summary>
        /// <param name="label"></param>
        [Conditional("DEBUG")]
        protected void DebugOutputMemoryUsage(string label = null)
        {
            var limit = DeviceStatus.ApplicationMemoryUsageLimit;
            var current = DeviceStatus.ApplicationCurrentMemoryUsage;
            var remaining = limit - current;
            var peak = DeviceStatus.ApplicationPeakMemoryUsage;
            var safetyMargin = limit - peak;

            if (label != null)
            {
                Debug.WriteLine(label);
            }
            Debug.WriteLine("Memory limit (bytes): " + limit);
            Debug.WriteLine("Usual limit (bytes): 6291456");
            Debug.WriteLine("Current memory usage: {0} bytes ({1} bytes remaining)", current, remaining);
            Debug.WriteLine("Peak memory usage: {0} bytes ({1} bytes safety margin)", peak, safetyMargin);
        }


        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected async override void OnInvoke(ScheduledTask task)
        {
            if (task is PeriodicTask && IsolatedStorageSettings.ApplicationSettings.Contains("username") && IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                DebugOutputMemoryUsage("Task started");

                SparklrClient client = new SparklrClient();
                string username = IsolatedStorageSettings.ApplicationSettings["username"].ToString();
                byte[] passbyts = ProtectedData.Unprotect((byte[])IsolatedStorageSettings.ApplicationSettings["password"], null);
                string password = Encoding.UTF8.GetString(passbyts, 0, passbyts.Length);

                LoginEventArgs loginArgs = await client.LoginAsync(username, password);
                if (loginArgs.IsSuccessful)
                {
                    loginArgs = null;

                    JSONRequestEventArgs<Stream> args = await client.GetBeaconStreamAsync("0", 0, 1);
                    if (args.IsSuccessful)
                    {
                        Stream strm = args.Object;

#if DEBUG
                        if (strm.notifications == null)
                        {
                            strm.notifications = new Notification[1];
                            strm.notifications[0] = new Notification()
                               {
                                   from = 4,
                                   type = 1,
                                   body = "Debug test"
                               };
                        }
#endif
                        if (strm.notifications != null && strm.notifications.Length > 0)
                        {
                            if (strm.notifications.Length == 1)
                            {
                                //Show notification directly
                                ShellToast notification = new ShellToast();

                                notification.Title = "Sparklr*";
                                notification.Content = await NotificationHelpers.Format(strm.notifications[0].type, strm.notifications[0].body, strm.notifications[0].from, client);
                                notification.NavigationUri = NotificationHelpers.GenerateActionUri(strm.notifications[0]);

                                if (!String.IsNullOrEmpty(notification.Content))
                                    notification.Show();
                            }
                            else
                            {
                                //Show the notification count... Because 100 notifications suck...
                                ShellToast notification = new ShellToast();

                                notification.Title = "Sparklr*";
                                notification.Content = String.Format("You have {0} notifications.", strm.notifications.Length);
                                notification.NavigationUri = new Uri("/Pages/MainPage.xaml?notification=" + strm.notifications[0].id, UriKind.Relative);

                                notification.Show();
                            }
                        }
                    }

                    TilesCreator.UpdateTiles(false, client);
                }

                DebugOutputMemoryUsage("Task completed");
            }
        }


    }
}