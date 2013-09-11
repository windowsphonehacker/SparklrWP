using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace SparklrWP.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        public SearchViewModel()
        {
        }

        private ObservableCollection<UserItemViewModel> users = new ObservableCollection<UserItemViewModel>();
        public ObservableCollection<UserItemViewModel> Users
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

        private ObservableCollection<PostItemViewModel> posts = new ObservableCollection<PostItemViewModel>();
        public ObservableCollection<PostItemViewModel> Posts
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

                if (results.Object.users != null)
                    foreach (SearchUser user in results.Object.users)
                    {
                        Users.Add(new UserItemViewModel(user.id, user.username, "http://d.sparklr.me/i/t" + user.id + ".jpg"));
                    }

                if (results.Object.posts != null)
                    foreach (SearchPost post in results.Object.posts)
                    {
                        Posts.Add(new PostItemViewModel(post.id));
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
