using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SparklrWP
{
    public class PostViewModel : INotifyPropertyChanged
    {
        public int Id { get; private set; }

        public PostViewModel(ItemViewModel post)
        {
            this.Id = post.Id;
            Comments = new ObservableCollection<ItemViewModel>();
            MainPost = post;
        }

        //For design:
        public PostViewModel()
        {

        }

        private ItemViewModel mainPost;
        public ItemViewModel MainPost
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
                    loadComments();
                    NotifyPropertyChanged("MainPost");
                }
            }
        }

        private ObservableCollection<ItemViewModel> comments;
        public ObservableCollection<ItemViewModel> Comments
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
            if (obj is ItemViewModel)
            {
                ItemViewModel m = (ItemViewModel)obj;
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

        private async void loadComments()
        {
            GlobalLoading.Instance.IsLoading = true;

            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Post> post = await App.Client.GetPostInfo(this.Id);

            if (post != null && post.IsSuccessful && post.Object.comments != null)
            {
                foreach (Comment c in post.Object.comments)
                {
                    if (c != null)
                    {
                        string from = c.from.ToString();

                        JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> response = await App.Client.GetUsernamesAsync(new int[] { c.from });

                        if (response.IsSuccessful && response.Object[0] != null && !string.IsNullOrEmpty(response.Object[0].username))
                            from = response.Object[0].username;

                        comments.Add(new ItemViewModel(c.id)
                            {
                                Message = c.message,
                                From = from
                            });
                    }
                }
            }

            GlobalLoading.Instance.IsLoading = false;
        }
    }
}