using Microsoft.Phone.Shell;
using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Beacon;
using SparklrLib.Objects.Responses.Work;
using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SparklrWP.Utils
{
    public enum TileSize
    {
        Small,
        Medium,
        Wide
    }

    public static class TilesCreator
    {
        private const string TilesFolder = @"Shared\ShellContent";

        private const string PrimaryWideFilename = "PrimaryWide.jpg";
        private const string PrimaryMediumFilename = "PrimaryMedium.jpg";

        private const int SmallWidth = 156;
        private const int SmallHeight = 156;
        private const int MediumWidth = 336;
        private const int MediumHeight = 336;
        private const int WideWidth = 691;
        private const int WideHeight = 336;

        static TilesCreator()
        {
#if DEBUG
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists(TilesFolder))
                {
                    string filter = System.IO.Path.Combine(TilesFolder, "*");
                    foreach (string file in storage.GetFileNames(filter))
                    {
                        string fileName = System.IO.Path.Combine(TilesFolder, file);
                        storage.DeleteFile(fileName);
                        Globals.log("Deleted tile file {0}", fileName);
                    }
                }
            }
#endif
        }

        public static async Task<bool> PinUserprofile(int userid, SparklrLib.SparklrClient client)
        {
            JSONRequestEventArgs<User> userdata = await client.GetUserAsync(userid);

            if (userdata.IsSuccessful)
            {
                if (await CreateProfileTileImages(userid))
                {
                    ShellTileData data = Mangopollo.Tiles.TilesCreator.CreateFlipTile(
                                            userdata.Object.name,
                                            "",
                                            "",
                                            "",
                                            null,
                                            new Uri("/Assets/TileBackgrounds/Small.png", UriKind.Relative),
                                            new Uri("/Assets/TileBackgrounds/Medium.png", UriKind.Relative),
                                            new Uri("isostore:/" + GetUserprofileTileLocation(userid, TileSize.Medium).Replace(@"\", @"/"), UriKind.Absolute),
                                            new Uri("/Assets/TileBackgrounds/Wide.png", UriKind.Relative),
                                            new Uri("isostore:/" + GetUserprofileTileLocation(userid, TileSize.Wide).Replace(@"\", @"/"), UriKind.Absolute)
                                            );

                    ShellTileExtensions.CreateWideTile(new Uri("/Pages/Profile.xaml?userId=" + userid.ToString(), UriKind.Relative), data);
                    return true;
                }
            }
            return false;
        }

        public async static void UpdatePrimaryTile(bool updateImage, SparklrLib.SparklrClient client)
        {
            if (client.IsLoggedIn)
            {
                foreach (ShellTile tile in ShellTile.ActiveTiles)
                {
                    if (tile.NavigationUri.ToString() == "/")
                    {
                        if (updateImage)
                        {
                            string backgroundImage = "http://d.sparklr.me/i/" + client.UserId.ToString() + ".jpg";
                            string backgroundImageWide = "http://d.sparklr.me/i/b" + client.UserId.ToString() + ".jpg";

                            Rectangle r = new Rectangle();
                            ImageBrush b = new ImageBrush();
                            b.ImageSource = (BitmapImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(backgroundImage);
                            b.Opacity = 0.7;
                            b.Stretch = Stretch.UniformToFill;
                            r.Fill = b;

                            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                            {
                                string wideName = System.IO.Path.Combine(TilesFolder, PrimaryWideFilename);
                                string mediumName = System.IO.Path.Combine(TilesFolder, PrimaryMediumFilename);

                                if (isoStore.FileExists(wideName))
                                    isoStore.DeleteFile(wideName);

                                if (isoStore.FileExists(mediumName))
                                    isoStore.DeleteFile(mediumName);

                                saveTileImage(isoStore, mediumName, r, MediumWidth, MediumHeight);
                                b.ImageSource = (BitmapImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(backgroundImageWide);
                                saveTileImage(isoStore, wideName, r, WideWidth, WideHeight);
                            }
                        }

                        string notificationText = "";
                        int? count = null;

                        JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> args = await client.GetBeaconStreamAsync(0, 1);
                        if (args.IsSuccessful && args.Object.notifications != null && args.Object.notifications.Length > 0)
                        {
                            Notification not = args.Object.notifications[0];
                            notificationText = await SparklrWP.Utils.NotificationHelpers.Format(not.type, not.body, not.from, client);
                            count = args.Object.notifications.Length;
                        }

                        ShellTileData data = Mangopollo.Tiles.TilesCreator.CreateFlipTile(
                            "",
                            "",
                            notificationText,
                            notificationText,
                            count,
                            new Uri("/Assets/TileBackgrounds/Small.png", UriKind.Relative),
                            new Uri("/Assets/TileBackgrounds/Medium.png", UriKind.Relative),
                            new Uri("isostore:/" + System.IO.Path.Combine(TilesFolder, PrimaryMediumFilename).Replace(@"\", @"/"), UriKind.Absolute),
                            new Uri("/Assets/TileBackgrounds/WidePrimary.png", UriKind.Relative),
                            new Uri("isostore:/" + System.IO.Path.Combine(TilesFolder, PrimaryWideFilename).Replace(@"\", @"/"), UriKind.Absolute)
                        );

                        tile.Update(data);

                        break;
                    }
                }
            }
            else
            {
                throw new NotSupportedException("Client not logged in");
            }
        }

        //TODO: make async
        public async static Task<bool> CreateProfileTileImages(int userId)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists(TilesFolder))
                {
                    isoStore.CreateDirectory(TilesFolder);
                }

                string location = "http://d.sparklr.me/i/b" + userId + ".jpg";

                string smallLocation = GetUserprofileTileLocation(userId, TileSize.Small);
                string mediumLocation = GetUserprofileTileLocation(userId, TileSize.Medium);
                string wideLocation = GetUserprofileTileLocation(userId, TileSize.Wide);

                Rectangle r = new Rectangle();
                ImageBrush image = new ImageBrush();
                image.ImageSource = (BitmapImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(location);
                image.Stretch = Stretch.UniformToFill;
                r.Fill = image;

                try
                {
                    if (!isoStore.FileExists(smallLocation))
                    {
                        saveTileImage(isoStore, smallLocation, r, SmallWidth, SmallHeight);
                    }

                    if (!isoStore.FileExists(mediumLocation))
                    {
                        saveTileImage(isoStore, mediumLocation, r, MediumWidth, MediumHeight);
                    }

                    if (!isoStore.FileExists(wideLocation))
                    {
                        saveTileImage(isoStore, wideLocation, r, WideWidth, WideHeight);
                    }

                    return true;
                }
                catch (System.IO.IOException)
                {
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
#endif

                    return false;
                }
            }
        }

        private static void saveTileImage(IsolatedStorageFile isoStore, string location, Rectangle r, int width, int height)
        {
            r.Width = width;
            r.Height = height;
            r.UpdateLayout();

            using (IsolatedStorageFileStream fs = isoStore.CreateFile(location))
            {
                WriteableBitmap smallTile = new WriteableBitmap(r, null);
                smallTile.SaveJpeg(fs, width, height, 0, 100);

#if DEBUG
                Globals.log("Created tile image {0}", location);
#endif
            }
        }

        public static BitmapImage LoadProfileTileImage(int userid, TileSize size)
        {
            string location = GetUserprofileTileLocation(userid, size);

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(location))
                {
                    BitmapImage ret = new BitmapImage();
                    using (IsolatedStorageFileStream fs = isoStore.OpenFile(location, System.IO.FileMode.Open))
                    {
                        ret.SetSource(fs);
#if DEBUG
                        Globals.log("loaded file {0}", location);
#endif
                    }
                    return ret;
                }
                else
                {
                    return null;
                }
            }
        }


        public static string GetUserprofileTileLocation(int userid, TileSize size)
        {
            switch (size)
            {
                case TileSize.Small:
                    return System.IO.Path.Combine(TilesFolder, String.Format("{0}_{1}.jpg", userid, SmallWidth));
                case TileSize.Medium:
                    return System.IO.Path.Combine(TilesFolder, String.Format("{0}_{1}.jpg", userid, MediumWidth));
                case TileSize.Wide:
                    return System.IO.Path.Combine(TilesFolder, String.Format("{0}_{1}.jpg", userid, WideWidth));
            }

            //TODO: add better exception type
            throw new Exception("size not set properly");
        }
    }
}
