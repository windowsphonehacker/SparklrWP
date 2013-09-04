extern alias ImageToolsDLL;
using Coding4Fun.Toolkit.Controls;
using ImageToolsDLL::ImageTools;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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
        /// Loads an extended image from an Url asynchronously
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A BitmapImage, that can be set as a source</returns>
        public static async Task<ExtendedImage> LoadExtendedImageFromUrlAsync(Uri location)
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
        /// Loads an extended image from an Url asynchronously
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A BitmapImage, that can be set as a source</returns>
        public static Task<ExtendedImage> LoadExtendedImageFromUrlAsync(string location)
        {
            return LoadExtendedImageFromUrlAsync(new Uri(location));
        }

        /// <summary>
        /// Loads an image from an Url asynchronously
        /// </summary>
        /// <param name="location">The location of the image</param>
        /// <returns>A BitmapImage, that can be set as a source</returns>
        public static async Task<BitmapImage> LoadImageFromUrlAsync(Uri location)
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
        public static Task<BitmapImage> LoadImageFromUrlAsync(string location)
        {
            return LoadImageFromUrlAsync(new Uri(location));
        }

        /// <summary>
        /// Displays a non intrusive toast notification
        /// </summary>
        /// <param name="text">The text to display</param>
        public static void Notify(string text)
        {
            ToastPrompt p = new ToastPrompt();
            p.Message = text;
            p.Show();
        }

        /// <summary>
        /// Displays a non intrusive toast notification
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="caption">The title of the message</param>
        public static void Notify(string caption, string text)
        {
            ToastPrompt p = new ToastPrompt();
            p.Message = text;
            p.Title = caption;
            p.Show();
        }

        /// <summary>
        /// Finds a scroll viewer inside the specified parents visual tree.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static ScrollViewer FindScrollViewer(this DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                if (elt is ScrollViewer) return (ScrollViewer)elt;
                var result = FindScrollViewer(elt);
                if (result != null) return result;
            }
            return null;
        }

        /// <summary>
        /// Updates the binding on a textbox
        /// </summary>
        /// <param name="textBox"></param>
        public static void UpdateBinding(this TextBox textBox)
        {
            BindingExpression bindingExpression =
                    textBox.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateSource();
            }
        }
    }
}
