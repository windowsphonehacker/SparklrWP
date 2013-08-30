extern alias ImageToolsDLL;
using ImageToolsDLL::ImageTools;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SparklrWP.Utils
{
    static class Helpers
    {
        /// <summary>
        /// Initializes the Helper class
        /// </summary>
        static Helpers()
        {
            ImageToolsHelper.InitializeImageTools();
        }

        /// <summary>
        /// Loads an image from an Url asynchronously
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A BitmapImage, that can be set as a source</returns>
        public static async Task<ExtendedImage> LoadImageFromUrlAsync(Uri location)
        {
            WebClient client = new WebClient();
            ExtendedImage image = new ExtendedImage();
            Stream source = await client.OpenReadTaskAsync(location);

            if (location.ToString().EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
            {
                image.SetSource(source);

                TaskCompletionSource<ExtendedImage> imageLoaded = new TaskCompletionSource<ExtendedImage>();

                EventHandler loadingCompleteHandler = new EventHandler((sender, e) =>
                {
                    imageLoaded.SetResult(image);
                });

                EventHandler<UnhandledExceptionEventArgs> loadingFailedHandler = new EventHandler<UnhandledExceptionEventArgs>((sender, e) =>
                {
                    imageLoaded.SetResult(image);
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
#endif
                });



                image.LoadingCompleted += loadingCompleteHandler;
                image.LoadingFailed += loadingFailedHandler;

                image = await imageLoaded.Task;

                //Remove handlers, otherwise the object might be kept in the memory
                image.LoadingCompleted -= loadingCompleteHandler;
                image.LoadingFailed -= loadingFailedHandler;
            }
            else
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(source);
                WriteableBitmap writeable = new WriteableBitmap(bmp);
                image = ImageExtensions.ToImage(writeable);
            }

            source.Close();
            return image;
        }

        /// <summary>
        /// Loads an image from an Url asynchronously
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A BitmapImage, that can be set as a source</returns>
        public static Task<ExtendedImage> LoadImageFromUrlAsync(string location)
        {
            return LoadImageFromUrlAsync(new Uri(location));
        }
    }
}
