extern alias ImageToolsDLL;
using SparklrWP.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SparklrWP.Controls
{
    public enum Location
    {
        Top,
        Bottom
    }

    [Description("Provides a control that can display a Sparklr post.")]
    public sealed partial class SparklrText : UserControl, IDisposable
    {
        public static readonly DependencyProperty UserbarLocationProperty = DependencyProperty.Register("UserbarLocation", typeof(Location), typeof(object), new PropertyMetadata(userbarLocationChanged));
        /// <summary>
        /// The content of the post
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(object), new PropertyMetadata(textPropertyChanged));

        /// <summary>
        /// The author's name
        /// </summary>
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(object), new PropertyMetadata(usernamePropertyChanged));

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
        public static readonly DependencyProperty PostProperty = DependencyProperty.Register("Post", typeof(PostItemViewModel), typeof(object), new PropertyMetadata(postPropertyChanged));

        /// <summary>
        /// Specifies if the comment numer is visible or not
        /// </summary>
        public static readonly DependencyProperty CommentCountVisibilityProperty = DependencyProperty.Register("CommentCountVisibility", typeof(Visibility), typeof(object), new PropertyMetadata(commentCountVisibilityChanged));

        /// <summary>
        /// Specifies if the element can be deleted by the used
        /// </summary>
        public static readonly DependencyProperty IsDeletableProperty = DependencyProperty.Register("IsDeletable", typeof(bool), typeof(object), new PropertyMetadata(isDeletableChanged));

        private static void textPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Text = e.NewValue.ToString();
        }

        private static void isDeletableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.IsDeletable = (bool)e.NewValue;
        }

        private static void commentCountVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.CommentCountVisibility = (Visibility)e.NewValue;
        }

        static void userbarLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.UserbarLocation = (Location)e.NewValue;
        }

        private static void postPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Post = (PostItemViewModel)e.NewValue;
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

        private static void usernamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SparklrText control = d as SparklrText;
            control.Username = e.NewValue.ToString();
        }


        public event EventHandler Delete;

        private string text;
        private string username;
        private string imagelocation;
        private bool isDeletable;
        private int? comments;
        private PostItemViewModel post;
        private Location userbarLocation = Location.Bottom;
        private Visibility commentCountVisibility = Visibility.Visible;

        /// <summary>
        /// The location of the userbar
        /// </summary>
        public Location UserbarLocation
        {
            get
            {
                return userbarLocation;
            }
            set
            {
                if (userbarLocation != value)
                {
                    userbarLocation = value;

                    switch (userbarLocation)
                    {
                        case Location.Top:
                            Grid.SetRow(sparklrText, 1);
                            Grid.SetRow(userbar, 0);
                            break;
                        case Location.Bottom:
                            Grid.SetRow(sparklrText, 0);
                            Grid.SetRow(userbar, 1);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The content of the post
        /// </summary>
        public Visibility CommentCountVisibility
        {
            get
            {
                return commentCountVisibility;
            }
            set
            {
                if (commentCountVisibility != value)
                {
                    commentCountVisibility = value;

                    switch (commentCountVisibility)
                    {
                        case Visibility.Collapsed:
                            commentCountContainer.Visibility = Visibility.Collapsed;
                            Grid.SetColumnSpan(usernameTextBlock, 2);
                            break;

                        case Visibility.Visible:
                            commentCountContainer.Visibility = Visibility.Visible;
                            Grid.SetColumnSpan(usernameTextBlock, 1);
                            break;
                    }
                }
            }
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
                    sparklrText.Text = value;
                    refreshVisibility();
                }
            }
        }

        public bool IsDeletable
        {
            get
            {
                return isDeletable;
            }
            set
            {
                if (isDeletable != value)
                {
                    isDeletable = value;
                    refreshVisibility();
                }
            }
        }

        /// <summary>
        /// A underlying post. Configures everything else in here. As a workarounf for issue #33, you can't update the control with a post that has a diffrent ID
        /// </summary>
        public PostItemViewModel Post
        {
            get
            {
                return post;
            }
            set
            {
                if (post != value)
                {
                    if (Post != null)
                    {
                        Post.PropertyChanged -= Post_PropertyChanged;
                    }
                    this.ImageLocation = value.ImageUrl;
                    this.Username = value.AuthorName;
                    this.Comments = value.CommentCount;
                    this.Text = value.Message;

                    post = value;
                    Post.PropertyChanged += Post_PropertyChanged;
                }
            }
        }

        public void Dispose()
        {
            if (Post != null)
                Post.PropertyChanged -= Post_PropertyChanged;
        }

        void Post_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ImageUrl":
                    this.ImageLocation = Post.ImageUrl;
                    break;

                case "Username":
                    this.Username = Post.AuthorName;
                    break;

                case "CommentCount":
                    this.Comments = Post.CommentCount;
                    break;

                case "Message":
                    this.Text = Post.Message;
                    break;
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
                    sparklrText.ImageLocation = value;
                    imagelocation = value;
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
        /// Gets the visibility of the userbar, depending on username, comments and likes
        /// </summary>
        public Visibility UserbarVisibility
        {
            get
            {
                return (String.IsNullOrEmpty(username) && comments == null) ? Visibility.Collapsed : Visibility.Visible;
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

        void postImage_ImageUpdated(object sender, EventArgs e)
        {
            refreshVisibility();
        }

        /// <summary>
        /// updates the visibility of the userbar
        /// </summary>
        private void refreshVisibility()
        {
            userbar.Visibility = UserbarVisibility;
            PostContextMenu.Visibility = IsDeletable ? Visibility.Visible : Visibility.Collapsed;
            PostContextMenu.IsEnabled = IsDeletable;
            this.InvalidateMeasure();
        }




        private void DeletePost_Click(object sender, RoutedEventArgs e)
        {
            if (Delete != null)
                Delete(this, null);
        }

        private void messageContentContainer_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = !IsDeletable;
        }

        private void sparklrText_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }
    }
}
