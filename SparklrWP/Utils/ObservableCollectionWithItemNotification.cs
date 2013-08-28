using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace SparklrWP.Utils
{
    public class ObservableCollectionWithItemNotification<T> : ObservableCollection<T>
    {
        public ObservableCollectionWithItemNotification(IEnumerable<T> collection)
            : base(collection)
        {
            this.CollectionChanged += ObservableCollectionWithItemNotification_CollectionChanged;
            foreach (INotifyPropertyChanged item in collection)
                item.PropertyChanged += item_PropertyChanged;
        }

        public ObservableCollectionWithItemNotification(List<T> collection)
            : base(collection)
        {
            this.CollectionChanged += ObservableCollectionWithItemNotification_CollectionChanged;
            foreach (INotifyPropertyChanged item in collection)
                item.PropertyChanged += item_PropertyChanged;
        }

        public ObservableCollectionWithItemNotification()
        {
            this.CollectionChanged += ObservableCollectionWithItemNotification_CollectionChanged;
        }

        void ObservableCollectionWithItemNotification_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e != null)
            {
                if (e.OldItems != null)
                    foreach (INotifyPropertyChanged item in e.OldItems)
                        item.PropertyChanged -= item_PropertyChanged;

                if (e.NewItems != null)
                    foreach (INotifyPropertyChanged item in e.NewItems)
                        item.PropertyChanged += item_PropertyChanged;
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var reset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                        this.OnCollectionChanged(reset);
                    });
        }
    }
}
