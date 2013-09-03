using SparklrLib.Objects;
using SparklrWP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;

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
            this.Items = new ObservableCollectionWithItemNotification<ItemViewModel>();
            GroupedItems = (new ObservableCollectionWithItemNotification<FriendViewModel>()).GroupFriends();

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
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                loadData();
            });
        }

        private ObservableCollectionWithItemNotification<ItemViewModel> _items;
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollectionWithItemNotification<ItemViewModel> Items
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

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
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

            await import(args);

            GlobalLoading.Instance.IsLoading = false;
            streamUpdater.Change(10000, Timeout.Infinite);
        }

        private async System.Threading.Tasks.Task import(JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> args)
        {
            if (args.IsSuccessful)
            {
                SparklrLib.Objects.Responses.Beacon.Stream stream = args.Object;

                if (stream != null && stream.data != null)
                {
                    if (stream.notifications != null)
                    {
                        NewCount = stream.notifications.Count;

                        Notifications.Clear();
                        foreach (SparklrLib.Objects.Responses.Beacon.Notification n in stream.notifications)
                        {
                            Notifications.Add(new NotificationViewModel(n.id)
                            {
                                Message = n.body,
                                From = n.from
                            });
                        }
                    }

                    int count = stream.data.length;

                    foreach (var t in stream.data.timeline)
                    {
                        if (LastTime < t.time)
                        {
                            LastTime = t.time;
                        }
                        if (LastTime < t.modified)
                        {
                            LastTime = t.modified;
                        }

                        ItemViewModel existingitem = null;
                        existingitem = (from i in Items where i.Id == t.id select i).FirstOrDefault();

                        if (existingitem == null)
                        {
                            ItemViewModel newItem = new ItemViewModel(t.id) { Message = t.message, CommentCount = (t.commentcount == null ? 0 : (int)t.commentcount), From = t.from.ToString(), AuthorId = t.from, OrderTime = t.modified > t.time ? t.modified : t.time };
                            if (!String.IsNullOrEmpty(t.meta))
                            {
                                newItem.ImageUrl = "http://d.sparklr.me/i/t" + t.meta;
                            }

                            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> response = await App.Client.GetUsernamesAsync(new int[] { t.from });
                            if (response.IsSuccessful && response.Object[0] != null && !string.IsNullOrEmpty(response.Object[0].username))
                                newItem.From = response.Object[0].username;

                            addItem(newItem);
                        }
                        else
                        {
                            existingitem.Message = t.message;
                            existingitem.CommentCount = (t.commentcount == null ? 0 : (int)t.commentcount);
                            existingitem.From = t.from.ToString();
                            existingitem.OrderTime = t.modified > t.time ? t.modified : t.time;

                            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> response = await App.Client.GetUsernamesAsync(new int[] { t.from });

                            if (response.IsSuccessful && response.Object[0] != null && !string.IsNullOrEmpty(response.Object[0].username))
                                existingitem.From = response.Object[0].username;
                        }
                    }

                    this.IsDataLoaded = true;
#if DEBUG
                    foreach (ItemViewModel i in Items)
                    {
                        if (i.ImageUrl != null)
                            App.logger.log(i.ImageUrl);
                    }
#endif
                }
            }
        }

        private void addItem(ItemViewModel item)
        {
            if (Items == null)
                Items = new ObservableCollectionWithItemNotification<ItemViewModel>();

            if (BeforeItemAdded != null)
                BeforeItemAdded(this, null);

            if (Items.Count() == 0)
            {
                Items.Add(item);
            }
            else
            {
                int time = item.OrderTime;

                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i].OrderTime < time)
                    {
                        Items.Insert(i, item);
                        break;
                    }
                    else if (i + 1 == Items.Count())
                    {
                        Items.Add(item);
                        break;
                    }
                }
            }

            if (AfterItemAdded != null)
                AfterItemAdded(this, null);
        }

        public async void LoadMore()
        {
            streamUpdater.Change(Timeout.Infinite, Timeout.Infinite);
            GlobalLoading.Instance.IsLoading = true;

            //TODO: Implement properly
            JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> moreItems = await App.Client.GetMoreItems(LastTime);
            await import(moreItems);

            GlobalLoading.Instance.IsLoading = false;
            streamUpdater.Change(10000, Timeout.Infinite);
        }

        private async void loadFriends()
        {
            JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Friends> fargs = await App.Client.GetFriendsAsync();

            if (fargs.IsSuccessful)
            {
                List<int> friends = new List<int>();

                foreach (int id in fargs.Object.followers)
                {
                    friends.Add(id);
                }

                foreach (int id in fargs.Object.following)
                {
                    if (!friends.Contains(id)) friends.Add(id);
                }

                JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> uargs = await App.Client.GetUsernamesAsync(friends.ToArray());
                foreach (int id in friends)
                {
                    AddFriend(new FriendViewModel(id)
                    {
                        Name = App.Client.Usernames.ContainsKey(id) ? App.Client.Usernames[id] : "User " + id,
                        Image = "http://d.sparklr.me/i/t" + id + ".jpg"
                    });
                }

#if DEBUG
                foreach (var group in GroupedItems)
                {
                    if (group.HasItems)
                    {
                        App.logger.log("Group {0} has the following entries:", group.Title);
                        App.logger.log(String.Join<FriendViewModel>(", ", group.ToArray()));
                    }
                }
#endif
            }
        }

        ObservableCollectionWithItemNotification<GroupedObservableCollection<FriendViewModel>> _groupedItems;
        public ObservableCollectionWithItemNotification<GroupedObservableCollection<FriendViewModel>> GroupedItems
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
                    NotifyPropertyChanged("NewCount");
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

        public void AddFriend(FriendViewModel f)
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
    }
}