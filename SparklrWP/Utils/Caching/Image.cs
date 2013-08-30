extern alias ImageToolsDLL;
using ImageToolsDLL::ImageTools;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SparklrWP.Utils.Caching
{
    /// <summary>
    /// Provides methods to load cached images
    /// </summary>
    public static class Image
    {
        /// <summary>
        /// Contains the location where cached images are stored
        /// </summary>
        public const String CacheFolder = @"Cache\Images";

        private static int maximumCacheSizeMB = 50;
        /// <summary>
        /// Specifies the maximum cache size in MB
        /// </summary>
        public static int MaximumCacheSizeMB
        {
            get
            {
                return maximumCacheSizeMB;
            }
            set
            {
                if (maximumCacheSizeMB != value)
                    maximumCacheSizeMB = value;
            }
        }

        private static TimeSpan cacheTimeSpan = new TimeSpan(7, 0, 0, 0);
        /// <summary>
        /// Specifies how long images are stored in the cache
        /// </summary>
        public static TimeSpan CacheTimeSpan
        {
            get
            {
                return cacheTimeSpan;
            }
            set
            {
                if (cacheTimeSpan != value)
                    cacheTimeSpan = value;
            }
        }

        /// <summary>
        /// Initializes the image cache
        /// </summary>
        static Image()
        {
            ImageToolsHelper.InitializeImageTools();

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
#if DEBUG
                //Set to false to keep the files in the cache
                if (true)
                {
                    ClearImageCache();
                    App.logger.log("Deleted cache content because DEBUG flag was set");
                }
#endif

                //Make sure that the directory exists
                if (!storage.DirectoryExists(CacheFolder))
                {
                    storage.CreateDirectory(CacheFolder);
#if DEBUG
                    App.logger.log("Created folder {0}", CacheFolder);
#endif
                }
#if DEBUG
                App.logger.log("Isolated storage is using {0} of {1} bytes", storage.AvailableFreeSpace, storage.Quota);
#endif
            }
        }

        /// <summary>
        /// Removes all files from the cache folder
        /// </summary>
        public static void ClearImageCache()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists(CacheFolder))
                {
                    //TODO?: Refractor to GetStorageFiles
                    string filter = Path.Combine(CacheFolder, "*");
                    foreach (string file in storage.GetFileNames(filter))
                    {
                        string fileName = Path.Combine(CacheFolder, file);
                        storage.DeleteFile(fileName);
#if DEBUG
                        App.logger.log("Deleted cache file {0}", fileName);
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Enforces the limits specified by the user
        /// </summary>
        /// <returns>The number of files deleted</returns>
        public static int CleanImageCache()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.DirectoryExists(CacheFolder))
                {
                    string filter = Path.Combine(CacheFolder, "*");

                    //First pass: clean the expired files
                    foreach (string file in storage.GetFileNames(filter))
                    {
                        string fileName = Path.Combine(CacheFolder, file);

                        if (storage.GetLastWriteTime(fileName) < DateTime.Now.Subtract(cacheTimeSpan))
                        {
                            storage.DeleteFile(fileName);
#if DEBUG
                            App.logger.log("Deleted {0} from cache because it expired.", fileName);
#endif
                        }
                    }

                    //Second pass: delete files until the size is back to the maximum size specified
                    string[] filenames = storage.GetFileNames(filter);

                    for (int i = 0; i < filenames.Length && (storage.Quota - storage.AvailableFreeSpace).ConvertBytesToMegabytes() > MaximumCacheSizeMB; i++)
                    {
                        storage.DeleteFile(filenames[i]);
#if DEBUG
                        App.logger.log("Deleted {0} from cache because we exceed our specified capacity.", filenames[i]);
#endif
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Loads an image asynchronously. If the image is already cached, it will be loaded from the cache. Otherwise it will be added to the cache
        /// </summary>
        /// <param name="url">An absolute URI pointing to the image</param>
        /// <returns>A Bitmap with the image</returns>
        public static Task<ExtendedImage> LoadCachedImageFromUrlAsync(String url)
        {
            return LoadCachedImageFromUrlAsync(new Uri(url));
        }

        /// <summary>
        /// Loads an image asynchronously. If the image is already cached, it will be loaded from the cache. Otherwise it will be added to the cache
        /// </summary>
        /// <param name="url">An absolute URI pointing to the image</param>
        /// <returns>A Bitmap with the image</returns>
        public async static Task<ExtendedImage> LoadCachedImageFromUrlAsync(Uri url)
        {
            try
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string file = Path.Combine(CacheFolder, getCachenameFromUri(url));

                    if (cacheContainsUri(url))
                    {
                        ExtendedImage cachedImage = new ExtendedImage();
                        using (IsolatedStorageFileStream cachedFile = storage.OpenFile(file, FileMode.Open, FileAccess.Read))
                        {
                            BitmapImage image = new BitmapImage();
                            image.SetSource(cachedFile);

                            WriteableBitmap tmp = new WriteableBitmap(image);
                            cachedImage = tmp.ToImage();

#if DEBUG
                            App.logger.log("Loaded image {0} from cached file {1}", url, file);
#endif
                            return cachedImage;
                        }
                    }
                    else
                    {
                        ExtendedImage loadedImage = await Helpers.LoadImageFromUrlAsync(url);

                        //GIF files don't support saving with imagetools
                        if (!url.ToString().EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
                            saveImageToCache(loadedImage, file, storage);

                        return loadedImage;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
#if DEBUG
                    App.logger.log("Error loading from cache: {0}", url);
#endif
                }
#if DEBUG
                throw;
#endif
                return null;
            }
        }

        private static void saveImageToCache(ExtendedImage image, string filename, IsolatedStorageFile storage)
        {
            try
            {
                using (IsolatedStorageFileStream cachedFile = storage.OpenFile(filename, FileMode.Create))
                {
                    WriteableBitmap bitmap = ImageExtensions.ToBitmap(image);
                    bitmap.SaveJpeg(cachedFile, bitmap.PixelWidth, bitmap.PixelHeight, 0, 80);

#if DEBUG
                    App.logger.log("Created cached file {0}", filename);
#endif
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        /// <summary>
        /// Checks if the image exists in the cache
        /// </summary>
        /// <param name="location">An Uri pointing to an image</param>
        /// <returns>True if the image is in the cache folder, otherwise false</returns>
        private static bool cacheContainsUri(Uri location)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string targetFile = Path.Combine(CacheFolder, getCachenameFromUri(location));
                return storage.FileExists(targetFile);
            }
        }

        /// <summary>
        /// Generates an cache name for an image
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A string with a unique name for every URI</returns>
        private static string getCachenameFromUri(Uri location)
        {
            return String.Format("{0}", location.ToString().EncodeUrl());
        }
    }
}
