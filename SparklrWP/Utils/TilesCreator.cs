using Microsoft.Phone.Shell;
using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using System;
using System.Collections.Generic;
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
                        App.logger.log("Deleted tile file {0}", fileName);
                    }
                }
            }
#endif
        }

        public static async Task<bool> PinUserprofile(int userid)
        {
            JSONRequestEventArgs<User> userdata = await App.Client.GetUserAsync(userid);

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
                App.logger.log("Created tile image {0}", location);
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
                        App.logger.log("loaded file {0}", location);
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
