extern alias ImageToolsDLL;
using ImageTools;
using ImageTools.Controls;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SparklrWP.Controls
{
    public enum ExtendedImageMode
    {
        AnimatedImage,
        StaticImage
    }

    /// <summary>
    /// The ExtendedImage provides easy access to cached Images. It also supports animated GIF files.
    /// </summary>
    public sealed partial class ExtendedImageControl : UserControl, IDisposable
    {
        public DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(String), typeof(FrameworkElement), new PropertyMetadata(imageSourceChanged));
        public DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(FrameworkElement), new PropertyMetadata(stretchChanged));

        private static void stretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedImageControl c = d as ExtendedImageControl;
            c.Stretch = (Stretch)e.NewValue;
        }

        private static void imageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedImageControl c = d as ExtendedImageControl;
            c.ImageSource = (String)e.NewValue;
        }

        private string imageSource = null;
        public String ImageSource
        {
            get
            {
                return imageSource;
            }
            set
            {
                unloadImage();

                if (imageSource != value)
                {
                    imageSource = value;
                    if (String.IsNullOrEmpty(value))
                    {
                        unloadImage();
                    }
                    else
                    {
                        loadImage();
                    }
                }
            }
        }

        private FrameworkElement imageDisplay = null;

        private Stretch stretch = Stretch.Uniform;
        public Stretch Stretch
        {
            get
            {
                return stretch;
            }
            set
            {
                if (stretch != value)
                {
                    stretch = value;
                    //TODO: implement stretch changes
                }
            }
        }

        public ExtendedImageMode CurrentImageMode { get; private set; }

        public event EventHandler ImageUpdated;
        public BitmapImage Image
        {
            get
            {
                if (imageDisplay is Image)
                    return (imageDisplay as Image).Source as BitmapImage;
                return null;
            }
        }

        private async void loadImage()
        {
            if (!String.IsNullOrEmpty(ImageSource))
            {
                string loadedLocation = String.Copy(ImageSource);

                try
                {
                    if (loadedLocation.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ExtendedImage loadedImage = (ExtendedImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<ExtendedImage>(loadedLocation);

                        if (ImageSource == loadedLocation)
                        {
                            AnimatedImage image = new AnimatedImage();
                            image.Source = loadedImage;
                            LayoutRoot.Children.Add(image);
                            CurrentImageMode = ExtendedImageMode.AnimatedImage;
#if DEBUG
                            App.logger.log("Loaded {0} as animated image", loadedLocation);
#endif
                            imageDisplay = image;
                            raiseImageUpdated();
                        }
                    }
                    else
                    {
                        BitmapImage loadedImage = (BitmapImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(loadedLocation);
                        if (ImageSource == loadedLocation)
                        {
                            Image image = new Image();
                            image.Source = loadedImage;
                            LayoutRoot.Children.Add(image);
                            CurrentImageMode = ExtendedImageMode.StaticImage;
#if DEBUG
                            App.logger.log("Loaded {0} as static image", loadedLocation);
#endif
                            imageDisplay = image;
                            raiseImageUpdated();
                        }
                    }
                }
                catch (WebException)
                {
#if DEBUG
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
#endif
                }
            }
        }

        private void raiseImageUpdated()
        {
            if (ImageUpdated != null)
                ImageUpdated(this, null);
        }

        private void unloadImage()
        {
            if (imageDisplay is AnimatedImage)
            {
                (imageDisplay as AnimatedImage).Stop();

                if ((imageDisplay as AnimatedImage).Source != null & (imageDisplay as AnimatedImage).Source.Frames != null)
                    (imageDisplay as AnimatedImage).Source.Frames.Clear();

                (imageDisplay as AnimatedImage).Source = null;
            }
            else if (imageDisplay is Image)
            {
                (imageDisplay as Image).Source = null;
            }

            imageDisplay = null;
            LayoutRoot.Children.Clear();
        }

        public ExtendedImageControl()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            unloadImage();
        }
    }
}
