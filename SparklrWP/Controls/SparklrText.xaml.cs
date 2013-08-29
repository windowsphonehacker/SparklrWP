using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SparklrWP.Controls
{
    [Description("Provides a control that can display a Sparklr post.")]
    public partial class SparklrText : UserControl
    {
        /// <summary>
        /// The content of the post
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(object), new PropertyMetadata(textPropertyChanged));

        /// <summary>
        /// The author's name
        /// </summary>
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(object), new PropertyMetadata(usernamePropertyChanged));

        /// <summary>
        /// The number of likes
        /// </summary>
        public static readonly DependencyProperty LikesProperty = DependencyProperty.Register("Likes", typeof(int), typeof(object), new PropertyMetadata(likesPropertyChanged));

        /// <summary>
        /// The number of comments
        /// </summary>
        public static readonly DependencyProperty CommentsProperty = DependencyProperty.Register("Comments", typeof(int), typeof(object), new PropertyMetadata(commentsPropertyChanged));

        /// <summary>
        /// The locatio (URI) of the image
        /// </summary>
        public static readonly DependencyProperty ImageLocationProperty = DependencyProperty.Register("ImageLocation", typeof(string), typeof(object), new PropertyMetadata(imagelocationPropertyChanged));

        /// <summary>
        /// A ItemViewModel that contains all the required data.
        /// </summary>
        public static readonly DependencyProperty PostProperty = DependencyProperty.Register("Post", typeof(ItemViewModel), typeof(object), new PropertyMetadata(postPropertyChanged));

        private static void postPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Post = (ItemViewModel)e.NewValue;
        }

        private static void imagelocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.ImageLocation = (string)e.NewValue;
        }


        private static void commentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Comments = (int)e.NewValue;
        }


        private static void likesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Likes = (int)e.NewValue;
        }

        private static void usernamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Username = e.NewValue.ToString();
        }

        private static void textPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// Matches hashtags like #test and #123
        /// </summary>
        private static Regex hashTagRegex = new Regex(@"(#[\w\b]*)", RegexOptions.Compiled);

        /// <summary>
        /// Matches usernames like @test and @123
        /// </summary>
        private static Regex userMentionRegex = new Regex(@"(@[\w\b]*)", RegexOptions.Compiled);

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

        private string text;
        private string username;
        private string imagelocation;
        private int? likes;
        private int? comments;
        private ItemViewModel post;
        private BitmapImage image;

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
        /// A underlying post. Configures everything else in here. As a workarounf for issue #33, you can't update the control with a post that has a diffrent ID
        /// </summary>
        public ItemViewModel Post
        {
            get
            {
                return post;
            }
            set
            {
                if (post != value)
                {
                    this.ImageLocation = value.ImageUrl;
                    this.Username = value.From;
                    this.Comments = value.CommentCount;
                    this.Likes = value.LikesCount;
                    this.Text = value.Message;

                    post = value;
                }
            }
        }

        /// <summary>
        /// The username of the post author
        /// </summary>
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (username != value)
                {
                    username = value;
                    usernameTextBlock.Text = username;
                    refreshVisibility();
                }
            }
        }

        /// <summary>
        /// The number of comments
        /// </summary>
        public int? Comments
        {
            get
            {
                return comments;
            }
            set
            {
                if (comments != value)
                {
                    comments = value;
                    commentCountTextBlock.Text = comments == 1 ? "1" : String.Format("{0}", comments);
                    refreshVisibility();
                }
            }
        }

        /// <summary>
        /// The number of likes
        /// </summary>
        public int? Likes
        {
            get
            {
                return likes;
            }
            set
            {
                if (likes != value)
                {
                    likes = value;
                    likesTextBlock.Text = String.Format("{0} likes", likes);
                    refreshVisibility();
                }
            }
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
                    bool startLoading = false;
                    if (String.IsNullOrEmpty(value))
                    {
                        MessageImage.Source = null;
                    }
                    else
                    {
                        if (value.EndsWith(",[]"))
                            value = value.Replace(",[]", "");

                        startLoading = true;
                    }

                    imagelocation = value;

                    if (startLoading)
                        loadImage(value);
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

        private async void loadImage(string value)
        {
            try
            {
                string oldLink = String.Copy(value);
                BitmapImage loaded = await Utils.Caching.Image.LoadCachedImageFromUrlAsync(value);

                if (oldLink == this.imagelocation)
                {
                    MessageImage.Source = loaded;
                    image = loaded;
                    refreshVisibility();
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

        /// <summary>
        /// Gets the visibility of the userbar, depending on username, comments and likes
        /// </summary>
        public Visibility UserbarVisibility
        {
            get
            {
                return (String.IsNullOrEmpty(username) && likes == null && comments == null) ? Visibility.Collapsed : Visibility.Visible;
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
        /// Creates a new instance of the SparklrText control
        /// </summary>
        public SparklrText()
        {
            InitializeComponent();
            this.LayoutRoot.DataContext = this;
        }

        /// <summary>
        /// updates the visibility of the userbar
        /// </summary>
        private void refreshVisibility()
        {
            userbar.Visibility = UserbarVisibility;
            ImageContainer.Visibility = ImageVisibility;
            messageContentContainer.Visibility = TextVisibility;
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Rebuilts and rehighlights the post
        /// </summary>
        /// <param name="value">The post content</param>
        private void updateText(string value)
        {
            messageContentParagraph.Inlines.Clear();
            //Split on every hashtag
            string[] splittedTags = hashTagRegex.Split(value);

            //Iterate over the parts
            foreach (string s in splittedTags)
            {

                if (hashTagRegex.IsMatch(s))
                {
                    //If the hashtag regex matches the substring, we only have a hashtag
                    messageContentParagraph.Inlines.Add(getHighlightedInline(s));
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
            if (image != null)
            {
                try
                {
                    string filename = String.Format("{0}", Guid.NewGuid().ToString("N"));
                    WriteableBitmap bmp = new WriteableBitmap(image);

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
                                MessageBox.Show("Hooray! We downloaded the image. You can now admire it via the Image hub.");
                            };

                        downloader.OpenReadAsync(new Uri(location));

                        MessageBox.Show("We are now downloading the image in the beackground. We will tell you when everything is ready!");
                    }
                    else
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bmp.SaveJpeg(ms, bmp.PixelWidth, bmp.PixelHeight, 0, 85);
                            ms.Seek(0, SeekOrigin.Begin);

                            //TODO: Uncomment on release
                            using (MediaLibrary library = new MediaLibrary())
                            {
                                library.SavePicture(filename, ms);
                            }

                            MessageBox.Show("Yay! We saved the image to your gallery. You can now admire it everytime you want!");
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
    }
}
