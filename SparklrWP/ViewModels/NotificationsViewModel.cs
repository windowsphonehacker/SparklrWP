using System;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace SparklrWP
{
    public sealed class NotificationsViewModel : INotifyPropertyChanged
    {
        public NotificationsViewModel()
        {
            this.Items = new ObservableCollection<NotificationViewModel>();
        }

        /// <summary>
        /// A collection for NotificationViewModel objects.
        /// </summary>
        public ObservableCollection<NotificationViewModel> Items { get; private set; }

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

        /// <summary>
        /// Creates and adds a few NotificationViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            GlobalLoading.Instance.IsLoading = true;
            this.IsDataLoaded = true;
            //NewCount = 19; //TODO: Fill this
            GlobalLoading.Instance.IsLoading = false;
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