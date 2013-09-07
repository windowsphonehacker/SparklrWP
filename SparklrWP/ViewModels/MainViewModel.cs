using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Beacon;
using SparklrWP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace SparklrWP
{
    public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The streamUpdater starts stream updates every 10 seconds.
        /// </summary>
        Timer streamUpdater;
        public event EventHandler BeforeItemAdded;
        public event EventHandler AfterItemAdded;

        public MainViewModel()
        {
            Items = new ObservableCollectionWithItemNotification<PostItemViewModel>();
            GroupedItems = (new ObservableCollectionWithItemNotification<UserItemViewModel>()).GroupFriends();

            streamUpdater = new Timer(streamUpdater_Tick, null, Timeout.Infinite, Timeout.Infinite);

            //We do not start the updater here. It will be started by the callback of the reponse
            //Warning: Possible issue where a internet conenction is not stable

            loadData();
            loadFriends();
        }

        /// <summary>
        /// Occures when the streamUpdater elapses
        /// </summary>
        /// <param name="state"></param>
        void streamUpdater_Tick(object state)
        {
            //The streamUpdater is stopped in loadData to prevent multiple requests
            loadData();
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

        public void Update()
        {
            this.loadData(true);
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public int LastTime = 0;

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        /// <param name="stopTimer">Indicates if the internal timer should be stopped</param>
        private async void loadData(bool stopTimer = true)
        {
            //Stop the updater, to prevent multiple requests

            if (stopTimer)
                streamUpdater.Change(Timeout.Infinite, Timeout.Infinite);

            GlobalLoading.Instance.IsLoading = true;

            JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> args = await App.Client.GetBeaconStreamAsync(LastTime);

            import(args);

            GlobalLoading.Instance.IsLoading = false;
            streamUpdater.Change(10000, Timeout.Infinite);
        }

        private void import(JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> args)
        {
            if (args.IsSuccessful)
            {
                SparklrLib.Objects.Responses.Beacon.Stream stream = args.Object;

                if (stream != null && stream.data != null)
                {
                    foreach (var t in stream.data)
                    {
                        if (LastTime < t.time)
                        {
                            LastTime = t.time;
                        }
                        if (t.modified != null && LastTime < (int)t.modified)
                        {
                            LastTime = (int)t.modified;
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
            }
        }

        public void UpdateNotifications(Notification[] notifications)
        {
            if (notifications != null)
            {
                NewCount = notifications.Length;

                Notifications.Clear();
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
            streamUpdater.Change(Timeout.Infinite, Timeout.Infinite);
            GlobalLoading.Instance.IsLoading = true;

            //TODO: Implement properly
            JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> moreItems = await App.Client.GetMoreItems(LastTime);
            import(moreItems);

            GlobalLoading.Instance.IsLoading = false;
            streamUpdater.Change(10000, Timeout.Infinite);
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
                    AddFriend(new UserItemViewModel(id, App.Client.Usernames[id], "http://d.sparklr.me/i/t" + id + ".jpg"));
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