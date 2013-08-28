using SparklrWP.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SparklrWP
{
    public class FriendsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FriendViewModel> _items;
        public ObservableCollection<FriendViewModel> Items
        {
            get
            {
                return new ObservableCollection<FriendViewModel>(_items);
            }
            /*set
            {
                if (_items != value)
                {
                    _items = value;
                    GroupedItems = _items.GroupFriends();
                    NotifyPropertyChanged("Items");
                    NotifyPropertyChanged("GroupedItems");
                }
            }*/
        }

        public void AddFriend(FriendViewModel f)
        {
            _items.Add(f);
            GroupedItems.AddFriend(f);

        }

        public ObservableCollection<GroupedObservableCollection<FriendViewModel>> GroupedItems { get; private set; }

        public FriendsViewModel()
        {
            _items = new ObservableCollection<FriendViewModel>();
            GroupedItems = _items.GroupFriends();

            NotifyPropertyChanged("GroupedItems");
            NotifyPropertyChanged("Items");
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
