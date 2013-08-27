using System;
using System.ComponentModel;

namespace SparklrWP
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private string _handle;
        private string _profileImage;
        private string _backgroundImage;
        private string _bio;
        private int _id;

        public string Handle
        {
            get
            {
                return _handle;
            }
            set
            {
                if (_handle != value)
                {
                    _handle = value;
                    NotifyPropertyChanged("Handle");
                }
            }
        }

        public string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                    NotifyPropertyChanged("BackgroundImage");
                }
            }
        }

        public string ProfileImage
        {
            get
            {
                return _profileImage;
            }
            set
            {
                if (_profileImage != value)
                {
                    _profileImage = value;
                    NotifyPropertyChanged("ProfileImage");
                }
            }
        }

        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        public string Bio
        {
            get
            {
                return _bio;
            }
            set
            {
                if (_bio != value)
                {
                    _bio = value;
                    NotifyPropertyChanged("Bio");
                }
            }
        }

        public ProfileViewModel()
        {

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