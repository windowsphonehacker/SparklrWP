using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SparklrWP
{
    public class TagViewModel : INotifyPropertyChanged
    {
        //TODO?: loading overlay?
        //TODO: implement "load more"
        //public event EventHandler InitialLoadingComplete;

        PeriodicTimer tagUpdater;
        int lasttime = 0;

        //Designtime support
        public TagViewModel()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                throw new NotSupportedException("Use the TagViewModel(string) constructor instead");
            }
        }

        public TagViewModel(string tag)
        {
            this.Tag = tag;
            tagUpdater = new PeriodicTimer(10000, false);
            tagUpdater.TimeoutElapsed += tagUpdater_TimeoutElapsed;
            LoadPosts();
        }

        private async void tagUpdater_TimeoutElapsed(object sender, EventArgs e)
        {
            GlobalLoading.Instance.IsLoading = true;

            JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Tag> result = await App.Client.GetBeaconTagAsync(this.Tag, lasttime);

            if (result.IsSuccessful)
            {
                foreach (SparklrLib.Objects.Responses.Beacon.Datum t in result.Object.data)
                {
                    if (lasttime < t.time)
                        lasttime = t.time;

                    PostItemViewModel p = new PostItemViewModel(
                            t.id,
                            t.from,
                            t.message,
                            null,
                            null,
                            t.commentcount ?? 0,
                            null,
                            false,
                            t.imageUrl,
                            t.network,
                            t.modified ?? t.time,
                            t.time,
                            t.via,
                            null,
                            null
                            );

                    insertPost(p);
                }
            }

            GlobalLoading.Instance.IsLoading = false;
        }

        private ObservableCollection<PostItemViewModel> posts = new ObservableCollection<PostItemViewModel>();
        /// <summary>
        /// Contains posts with this tag
        /// </summary>
        public ObservableCollection<PostItemViewModel> Posts
        {
            get
            {
                return posts;
            }
            private set
            {
                if (posts != value)
                {
                    posts = value;
                    NotifyPropertyChanged("Posts");
                }
            }
        }

        private string tag = "";
        /// <summary>
        /// The tag. Will retreive a leading # if it doesnt contain one. Will be converted to lowercase.
        /// </summary>
        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (tag != value)
                {
                    tag = value.StartsWith("#") ? value : "#" + value;
                    tag = tag.ToLowerInvariant();
                    NotifyPropertyChanged("Tag");
                }
            }
        }

        /// <summary>
        /// Loads/refreshes the posts
        /// </summary>
        public async void LoadPosts()
        {
            GlobalLoading.Instance.IsLoading = true;
            posts.Clear();

            JSONRequestEventArgs<Tag[]> result = await App.Client.GetTagPostsAsync(Tag.TrimStart('#'));

            if (result.IsSuccessful)
            {
                foreach (Tag t in result.Object)
                {
                    if (lasttime < t.time)
                        lasttime = t.time;

                    PostItemViewModel post = new PostItemViewModel(
                        t.id,
                        t.from,
                        t.message,
                        null,
                        null,
                        t.commentcount ?? 0,
                        null,
                        false,
                        t.imageUrl,
                        t.network,
                        t.modified ?? t.time,
                        t.time,
                        t.via,
                        null,
                        null
                        );

                    post.FillNamesAndImages();
                    insertPost(post);
                }
            }

            GlobalLoading.Instance.IsLoading = false;
        }

        private void insertPost(PostItemViewModel item)
        {
            if (Posts.Count == 0)
            {
                Posts.Add(item);
            }
            else
            {
                for (int i = 0; i < Posts.Count; i++)
                {
                    if (Posts[i].OrderTime < item.OrderTime)
                    {
                        Posts.Insert(i, item);
                        break;
                    }
                    else if (i + 1 == Posts.Count)
                    {
                        Posts.Add(item);
                        break;
                    }
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
    }
}
