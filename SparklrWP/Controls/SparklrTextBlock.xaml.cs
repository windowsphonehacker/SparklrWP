extern alias ImageToolsDLL;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SparklrWP.Controls
{
    public partial class SparklrTextBlock : UserControl
    {
        public SparklrTextBlock()
        {
            InitializeComponent();
            this.LayoutRoot.DataContext = this;
            postImage.ImageUpdated += postImage_ImageUpdated;
        }

        void postImage_ImageUpdated(object sender, EventArgs e)
        {
            refreshVisibility();
        }

        /// <summary>
        /// The content of the post
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(object), new PropertyMetadata(textPropertyChanged));

        private static void textPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrTextBlock control = d as SparklrTextBlock;
            control.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// The content of the post
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    updateText(value);
                    refreshVisibility();
                }
            }
        }

        /// <summary>
        /// updates the visibility of the userbar
        /// </summary>
        private void refreshVisibility()
        {
            ImageContainer.Visibility = ImageVisibility;
            messageContentContainer.Visibility = TextVisibility;
            SaveImageMenuEntry.IsEnabled = postImage.CurrentImageMode == ExtendedImageMode.StaticImage;
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Contains the URL to the related image
        /// </summary>
        public string ImageLocation
        {
            get
            {
                return imagelocation;
            }
            set
            {
                if (imagelocation != value)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        if (value.EndsWith(",[]"))
                            value = value.Replace(",[]", "");
                    }

                    postImage.ImageSource = value;
                    imagelocation = value;
                    refreshVisibility();
                }
            }
        }

        public Visibility TextVisibility
        {
            get
            {
                return String.IsNullOrEmpty(this.text) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the visibility for the image area
        /// </summary>
        public Visibility ImageVisibility
        {
            get
            {
                return String.IsNullOrEmpty(imagelocation) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// Matches hashtags like #test and #123
        /// </summary>
        private static Regex hashTagRegex = new Regex(@"(#[\w-]+)", RegexOptions.Compiled);

        /// <summary>
        /// Matches usernames like @test and @123
        /// </summary>
        private static Regex userMentionRegex = new Regex(@"(@[\w\b]*)", RegexOptions.Compiled);

        /// <summary>
        /// Matches the user id from "[1] Repost"
        /// </summary>
        private static Regex repostRegex = new Regex(@"^\[(\d+)\]", RegexOptions.Compiled);

        /// <summary>
        /// A regex that matches any url and captures the destination without the http(s)://
        /// </summary>
        private static Regex urlRegex = new Regex(@"(?:((?:http|ftp|https):\/\/))?([\w\-_]+(?:\.[\w\-_]+)+(?:[\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex urlSplitRegex = new Regex(@"((?:(?:http|ftp|https):\/\/)?(?:[\w\-_]+(?:\.[\w\-_]+)+(?:[\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// The highlight color for tags, usernames, etc...
        /// </summary>
        private SolidColorBrush accentColor = GetColorFromHex("FF454050");
        private SolidColorBrush accentBackgroundColor = new SolidColorBrush(Colors.White);

        /// <summary>
        /// Rebuilts and rehighlights the post
        /// </summary>
        /// <param name="value">The post content</param>
        private async void updateText(string value)
        {
            messageContentParagraph.Inlines.Clear();

            if (value == "☝")
            {
                messageContentParagraph.Inlines.Add(new Run() { Text = "likes this post.", FontStyle = FontStyles.Italic });
            }
            else
            {
                string[] lines = value.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (repostRegex.IsMatch(lines[i]))
                    {
                        Match firstMatch = repostRegex.Match(lines[i]);
                        int userId = Convert.ToInt32(firstMatch.Groups[1].Value);
                        lines[i] = lines[i].Replace(String.Format("[{0}] ", userId), "");
                        messageContentParagraph.Inlines.Add(await getRepostAsInline(userId));
                    }

                    //Split on every hashtag
                    string[] splittedTags = hashTagRegex.Split(lines[i]);

                    //Iterate over the parts
                    foreach (string s in splittedTags)
                    {

                        if (hashTagRegex.IsMatch(s))
                        {
                            //If the hashtag regex matches the substring, we only have a hashtag
                            messageContentParagraph.Inlines.Add(getInlineAsTag(s));
                        }
                        else
                        {
                            //See if the part contains at least one mention
                            if (userMentionRegex.IsMatch(s))
                            {
                                //split the mentions
                                string[] usernameParts = userMentionRegex.Split(s);

                                foreach (string username in usernameParts)
                                {
                                    if (userMentionRegex.IsMatch(username))
                                        messageContentParagraph.Inlines.Add(getAsInlineUsername(username));
                                    else
                                    {
                                        //Check if we still have urls in here
                                        if (urlRegex.IsMatch(username))
                                        {
                                            replaceUrls(username);
                                        }
                                        else
                                            messageContentParagraph.Inlines.Add(getAsInline(username));
                                    }
                                }
                            }
                            else if (urlRegex.IsMatch(s))
                            {
                                replaceUrls(s);
                            }
                            else
                            {
                                //The substring doesn't contain username
                                messageContentParagraph.Inlines.Add(getAsInline(s));
                            }
                        }
                    }

                    if ((i + 1) < lines.Length)
                    {
                        messageContentParagraph.Inlines.Add(new LineBreak());
                        messageContentParagraph.Inlines.Add(new LineBreak());
                    }
                }
            }
            this.InvalidateMeasure();
        }


        private void replaceUrls(string value)
        {
            string[] urlparts = urlSplitRegex.Split(value);

            foreach (string urlpart in urlparts)
            {
                //Match the urlpart against the regex
                Match urlMatch = urlRegex.Match(urlpart);


                if (urlMatch.Captures.Count == 0)
                {
                    //No url found, add urlpart as text
                    messageContentParagraph.Inlines.Add(getAsInline(urlpart));
                }
                else
                {
                    //Add part as url
                    //Create a valid url from the matches
                    string url;

                    //see if we matched the protocol and build based on that
                    if (!String.IsNullOrEmpty(urlMatch.Groups[1].Value))
                    {
                        url = String.Format("{0}{1}", urlMatch.Groups[1].Value, urlMatch.Groups[2].Value);
                    }
                    else
                    {
                        //http:// is missing
                        url = String.Format("http://{0}", urlMatch.Groups[2].Value);
                    }

                    Uri uri;
                    if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
                        messageContentParagraph.Inlines.Add(getAsInlineLink(urlpart, uri));
                    else
                        messageContentParagraph.Inlines.Add(getAsInline(urlpart));
                }
            }
        }



        /// <summary>
        /// creates a text element for a RichTextBox
        /// </summary>
        /// <param name="text">the text you want to add</param>
        /// <returns>A Inline element that can be added via Paragraph.Inlines.Add</returns>
        private Inline getAsInline(string text)
        {
            return new Run
            {
                Text = text
            };
        }

        /// <summary>
        /// creates a link for a RichTextBox
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="target">The target location</param>
        /// <returns>A Inline element that can be added via Paragraph.Inlines.Add</returns>
        private Inline getAsInlineLink(string text, Uri target)
        {
            Hyperlink ret = new Hyperlink();
            ret.TargetName = "_blank";
            ret.Foreground = accentColor;
            ret.NavigateUri = target;
            ret.Inlines.Add(text);
            return ret;
        }

        /// <summary>
        /// Formats the given string as clickable hastag
        /// </summary>
        /// <param name="s">the hashtag to format</param>
        /// <returns>the formatted hashtag</returns>
        private Inline getInlineAsTag(string s)
        {
            Hyperlink ret = new Hyperlink();
            ret.Foreground = accentColor;
            ret.Inlines.Add(s);

            ret.Click += (sender, e) =>
            {
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format("/Pages/TagsPage.xaml?tag={0}", s.EncodeUrl()), UriKind.Relative));
            };

            return ret;
        }

        private Inline getAsInlineUsername(string username)
        {
            if (username.StartsWith("@"))
                username = username.TrimStart('@');

            Hyperlink ret = new Hyperlink();
            ret.Foreground = accentColor;
            ret.Inlines.Add("@" + username);

            ret.Click += (sender, e) =>
            {
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format("/Pages/Profile.xaml?userId={0}", username), UriKind.Relative));
            };

            return ret;
        }


        /// <summary>
        /// creates a highlighted text element for a RichTextBox
        /// </summary>
        /// <param name="text">the text you want to add</param>
        /// <returns>A formatted Inline element that can be added via Paragraph.Inlines.Add</returns>
        private Inline getHighlightedInline(string text)
        {
            /*
            InlineUIContainer container = new InlineUIContainer();
            StackPanel panel = new StackPanel();
            panel.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            panel.Height = 20;
            panel.Margin = new Thickness(3, 0, 3, 0);
            panel.Background = accentBackgroundColor;
            panel.Children.Add(new TextBlock() { Text = text, Foreground = accentColor, FontSize = 16, Padding = new Thickness(5, 2, 5, 2) });
            container.Child = panel;
            return container;*/

            return new Run
            {
                Text = text,
                Foreground = accentColor,
                TextDecorations = TextDecorations.Underline
            };
        }

        //TODO: extract to util namespace
        /// <summary>
        /// Creates a SolidColorBrush from a "AARRGGBB" string
        /// </summary>
        /// <param name="hexaColor">The string (format: "AARRGGBB")</param>
        /// <returns>A SolidColorBrush with the specified color</returns>
        public static SolidColorBrush GetColorFromHex(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(0, 2), 16),
                    Convert.ToByte(hexaColor.Substring(2, 2), 16),
                    Convert.ToByte(hexaColor.Substring(4, 2), 16),
                    Convert.ToByte(hexaColor.Substring(6, 2), 16)
                )
            );
        }

        //TODO: extract to Util namespace
        private void SaveImageToPhone_Click(object sender, RoutedEventArgs e)
        {
            if (postImage.Image != null)
            {
                try
                {
                    string filename = String.Format("{0}", Guid.NewGuid().ToString("N"));

                    if (ImageLocation.StartsWith("http://d.sparklr.me/i/t", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //We have a thumbnail
                        string location = ImageLocation.Replace("http://d.sparklr.me/i/t", "http://d.sparklr.me/i/");

                        WebClient downloader = new WebClient();
                        downloader.OpenReadCompleted += (dSender, dE) =>
                        {
                            using (MediaLibrary library = new MediaLibrary())
                            {
                                library.SavePicture(filename, dE.Result);
                            }
                            Helpers.Notify("Hooray!", "We've downloaded the image. You can now view it in the Image hub.");
                        };

                        downloader.OpenReadAsync(new Uri(location));

                        Helpers.Notify("Soon...", "...we will finish the download of your image. Stay tuned!");
                    }
                    else
                    {
                        WriteableBitmap bmp = new WriteableBitmap(postImage.Image);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            bmp.SaveJpeg(ms, bmp.PixelWidth, bmp.PixelHeight, 0, 85);
                            ms.Seek(0, SeekOrigin.Begin);

                            //TODO: Uncomment on release
                            using (MediaLibrary library = new MediaLibrary())
                            {
                                library.SavePicture(filename, ms);
                            }

                            Helpers.Notify("Yay!", "We've downloaded the image. You can now view it in the Image hub.");
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("We really tried, but we couldn't save your picture. Please try again later.\r\nWe're sorry :(", "Oops!", MessageBoxButton.OK);
                }
            }
        }

        private void ImageContainer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string location = this.ImageLocation;
            if (ImageLocation.StartsWith("http://d.sparklr.me/i/t", StringComparison.InvariantCultureIgnoreCase))
                location = ImageLocation.Replace("http://d.sparklr.me/i/t", "http://d.sparklr.me/i/");

            location = HttpUtility.UrlEncode(location);

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(String.Format("/Pages/PinchToZoom.xaml?image={0}", location), UriKind.Relative));
        }

        private string text;
        private string imagelocation;

        private async Task<Inline> getRepostAsInline(int userId)
        {
            Span s = new Span();
            Image author = new Image();

            object loaded = await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(String.Format("http://d.sparklr.me/i/t{0}.jpg", userId));

            author.Source = (BitmapImage)loaded;
            author.Stretch = Stretch.UniformToFill;
            author.Width = 28;
            author.Height = 28;
            InlineUIContainer container = new InlineUIContainer();
            container.Child = author;

            s.Inlines.Add(container);
            s.Inlines.Add(" ");

            JSONRequestEventArgs<Username[]> usernames = await App.Client.GetUsernamesAsync(new int[] { userId });

            if (usernames.IsSuccessful && usernames.Object.Length > 0)
            {
                s.Inlines.Add(getAsInlineUsername(usernames.Object[0].username));
            }
            else
            {
                s.Inlines.Add(getAsInlineUsername(userId.ToString()));
            }
            s.Inlines.Add(": ");
            return s;
        }

    }
}
