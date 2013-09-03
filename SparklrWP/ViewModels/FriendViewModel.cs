using System;
using System.ComponentModel;

namespace SparklrWP
{
    public class FriendViewModel : INotifyPropertyChanged
    {
        public FriendViewModel(int Id)
        {
            this.Id = Id;
        }

        //For design
        public FriendViewModel()
        {
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        private string _image;
        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (value != _image)
                {
                    _image = value;
                    NotifyPropertyChanged("Image");
                }
            }
        }
        private bool _isOnline;
        public bool IsOnline
        {
            get
            {
                return _isOnline;
            }
            set
            {
                if (value != _isOnline)
                {
                    _isOnline = value;
                    NotifyPropertyChanged("IsOnline");
                }
            }
        }
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            private set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
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

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(Name))
                return Name;
            else
                return Id.ToString();
        }
    }
}
