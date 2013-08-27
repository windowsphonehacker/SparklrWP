using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SparklrWP
{
    public class FriendsViewModel :INotifyPropertyChanged 
    {
        public ObservableCollection<FriendViewModel> Items {get;set;}

        public FriendsViewModel(){
            Items = new ObservableCollection<FriendViewModel>();
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
