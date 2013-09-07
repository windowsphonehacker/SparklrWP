using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using SparklrWP.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SparklrWP
{
    /// <summary>
    /// Represents a single post
    /// </summary>
    public class PostItemViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Creates a new instance of the post an fills in all the necessary details.
        /// </summary>
        /// <param name="Id">The id of the post</param>
        public PostItemViewModel(int Id)
        {
            this.Id = Id;
            UpdatePostInfo();
        }

        public PostItemViewModel(
            int id,
            int authorId,
            string message,
            string authorImage = null,
            string authorName = null,
            int? commentCount = null,
            ObservableCollectionWithItemNotification<CommentModel> comments = null,
            bool deletable = false,
            string imageUrl = null,
            string network = null,
            int orderTime = 0,
            int time = 0,
            int? viaId = null,
            string viaImage = null,
            string viaName = null)
        {
            this.Id = id;
            this.AuthorId = authorId;
            this.Message = message;
            this.AuthorImage = authorImage;
            this.AuthorName = authorName;
            this.CommentCount = commentCount;
            this.Comments = comments;
            this.Deletable = deletable;
            this.ImageUrl = imageUrl;
            this.Network = network;
            this.OrderTime = orderTime;
            this.Time = time;
            this.ViaId = viaId;
            this.ViaImage = viaImage;
            this.ViaName = viaName;
        }

        /// <summary>
        /// Performs an update of the posts information
        /// </summary>
        public async void UpdatePostInfo()
        {
            JSONRequestEventArgs<Post> result = await App.Client.GetPostInfoAsync(Id);

            if (result.IsSuccessful)
            {

                this.AuthorId = result.Object.from;
                this.CommentCount = result.Object.commentcount;
                //TODO: Add comments collection
                this.Deletable = result.Object.from == App.Client.UserId;
                this.ImageUrl = !String.IsNullOrEmpty(result.Object.meta) ? String.Format("http://d.sparklr.me/i/t{0}", result.Object.meta.Split(',')[0]) : null;
                this.Message = result.Object.message;
                this.Network = result.Object.network;
                this.OrderTime = result.Object.modified ?? result.Object.time;
                this.Time = result.Object.time;
                this.ViaId = result.Object.via;

                Comments = new ObservableCollectionWithItemNotification<CommentModel>();
                if (result.Object.comments != null)
                {
                    foreach (Comment c in result.Object.comments)
                    {
                        Comments.Add(new CommentModel(c.id)
                            {
                                From = c.from,
                                Message = c.message,
                                Time = c.time,
                                Deletable = c.from == App.Client.UserId
                            });
                    }
                }

                FillNamesAndImages();
            }
            else
            {
                //TODO: Use custom ExceptionType to allow better error handling.
                throw new Exception("Could not get post-data from server");
            }
        }

        public async void FillNamesAndImages()
        {
            this.AuthorImage = "http://d.sparklr.me/i/t" + this.AuthorId + ".jpg";

            if (ViaId != null)
                this.ViaImage = "http://d.sparklr.me/i/t" + this.ViaImage + ".jpg";

            List<int> namesToIdentify = new List<int>();
            namesToIdentify.Add(AuthorId);

            if (ViaId != null)
                namesToIdentify.Add((int)ViaId);

            JSONRequestEventArgs<Username[]> usernames = await App.Client.GetUsernamesAsync(namesToIdentify.ToArray());

            if (usernames.IsSuccessful)
            {
                foreach (Username u in usernames.Object)
                {
                    if (AuthorId == u.id && String.IsNullOrEmpty(AuthorName))
                    {
                        AuthorName = u.username;
                    }

                    if (ViaId == u.id && String.IsNullOrEmpty(ViaName))
                    {
                        ViaName = u.username;
                    }
                }
            }
        }

        //For design:
        public PostItemViewModel()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                throw new NotSupportedException("Use the PostItemViewModel(int Id) constructor instead");
            }
        }


        private ObservableCollectionWithItemNotification<CommentModel> _comments;
        public ObservableCollectionWithItemNotification<CommentModel> Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                if (_comments != value)
                {
                    _comments = value;
                    NotifyPropertyChanged("Comments");
                }
            }
        }


        private int _id;
        /// <summary>
        /// Contains the id of the post
        /// </summary>
        public int Id
        {
            get
            {
                return _id;
            }
            private set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private int _time;
        /// <summary>
        /// Contains the id of the post
        /// </summary>
        public int Time
        {
            get
            {
                return _time;
            }
            private set
            {
                if (_time != value)
                {
                    _time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }

        private int? _via;
        /// <summary>
        /// Contains the id of the via user.
        /// </summary>
        public int? ViaId
        {
            get
            {
                return _via;
            }
            private set
            {
                if (_via != value)
                {
                    _via = value;
                    NotifyPropertyChanged("ViaId");
                }
            }
        }

        private int _orderTime;
        /// <summary>
        /// Represents a timestamp that a mdel can use to sort
        /// </summary>
        public int OrderTime
        {
            get
            {
                return _orderTime;
            }
            private set
            {
                if (_orderTime != value)
                {
                    _orderTime = value;
                    NotifyPropertyChanged("OrderTime");
                }
            }
        }


        private int _authorId;
        /// <summary>
        /// Contains the id of the author.
        /// </summary>
        public int AuthorId
        {
            get
            {
                return _authorId;
            }
            private set
            {

                if (_authorId != value)
                {
                    _authorId = value;
                    NotifyPropertyChanged("AuthorId");
                }
            }
        }

        private string _authorName;
        /// <summary>
        /// Contains the friendly name of the via-user.
        /// </summary>
        public string AuthorName
        {
            get
            {
                return _authorName;
            }
            private set
            {
                if (value != _authorName)
                {
                    _authorName = value;
                    NotifyPropertyChanged("AuthorName");
                }
            }
        }

        private string _viaName;
        /// <summary>
        /// Contains the friendly name of the author.
        /// </summary>
        public string ViaName
        {
            get
            {
                return _viaName;
            }
            private set
            {
                if (value != _viaName)
                {
                    _viaName = value;
                    NotifyPropertyChanged("ViaName");
                }
            }
        }

        private string _authorImage;
        /// <summary>
        /// Contains the location of the authors profile image
        /// </summary>
        public string AuthorImage
        {
            get
            {
                return _authorImage;
            }
            private set
            {
                if (_authorImage != value)
                {
                    _authorImage = value;
                    NotifyPropertyChanged("AuthorImage");
                }
            }
        }

        private string _viaImage;
        /// <summary>
        /// Contains the location of the authors profile image
        /// </summary>
        public string ViaImage
        {
            get
            {
                return _viaImage;
            }
            private set
            {
                if (_viaImage != value)
                {
                    _viaImage = value;
                    NotifyPropertyChanged("ViaImage");
                }
            }
        }

        private string _network;
        /// <summary>
        /// Contains the name of the network of the post
        /// </summary>
        public string Network
        {
            get
            {
                return _network;
            }
            private set
            {
                if (value != _network)
                {
                    _network = value;
                    NotifyPropertyChanged("Network");
                }
            }
        }

        private string _message;
        /// <summary>
        /// Contains the content of the post
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            private set
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }

        private int? _commentCount;
        /// <summary>
        /// Contains the number of the post
        /// </summary>
        public int? CommentCount
        {
            get
            {
                return _commentCount;
            }
            private set
            {
                if (value != _commentCount)
                {
                    _commentCount = value;
                    NotifyPropertyChanged("CommentCount");
                }
            }
        }

        private bool _deletable;
        /// <summary>
        /// Indicates if the post is deletable
        /// </summary>
        public bool Deletable
        {
            get
            {
                return _deletable;
            }
            private set
            {
                if (value != _deletable)
                {
                    _deletable = value;
                    NotifyPropertyChanged("Deletable");
                }
            }
        }

        private string _imageUrl;
        /// <summary>
        /// Contains a link to the posts image
        /// </summary>
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            private set
            {
                if (value != _imageUrl)
                {
                    _imageUrl = value;
                    NotifyPropertyChanged("ImageUrl");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PostItemViewModel)
            {
                PostItemViewModel m = (PostItemViewModel)obj;
                return this.Id == m.Id;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", this.Id, this.Message);
        }
    }
}