using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace SparklrWP
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        public SearchViewModel()
        {
        }

        private ObservableCollection<FriendViewModel> users = new ObservableCollection<FriendViewModel>();
        public ObservableCollection<FriendViewModel> Users
        {
            get
            {
                return users;
            }
            set
            {
                if (users != value)
                {
                    users = value;
                    NotifyPropertyChanged("Users");
                }
            }
        }

        private ObservableCollection<ItemViewModel> posts = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Posts
        {
            get
            {
                return posts;
            }
            set
            {
                if (posts != value)
                {
                    posts = value;
                    NotifyPropertyChanged("Posts");
                }
            }
        }

        private string keyword = "";
        public string Keyword
        {
            get
            {
                return keyword;
            }
            set
            {
                if (keyword != value)
                {
                    keyword = value;
                    NotifyPropertyChanged("Keyword");
                }
            }
        }

        public bool isReady = true;
        public bool IsReady
        {
            get
            {
                return isReady;
            }
            private set
            {
                if (isReady != value)
                {
                    isReady = value;
                    NotifyPropertyChanged("IsReady");
                }
            }
        }

        public async void Search()
        {
            IsReady = false;
            GlobalLoading.Instance.IsLoading = true;
            JSONRequestEventArgs<Search> results = await App.Client.SearchAsync(Keyword);

            if (results.IsSuccessful)
            {
                Posts.Clear();
                Users.Clear();

                foreach (SearchUser user in results.Object.users)
                {
                    Users.Add(new FriendViewModel(user.id)
                        {
                            Name = user.username,
                            Image = "http://d.sparklr.me/i/t" + user.id + ".jpg"
                        });
                }

                foreach (SearchPost post in results.Object.posts)
                {
                    Posts.Add(new ItemViewModel(post.id)
                        {
                            AuthorId = post.from,
                            Message = post.message,
                            CommentCount = post.commentcount ?? 0,
                            ImageUrl = !String.IsNullOrEmpty(post.meta) ? String.Format("http://d.sparklr.me/i/t{0}", post.meta) : null
                        });
                }
            }
            else
            {
                MessageBox.Show("We're having trouble connecting to sparklr. Please try again later.", "Oops...", MessageBoxButton.OK);
            }

            GlobalLoading.Instance.IsLoading = false;
            IsReady = true;
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
    }
}
