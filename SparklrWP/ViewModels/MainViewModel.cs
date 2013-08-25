using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;


namespace SparklrWP
{
    public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The streamUpdater starts stream updates every 10 seconds.
        /// </summary>
        Timer streamUpdater;

        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();

            streamUpdater = new Timer(streamUpdater_Tick, null, 10000, Timeout.Infinite);

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

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public int LastTime = 1377357375;

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        /// <param name="stopTimer">Indicates if the internal timer should be stopped</param>
        private void loadData(bool stopTimer = true)
        {
            //Stop the updater, to prevent multiple requests
            if (stopTimer)
                streamUpdater.Change(10000, Timeout.Infinite);

            GlobalLoading.Instance.IsLoading = true;

            App.Client.BeginRequest(loadCallback,
#if DEBUG
 "beacon/stream/2?since=" + LastTime + "&n=0&network=1" //Development network
#else
 "beacon/stream/0?since="+LastTime+"&n=0"
#endif
                );
        }

        private bool loadCallback(string result)
        {
            if (result == null || result == "")
            {
                GlobalLoading.Instance.IsLoading = false;
                streamUpdater.Change(10000, Timeout.Infinite);
                return false;
            }

            SparklrLib.Objects.Responses.Beacon.Stream stream = JsonConvert.DeserializeObject<SparklrLib.Objects.Responses.Beacon.Stream>(result);


            if (stream != null && stream.notifications != null)
            {
                foreach (var not in stream.notifications)
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
                }
            }

            if (stream == null || stream.data == null)
            {
                GlobalLoading.Instance.IsLoading = false;
                streamUpdater.Change(10000, Timeout.Infinite);
                return true;
            }
            int count = stream.data.length;
            foreach (var t in stream.data.timeline)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                  {
                      ItemViewModel existingitem = null;
                      try
                      {
                          existingitem = (from i in this.Items where i.Id == t.id select i).First();
                      }
                      catch (Exception) { }
                      if (existingitem == null)
                      {
                          existingitem = new ItemViewModel(t.id) { Message = t.message, CommentCount = (t.commentcount == null ? 0 : (int)t.commentcount), From = t.from.ToString() };
                          if (t.meta != null)
                          {
                              existingitem.ImageUrl = "http://d.sparklr.me/i/t" + t.meta;
                          }
                          this.Items.Add(existingitem);
                      }
                      else
                      {
                          existingitem.Message = t.message;
                          existingitem.CommentCount = (t.commentcount == null ? 0 : (int)t.commentcount);
                          existingitem.From = t.from.ToString(); //TODO: Use /work/username to get the user names
                      }
                  });
                if (LastTime < t.time)
                {
                    LastTime = t.time;
                }
                if (LastTime < t.modified)
                {
                    LastTime = t.modified;
                }
            }
            GlobalLoading.Instance.IsLoading = false;
            this.IsDataLoaded = true;
            streamUpdater.Change(10000, Timeout.Infinite);
            return this.IsDataLoaded;
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