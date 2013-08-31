using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using SparklrLib;
using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Beacon;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows.Media;
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
            //TODO: Add code to perform your task in background
            if (IsolatedStorageSettings.ApplicationSettings.Contains("username") && IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                SparklrClient client = new SparklrClient();
                string username = IsolatedStorageSettings.ApplicationSettings["username"].ToString();
                byte[] passbyts = ProtectedData.Unprotect((byte[])IsolatedStorageSettings.ApplicationSettings["password"], null);
                string password = Encoding.UTF8.GetString(passbyts, 0, passbyts.Length);

                Func<Notification, String> textGenerator = (not) =>
                {
                    if (not.type == 1)
                    {
                        if (not.body == "☝")
                        {
                            return "{0} likes this post.";
                        }
                        else
                        {
                            return "{0} commented " + not.body + ".";
                        }
                    }
                    else if(not.type == 2)
                    {
                        return "{0} mentioned you.";
                    }
                    else if (not.type == 3)
                    {
                        return "{0} messaged you.";
                    }
                    else
                    {
                        return "{0} did something to you.";
                    }
                };

                LoginEventArgs loginArgs = await client.LoginAsync(username, password);
                if (loginArgs.IsSuccessful)
                {
                    JSONRequestEventArgs<Stream> args = await client.GetBeaconStreamAsync(0, 1);
                    if (args.IsSuccessful)
                    {
                        Stream strm = args.Object;

#if DEBUG
                        if (strm.notifications == null)
                            strm.notifications = new List<Notification>();
                        strm.notifications.Add(new Notification()
                       {
                           from = 4,
                           type = 1,
                           body = "ILY"
                       });
#endif
                        if (strm.notifications != null && strm.notifications.Count > 0)
                        {

                            List<int> userIds = new List<int>();
                            foreach (Notification not in strm.notifications)
                            {
                                if (!userIds.Contains(not.from))
                                {
                                    userIds.Add(not.from);
                                }
                            }
                            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> unargs = await client.GetUsernamesAsync(userIds.ToArray()); if (unargs.IsSuccessful)
                            {
                                if (unargs.IsSuccessful)
                                {

                                    foreach (ShellTile til in ShellTile.ActiveTiles)
                                    {
                                        Mangopollo.Tiles.FlipTileData data = new Mangopollo.Tiles.FlipTileData();
                                        data.Title = "Sparklr*";
                                        System.Net.WebClient wc = new System.Net.WebClient();
                                        IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                                        if (storage.FileExists("/Shared/ShellContent/tile.jpg"))
                                        {
                                            storage.DeleteFile("/Shared/ShellContent/tile.jpg");
                                        }
                                        System.IO.Stream ily = await wc.OpenReadTaskAsync(new Uri("http://d.sparklr.me/i/" + strm.notifications[0].from + ".jpg"));
                                        await Deployment.Current.Dispatcher.InvokeAsync(() => {
                                            BitmapImage img = new BitmapImage();
                                            try{

                                                img.SetSource(ily);
                                            ily.Dispose();


                                            WriteableBitmap f = new WriteableBitmap(336,336);
                                            Tile l = new Tile();
                                            l.img.Source = img;
                                            l.textBlock.Text = Microsoft.Phone.Info.DeviceStatus.ApplicationMemoryUsageLimit + "/" + (Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage-Microsoft.Phone.Info.DeviceStatus.ApplicationMemoryUsageLimit).ToString();
                                            l.Measure(new Size(336, 336));
                                            l.Arrange(new Rect(0, 0, 336, 336));
                                            f.Render(l, null);
                                            f.Invalidate();
                                            using (IsolatedStorageFileStream str = storage.CreateFile("/Shared/ShellContent/tile.jpg"))
                                            {
                                                f.SaveJpeg(str, 336, 336, 0, 100);
                                                str.Close();
                                            }
                                            data.Title = "Sparklr*";
                                            data.BackgroundImage = new Uri("/Background.png", UriKind.Relative);
                                            data.BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/tile.jpg", UriKind.Absolute);
                                            til.Update(data);
                                            }
                                            catch (Exception e)
                                            {
                                                e.ToString();
                                            }
                                        });
                                    }

                                    foreach (Notification not in strm.notifications)
                                    {
                                        ShellToast notif = new ShellToast();
                                        notif.Title = "Sparklr*";
                                        notif.Content = String.Format(textGenerator(not), client.Usernames[not.from]);
                                        notif.Show();
                                    }
                                }
                                else
                                {
                                    foreach (Notification not in strm.notifications)
                                    {
                                        ShellToast notif = new ShellToast();
                                        notif.Title = "Sparklr*";
                                        notif.Content = String.Format(textGenerator(not), "Someone");
                                        notif.Show();
                                    }
                                }
                                NotifyComplete();
                            }
                            else
                            {
                                foreach (Notification not in strm.notifications)
                                {
                                    ShellToast notif = new ShellToast();
                                    notif.Title = "Sparklr*";
                                    notif.Content = String.Format(textGenerator(not), "Someone");
                                    notif.Show();
                                }
                            }
                            NotifyComplete();
                        }
                        else
                        {
                            NotifyComplete();
                        }
                    }
                }
                else
                {
                    NotifyComplete();
                }
            }
            else
            {
                NotifyComplete();
            }
        }
    }
}