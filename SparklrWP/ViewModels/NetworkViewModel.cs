using SparklrLib.Objects;
using SparklrWP.Utils;
using System;
using System.ComponentModel;
using System.Linq;

namespace SparklrWP.ViewModels
{
    public class NetworkViewModel : INotifyPropertyChanged
    {
        private PeriodicTimer updater;

        private int lastTime = 0;
        private int firstTime = int.MaxValue;

        public NetworkViewModel()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                throw new NotSupportedException("Use the NetworkViewModel(string) constructor instead");
            }
        }

        public NetworkViewModel(string name)
        {
            this.Name = name;
            updater = new PeriodicTimer(5000, false);
            updater.TimeoutElapsed += updater_TimeoutElapsed;
            loadPosts(true);
        }

        void updater_TimeoutElapsed(object sender, EventArgs e)
        {
            loadPosts();
        }

        private async void loadPosts(bool startTimer = false)
        {
            GlobalLoading.Instance.IsLoading = true;
            JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Stream> result = await App.Client.GetBeaconStreamAsync(NetworkHelpers.UnformatNetworkName(Name), lastTime);

            if (result.IsSuccessful)
            {
                import(result.Object.data);
            }
            GlobalLoading.Instance.IsLoading = false;

            if (startTimer)
                updater.Start();
        }

        private void import(SparklrLib.Objects.Responses.Beacon.Timeline[] data)
        {
            foreach (var t in data)
            {
                if (lastTime < t.time)
                {
                    lastTime = t.time;
                }
                if (t.modified != null && lastTime < (int)t.modified)
                {
                    lastTime = (int)t.modified;
                }

                if (firstTime > t.time)
                {
                    firstTime = t.time;
                }

                PostItemViewModel existingitem = null;
                existingitem = (from i in Posts where i.Id == t.id select i).FirstOrDefault();

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
        }

        public void Refresh()
        {
            updater.Stop();
            loadPosts(true);
        }

        private void addItem(PostItemViewModel item)
        {

            if (Posts.Count() == 0)
            {
                SmartDispatcher.BeginInvoke(() =>
                {
                    Posts.Add(item);
                });
            }
            else
            {
                int time = item.OrderTime;

                for (int i = 0; i < Posts.Count(); i++)
                {
                    if (Posts[i].OrderTime < time)
                    {
                        SmartDispatcher.BeginInvoke(() =>
                        {
                            Posts.Insert(i, item);
                        });
                        break;
                    }
                    else if (i + 1 == Posts.Count())
                    {
                        SmartDispatcher.BeginInvoke(() =>
                        {
                            Posts.Add(item);
                        });
                        break;
                    }
                }
            }
        }

        private ObservableCollectionWithItemNotification<PostItemViewModel> posts = new ObservableCollectionWithItemNotification<PostItemViewModel>();
        public ObservableCollectionWithItemNotification<PostItemViewModel> Posts
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

        private string name = "/";
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = NetworkHelpers.FormatNetworkName(value);
                    NotifyPropertyChanged("Name");
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

        public async void Track()
        {
            GlobalLoading.Instance.IsLoading = true;

            JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> result = await App.Client.TrackNetworkAsync(NetworkHelpers.UnformatNetworkName(this.Name));

            if (result.IsSuccessful)
            {
                if (!App.MainViewModel.TrackedNetworks.Contains(NetworkHelpers.FormatNetworkName(Name)))
                    App.MainViewModel.TrackedNetworks.Add(NetworkHelpers.FormatNetworkName(Name));

                Helpers.NotifyFormatted(String.Format("tracked {0}", NetworkHelpers.FormatNetworkName(Name)));
            }

            GlobalLoading.Instance.IsLoading = false;
        }

        public async void Untrack()
        {
            GlobalLoading.Instance.IsLoading = true;

            JSONRequestEventArgs<SparklrLib.Objects.Responses.Generic> result = await App.Client.UntrackNetworkAsync(NetworkHelpers.UnformatNetworkName(this.Name));

            if (result.IsSuccessful)
            {
                if (App.MainViewModel.TrackedNetworks.Contains(NetworkHelpers.FormatNetworkName(Name)))
                    App.MainViewModel.TrackedNetworks.Remove(NetworkHelpers.FormatNetworkName(Name));

                Helpers.NotifyFormatted(String.Format("untracked {0}", NetworkHelpers.FormatNetworkName(Name)));
            }

            GlobalLoading.Instance.IsLoading = false;
        }
    }
}
