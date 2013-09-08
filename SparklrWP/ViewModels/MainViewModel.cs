using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Beacon;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SparklrWP
{
    public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        PeriodicTimer streamUpdater;

        private const int updateInterval = 5000;

        public event EventHandler BeforeItemAdded;
        public event EventHandler AfterItemAdded;

        public MainViewModel()
        {
            Items = new ObservableCollectionWithItemNotification<PostItemViewModel>();
            GroupedItems = (new ObservableCollectionWithItemNotification<UserItemViewModel>()).GroupFriends();

            this.ProfileImage = "http://d.sparklr.me/i/" + App.Client.UserId + ".jpg";

            streamUpdater = new PeriodicTimer(updateInterval, false);
            streamUpdater.TimeoutElapsed += streamUpdater_TimeoutElapsed;

            //We do not start the updater here. It will be started by the callback of the reponse
            //Warning: Possible issue where a internet conenction is not stable

            loadData(true);
            loadFriends();
            loadUserDetails();
        }

        void streamUpdater_TimeoutElapsed(object sender, EventArgs e)
        {
            loadData(false);
        }

        private async void loadUserDetails()
        {
            JSONRequestEventArgs<User> result = await App.Client.GetUserAsync((int)App.Client.UserId);

            if (result.IsSuccessful)
            {
                this.About = result.Object.bio;
                this.Username = result.Object.name;
            }
        }

        private ObservableCollectionWithItemNotification<PostItemViewModel> _items;
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollectionWithItemNotification<PostItemViewModel> Items
        {
            get
            {
                return _items;
            }
            private set
            {
                if (_items != value)
                {
                    _items = value;

                    SmartDispatcher.BeginInvoke(() =>
                    {
                        NotifyPropertyChanged("Items");
                    });
                }
            }
        }

        private string _username;
        /// <summary>
        /// The name of the current logged in user
        /// </summary>
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != value)
                {
                    _username = value;

                    NotifyPropertyChanged("Username");
                }
            }
        }

        private string _about;
        /// <summary>
        /// The name of the current logged in user
        /// </summary>
        public string About
        {
            get
            {
                return _about;
            }
            set
            {
                if (_about != value)
                {
                    _about = value;

                    NotifyPropertyChanged("About");
                }
            }
        }

        private string _profileImage;
        /// <summary>
        /// The name of the current logged in user
        /// </summary>
        public string ProfileImage
        {
            get
            {
                return _profileImage;
            }
            set
            {
                if (_profileImage != value)
                {
                    _profileImage = value;

                    NotifyPropertyChanged("ProfileImage");
                }
            }
        }

        public void Update()
        {
            this.loadData(false);
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public int LastTime = 0;
        public int FirstTime = int.MaxValue;

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        /// <param name="startTimer">Indicates if the internal timer should be started</param>
        private async void loadData(bool startTimer)
        {
            GlobalLoading.Instance.IsLoading = true;

            JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> args = await App.Client.GetBeaconStreamAsync(LastTime);

            if (args.IsSuccessful)
                import(args.Object.data);

            GlobalLoading.Instance.IsLoading = false;

            if (startTimer)
            {
                streamUpdater.Start();
            }
        }

        private void import(SparklrLib.Objects.Responses.Beacon.Timeline[] data)
        {
            foreach (var t in data)
            {
                if (LastTime < t.time)
                {
                    LastTime = t.time;
                }
                if (t.modified != null && LastTime < (int)t.modified)
                {
                    LastTime = (int)t.modified;
                }

                if (FirstTime > t.time)
                {
                    FirstTime = t.time;
                }

                PostItemViewModel existingitem = null;
                existingitem = (from i in Items where i.Id == t.id select i).FirstOrDefault();

                if (existingitem == null)
                {
                    PostItemViewModel newItem = new PostItemViewModel(
                        t.id,
                        t.from,
                        t.message,
                        null,
                        null,
                        t.commentcount ?? 0,
                        null,
                        t.from == App.Client.UserId,
                        !String.IsNullOrEmpty(t.imageUrl) ? "http://d.sparklr.me/i/t" + t.imageUrl : null,
                        t.network,
                        t.modified ?? t.time,
                        t.time,
                        t.via);
                    newItem.FillNamesAndImages();
                    addItem(newItem);
                }
                else
                {
                    existingitem.UpdatePostInfo();
                }
            }

            this.IsDataLoaded = true;
#if DEBUG
            foreach (PostItemViewModel i in Items)
            {
                if (i.ImageUrl != null)
                    App.logger.log(i.ImageUrl);
            }
#endif
        }

        public void UpdateNotifications(Notification[] notifications)
        {
            if (notifications != null)
            {
                NewCount = notifications.Length;
                SmartDispatcher.BeginInvoke(() =>
                    {
                        Notifications.Clear();
                    });
                foreach (Notification n in notifications)
                {
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        Notifications.Add(new NotificationViewModel(n.id)
                        {
                            Message = n.body,
                            From = n.from
                        });
                    });
                }
            }
        }

        private void addItem(PostItemViewModel item)
        {
            if (Items == null)
                Items = new ObservableCollectionWithItemNotification<PostItemViewModel>();

            SmartDispatcher.BeginInvoke(() =>
                {
                    if (BeforeItemAdded != null)
                        BeforeItemAdded(this, null);
                });

            if (Items.Count() == 0)
            {
                SmartDispatcher.BeginInvoke(() =>
                    {
                        Items.Add(item);
                    });
            }
            else
            {
                int time = item.OrderTime;

                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i].OrderTime < time)
                    {
                        SmartDispatcher.BeginInvoke(() =>
                            {
                                Items.Insert(i, item);
                            });
                        break;
                    }
                    else if (i + 1 == Items.Count())
                    {
                        SmartDispatcher.BeginInvoke(() =>
                            {
                                Items.Add(item);
                            });
                        break;
                    }
                }
            }

            SmartDispatcher.BeginInvoke(() =>
                {
                    if (AfterItemAdded != null)
                        AfterItemAdded(this, null);
                });
        }

        public async void LoadMore()
        {
            streamUpdater.Stop();

            GlobalLoading.Instance.IsLoading = true;

            //TODO: Implement properly
            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Stream[]> moreItems = await App.Client.GetMoreItems(FirstTime);
            import(moreItems.Object);

            GlobalLoading.Instance.IsLoading = false;

            streamUpdater.Start();
        }

        private async void loadFriends()
        {
            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Friends> fargs = await App.Client.GetFriendsAsync();

            if (fargs.IsSuccessful)
            {
                List<int> friends = new List<int>();

                foreach (int id in fargs.Object)
                {
                    friends.Add(id);
                }

                JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> uargs = await App.Client.GetUsernamesAsync(friends.ToArray());
                foreach (int id in friends)
                {
                    string username = "";

                    foreach (Username u in uargs.Object)
                    {
                        if (id == u.id)
                        {
                            username = u.username;
                        }
                    }

                    AddFriend(new UserItemViewModel(id, username, "http://d.sparklr.me/i/t" + id + ".jpg"));
                }

#if DEBUG
                foreach (var group in GroupedItems)
                {
                    if (group.HasItems)
                    {
                        App.logger.log("Group {0} has the following entries:", group.Title);
                        App.logger.log(String.Join<UserItemViewModel>(", ", group.ToArray()));
                    }
                }
#endif
            }
        }

        ObservableCollectionWithItemNotification<GroupedObservableCollection<UserItemViewModel>> _groupedItems;
        public ObservableCollectionWithItemNotification<GroupedObservableCollection<UserItemViewModel>> GroupedItems
        {
            get
            {
                return _groupedItems;
            }
            set
            {
                if (_groupedItems != value)
                {
                    _groupedItems = value;
                    NotifyPropertyChanged("GroupedItems");
                }
            }
        }


        private int _newCount = 0;
        public int NewCount
        {
            get
            {
                return _newCount;
            }
            set
            {
                if (value != _newCount)
                {
                    _newCount = value;
                    SmartDispatcher.BeginInvoke(() =>
                        {
                            NotifyPropertyChanged("NewCount");
                        });
                }
            }
        }

        private ObservableCollection<NotificationViewModel> _notifications = new ObservableCollection<NotificationViewModel>();
        public ObservableCollection<NotificationViewModel> Notifications
        {
            get
            {
                return _notifications;
            }
            private set
            {
                if (_notifications != value)
                {
                    _notifications = value;
                    NotifyPropertyChanged("Notifications");
                }
            }
        }

        public void AddFriend(UserItemViewModel f)
        {
            GroupedItems.AddFriend(f);
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

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (streamUpdater != null)
                streamUpdater.Dispose();
        }

        public void RefreshFriends()
        {
            GroupedItems = (new ObservableCollectionWithItemNotification<UserItemViewModel>()).GroupFriends();
            loadFriends();
        }
    }
}