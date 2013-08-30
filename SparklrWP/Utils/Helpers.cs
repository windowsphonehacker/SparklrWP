extern alias ImageToolsDLL;
using ImageToolsDLL::ImageTools;
using System;
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

            if (location.ToString().EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
            {
                image.SetSource(await client.OpenReadTaskAsync(location));

                TaskCompletionSource<ExtendedImage> imageLoaded = new TaskCompletionSource<ExtendedImage>();

                image.LoadingCompleted += (sender, e) =>
                    {
                        imageLoaded.SetResult(image);
                    };

                image.LoadingFailed += (sender, e) =>
                    {
                        imageLoaded.SetResult(null);
                    };

                image = await imageLoaded.Task;
            }
            else
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(await client.OpenReadTaskAsync(location));
                WriteableBitmap writeable = new WriteableBitmap(bmp);
                image = ImageExtensions.ToImage(writeable);
            }

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
