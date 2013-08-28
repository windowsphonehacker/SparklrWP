using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SparklrWP.Utils
{
    static class Helpers
    {
        /// <summary>
        /// Loads an image from an Url asynchronously
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A BitmapImage, that can be set as a source</returns>
        public static async Task<ImageSource> LoadImageFromUrlAsync(Uri location)
        {
            WebClient client = new WebClient();
            BitmapImage image = new BitmapImage();
            image.SetSource(await client.OpenReadTaskAsync(location));
            return image;
        }

        /// <summary>
        /// Loads an image from an Url asynchronously
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A BitmapImage, that can be set as a source</returns>
        public static Task<ImageSource> LoadImageFromUrlAsync(string location)
        {
            return LoadImageFromUrlAsync(new Uri(location));
        }
    }
}
