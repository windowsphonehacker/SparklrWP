extern alias ImageToolsDLL;
using ImageToolsDLL::ImageTools;
using ImageToolsDLL::ImageTools.Controls;
using SparklrWP.Utils;
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
        public static DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(String), typeof(FrameworkElement), new PropertyMetadata(imageSourceChanged));
        public static DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(FrameworkElement), new PropertyMetadata(stretchChanged));

        private bool forceGifLoading = false;
        public bool ForceGIFLoading
        {
            get
            {
                return forceGifLoading;
            }
            set
            {
                if (forceGifLoading != value)
                {
                    forceGifLoading = value;
                    refreshImage();
                }
            }
        }
        public static BitmapImage GIFplaceholder;

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
                clearImage();

                if (imageSource != value)
                {
                    imageSource = value;
                    if (String.IsNullOrEmpty(value))
                    {
                        unloadImage();
                        clearImage();
                    }
                    else
                    {
                        loadImage();
                    }
                }
            }
        }

        private void refreshImage()
        {
            unloadImage();
            loadImage();
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
                    if (imageDisplay != null)
                        if (imageDisplay is AnimatedImage)
                        {
                            (imageDisplay as AnimatedImage).Stretch = value;
                        }
                        else
                        {
                            (imageDisplay as Image).Stretch = value;
                        }
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
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                if (ImageSource != null)
                {
                    Image b = new Image();
                    b.Source = new BitmapImage(new Uri(ImageSource));
                    imageDisplay = b;
                }
            }
            else if (!String.IsNullOrEmpty(ImageSource))
            {
                string loadedLocation = String.Copy(ImageSource);

                try
                {
                    if (loadedLocation.EndsWith("gif", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (ForceGIFLoading || Settings.LoadGIFsInStream)
                        {
                            ExtendedImage loadedImage = (ExtendedImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<ExtendedImage>(loadedLocation);

                            if (ImageSource == loadedLocation)
                            {
                                AnimatedImage image = new AnimatedImage();
                                image.Stretch = stretch;
                                image.Source = loadedImage;
                                LayoutRoot.Children.Add(image);
                                CurrentImageMode = ExtendedImageMode.AnimatedImage;
                                loadedImage = null;
#if DEBUG
                                App.logger.log("Loaded {0} as animated image", loadedLocation);
#endif
                                imageDisplay = image;
                                raiseImageUpdated();
                            }
                        }
                        else
                        {
                            Image i = new Image();
                            i.Source = GIFplaceholder;
                            i.Stretch = System.Windows.Media.Stretch.UniformToFill;
                            LayoutRoot.Children.Add(i);
                            raiseImageUpdated();
                        }
                    }
                    else
                    {
                        BitmapImage loadedImage = (BitmapImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(loadedLocation);
                        if (ImageSource == loadedLocation)
                        {
                            Image image = new Image();
                            image.Stretch = stretch;
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

        private void clearImage()
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                LayoutRoot.Children.Clear();
            });
        }

        private void unloadImage()
        {
            SmartDispatcher.BeginInvoke(() =>
            {
                if (imageDisplay != null)
                {
                    if (imageDisplay is AnimatedImage)
                    {
                        if ((imageDisplay as AnimatedImage).Source != null & (imageDisplay as AnimatedImage).Source.Frames != null)
                            (imageDisplay as AnimatedImage).Source.Frames.Clear();

                        (imageDisplay as AnimatedImage).Stop();

                        (imageDisplay as AnimatedImage).Source = null;
                    }
                    else if (imageDisplay is Image && ((Image)imageDisplay).Source != GIFplaceholder)
                    {
                        (imageDisplay as Image).Source = null;
                    }

                    imageDisplay = null;
                }
            });
        }

        public ExtendedImageControl()
        {
            if (GIFplaceholder == null)
            {
                GIFplaceholder = new BitmapImage(new Uri("/Assets/GIF.png", UriKind.Relative));
            }

            InitializeComponent();
        }

        public void Dispose()
        {
            unloadImage();
            GC.SuppressFinalize(this);
        }
    }
}
