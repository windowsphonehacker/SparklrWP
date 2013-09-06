using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using SparklrWP.ViewModels;
using System;
using System.ComponentModel;

namespace SparklrWP
{
    public sealed class PostViewModel : INotifyPropertyChanged
    {
        public int Id { get; private set; }
        public bool Liked { get; private set; }
        public int LikeID { get; private set; }

        public PostViewModel(PostItemViewModel post)
        {
            this.Id = post.Id;
            Comments = new ObservableCollectionWithItemNotification<CommentModel>();
            MainPost = post;
        }

        //For design:
        public PostViewModel()
        {

        }

        private PostItemViewModel mainPost;
        public PostItemViewModel MainPost
        {
            get
            {
                return mainPost;
            }
            set
            {
                if (mainPost != value)
                {
                    mainPost = value;
                    loadComments(true);
                    NotifyPropertyChanged("MainPost");
                }
            }
        }

        private ObservableCollectionWithItemNotification<CommentModel> comments;
        public ObservableCollectionWithItemNotification<CommentModel> Comments
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
                    NotifyPropertyChanged("Comments");
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
            return String.Format("{0} - {1}", this.Id, this.MainPost.Message);
        }

        private async void loadComments(bool initial = false)
        {
            if (initial && MainPost.Comments != null)
            {
                Comments = MainPost.Comments;
            }
            else
            {
                GlobalLoading.Instance.IsLoading = true;

                comments.Clear();
                LikeID = -1;
                Liked = false;

                JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Post> post = await App.Client.GetPostInfoAsync(this.Id);

                if (post != null && post.IsSuccessful && post.Object.comments != null)
                {
                    foreach (Comment c in post.Object.comments)
                    {
                        if (c != null)
                        {
                            if (c.message == SparklrLib.SparklrClient.LikesEscape && c.from == App.Client.UserId)
                            {
                                Liked = true;
                                LikeID = c.id;
                            }

                            string from = c.from.ToString();

                            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> response = await App.Client.GetUsernamesAsync(new int[] { c.from });

                            if (response.IsSuccessful && response.Object[0] != null && !string.IsNullOrEmpty(response.Object[0].username))
                                from = response.Object[0].username;

                            comments.Add(new CommentModel(c.id)
                            {
                                From = c.from,
                                Message = c.message,
                                Time = c.time,
                                Deletable = c.from == App.Client.UserId
                            });
                        }
                    }
                }

                GlobalLoading.Instance.IsLoading = false;
            }
        }

        public void RefreshComments()
        {
            loadComments();
        }
    }
}