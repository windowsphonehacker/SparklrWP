using ImageTools;
using SparklrWP.Utils.Extensions;
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

        private static int maximumCacheSizeMB = 100;
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
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                ImageToolsHelper.InitializeImageTools();

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
#if DEBUG
                    //Set to false to keep the files in the cache
                    if (true)
                    {
                        ClearImageCache();
                        Globals.log("Deleted cache content because DEBUG flag was set");
                    }
#endif

                    //Make sure that the directory exists
                    if (!storage.DirectoryExists(CacheFolder))
                    {
                        storage.CreateDirectory(CacheFolder);
#if DEBUG
                        Globals.log("Created folder {0}", CacheFolder);
#endif
                    }
#if DEBUG
                    Globals.log("Isolated storage is using {0} of {1} bytes", GetCacheFolderSize(), storage.Quota);
#endif
                }
            }
        }

        /// <summary>
        /// Calculates the size of the cache folder
        /// </summary>
        /// <returns>The size in bytes</returns>
        public static long GetCacheFolderSize()
        {
            long total = 0;

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (string fileName in isoStore.GetFileNames(Path.Combine(CacheFolder, "*")))
                {
                    using (var file = isoStore.OpenFile(Path.Combine(CacheFolder, fileName), FileMode.Open))
                    {
                        total += file.Length;
                    }
                }
            }

            return total;
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
                        Globals.log("Deleted cache file {0}", fileName);
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
                            Globals.log("Deleted {0} from cache because it expired.", fileName);
#endif
                        }
                    }

                    //Second pass: delete files until the size is back to the maximum size specified
                    string[] filenames = storage.GetFileNames(filter);

                    for (int i = 0; i < filenames.Length && (storage.Quota - storage.AvailableFreeSpace).ConvertBytesToMegabytes() > MaximumCacheSizeMB; i++)
                    {
                        storage.DeleteFile(filenames[i]);
#if DEBUG
                        Globals.log("Deleted {0} from cache because we exceed our specified capacity.", filenames[i]);
#endif
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Loads an image asynchronously. If the image is already cached, it will be loaded from cache.
        /// </summary>
        /// <typeparam name="T">The type of the image to load. Can be BitmapImage or ExtendedImage</typeparam>
        /// <param name="url">The location if the image</param>
        /// <returns></returns>
        public static Task<object> LoadCachedImageFromUrlAsync<T>(String url)
        {
            try
            {
                return LoadCachedImageFromUrlAsync<T>(new Uri(url));
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
#if DEBUG
                    Globals.log("Error loading from cache: {0}", url);
#endif
                }
#if DEBUG
                throw;
#else
                return null;

#endif
            }
        }

        /// <summary>
        /// Loads an image asynchronously. If the image is already cached, it will be loaded from the cache. Otherwise it will be added to the cache
        /// </summary>
        /// <param name="url">An absolute URI pointing to the image</param>
        /// <typeparam name="T">The type of the image to load. Can be ExtendedImage or BitmapImage</typeparam>
        /// <returns>A Bitmap with the image</returns>
        public async static Task<object> LoadCachedImageFromUrlAsync<T>(Uri url)
        {
            try
            {
                if (typeof(T) == typeof(ExtendedImage))
                {
                    //We want to load a animated GIF file. These don't support caching so we can load it directly.
                    return await Imaging.Helpers.LoadExtendedImageFromUrlAsync(url);
                }
                else if (typeof(T) == typeof(BitmapImage))
                {
                    //We are using the Silverlight load methods. These support caching.
                    using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        string file = Path.Combine(CacheFolder, getCachenameFromUri(url));

                        if (cacheContainsUri(url))
                        {
                            BitmapImage cachedImage = new BitmapImage();
                            using (IsolatedStorageFileStream cachedFile = storage.OpenFile(file, FileMode.Open, FileAccess.Read))
                            {
                                cachedImage.SetSource(cachedFile);
#if DEBUG
                                Globals.log("Loaded image {0} from cached file {1}", url, file);
#endif
                                return cachedImage;
                            }
                        }
                        else
                        {
                            BitmapImage loadedImage = await Imaging.Helpers.LoadImageFromUrlAsync(url);
                            saveImageToCache(loadedImage, file, storage);
                            return loadedImage;
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException("You can only load ExtendedImage and SourceImage.");
                }
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
#if DEBUG
                    Globals.log("Error loading from cache: {0}", url);
#endif
                }
#if DEBUG
                throw;
#else
                return null;
#endif
            }
        }

        /// <summary>
        /// Saves an Image to the cache
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="filename">The filename to save (full relative path)</param>
        /// <param name="storage">The isolated storage of the application</param>
        private static void saveImageToCache(BitmapImage image, string filename, IsolatedStorageFile storage)
        {
            try
            {
                using (IsolatedStorageFileStream cachedFile = storage.OpenFile(filename, FileMode.Create))
                {
                    WriteableBitmap bitmap = new WriteableBitmap(image);
                    bitmap.SaveJpeg(cachedFile, bitmap.PixelWidth, bitmap.PixelHeight, 0, 80);

#if DEBUG
                    Globals.log("Created cached file {0}", filename);
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
