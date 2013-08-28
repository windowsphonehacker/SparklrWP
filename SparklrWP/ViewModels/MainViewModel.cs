using SparklrWP.Utils;
using System;
using System.Collections.Generic;
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
        Comparison<ItemViewModel> itemComparison = new Comparison<ItemViewModel>(
            (p, q) =>
            {
                if (p.OrderTime > q.OrderTime)
                    return 1;
                else if (p.OrderTime < q.OrderTime)
                    return -1;
                else
                    return 0;
            }
            );

        public MainViewModel()
        {
            this.Items = new ObservableCollectionWithItemNotification<ItemViewModel>();

            streamUpdater = new Timer(streamUpdater_Tick, null, Timeout.Infinite, Timeout.Infinite);

            //We do not start the updater here. It will be started by the callback of the reponse
            //Warning: Possible issue where a internet conenction is not stable

            loadData();
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

        //private int _newCount = 0;
        //public int NewCount
        //{
        //    get
        //    {
        //        return _newCount;
        //    }
        //    set
        //    {
        //        if (value != _newCount)
        //        {
        //            _newCount = value;
        //            NotifyPropertyChanged("NewCount");
        //            NotifyPropertyChanged("NewCountVisibility");
        //        }
        //    }
        //}

        //public Visibility NewCountVisibility
        //{
        //    get
        //    {
        //        return _newCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        //    }
        //}

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
        private void loadData(bool stopTimer = true)
        {
            //Stop the updater, to prevent multiple requests

            if (stopTimer)
                streamUpdater.Change(Timeout.Infinite, Timeout.Infinite);

            GlobalLoading.Instance.IsLoading = true;

            App.Client.GetBeaconStream(LastTime, (args) =>
            {
                if (args.IsSuccessful)
                {
                    SparklrLib.Objects.Responses.Beacon.Stream stream = args.Object;
                    if (stream != null && stream.notifications != null)
                    {
                        /*foreach (var not in stream.notifications)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                try
                                {
                                    if (MessageBox.Show("id: " + not.id + "\naction: " + not.action + "\ntype:" + not.type + "\nbody" + not.body, "Notification test", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        App.Client.BeginRequest(null, "work/delete/notification/" + not.id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK);
                                }
                            });
                        }*/
                    }

                    if (stream == null || stream.data == null)
                    {
                        GlobalLoading.Instance.IsLoading = false;
                        streamUpdater.Change(10000, Timeout.Infinite);
                        return;
                    }
                    int count = stream.data.length;

                    List<ItemViewModel> newItems = new List<ItemViewModel>(Items);
                    ManualResetEvent synchronize;

                    foreach (var t in stream.data.timeline)
                    {
                        synchronize = new ManualResetEvent(false);

                        if (LastTime < t.time)
                        {
                            LastTime = t.time;
                        }
                        if (LastTime < t.modified)
                        {
                            LastTime = t.modified;
                        }

                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            ItemViewModel existingitem = null;
                            existingitem = (from i in newItems where i.Id == t.id select i).FirstOrDefault();

                            if (existingitem == null)
                            {
                                ItemViewModel newItem = new ItemViewModel(t.id) { Message = t.message, CommentCount = (t.commentcount == null ? 0 : (int)t.commentcount), From = t.from.ToString(), OrderTime = t.modified > t.time ? t.modified : t.time };
                                if (!String.IsNullOrEmpty(t.meta))
                                {
                                    newItem.ImageUrl = "http://d.sparklr.me/i/t" + t.meta;
                                }

                                App.Client.GetUsernames(new int[] { t.from }, (response) =>
                                {
                                    if (response.IsSuccessful && response.Object[0] != null && !string.IsNullOrEmpty(response.Object[0].username))
                                        newItem.From = response.Object[0].username;
                                });

                                newItems.Add(newItem);
                            }
                            else
                            {
                                existingitem.Message = t.message;
                                existingitem.CommentCount = (t.commentcount == null ? 0 : (int)t.commentcount);
                                existingitem.From = t.from.ToString();
                                existingitem.OrderTime = t.modified > t.time ? t.modified : t.time;

                                App.Client.GetUsernames(new int[] { t.from }, (response) =>
                                {
                                    if (response.IsSuccessful && response.Object[0] != null && !string.IsNullOrEmpty(response.Object[0].username))
                                        existingitem.From = response.Object[0].username;
                                });
                            }

                            synchronize.Set();
                        });

                        synchronize.WaitOne();
                    }
                    newItems.Sort(itemComparison);
                    newItems.Reverse();

                    Items = new ObservableCollectionWithItemNotification<ItemViewModel>(newItems);
                    GlobalLoading.Instance.IsLoading = false;
                    this.IsDataLoaded = true;
                    streamUpdater.Change(10000, Timeout.Infinite);
                    return;
                }
            });
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
            //Dispose our disposable stuff

            if (streamUpdater != null)
                streamUpdater.Dispose();
        }
    }
}