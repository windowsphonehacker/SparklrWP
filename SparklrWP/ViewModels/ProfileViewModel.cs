extern alias ImageToolsDLL;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SparklrWP
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private string _handle;
        private string _profileImage;
        private string _backgroundImage;
        private string _bio;
        private ImageSource _profileImageSource;
        private int _id;

        //todo: implement properly
        private ObservableCollection<ItemViewModel> active = new ObservableCollection<ItemViewModel>();
        private ObservableCollection<ItemViewModel> posts = new ObservableCollection<ItemViewModel>();
        private ObservableCollection<ItemViewModel> mentions = new ObservableCollection<ItemViewModel>();
        private ObservableCollection<ItemViewModel> photos = new ObservableCollection<ItemViewModel>();

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
                    updateProfileImage();
                }
            }
        }

        public ImageSource ProfileImageSource
        {
            get
            {
                return _profileImageSource;
            }
            set
            {
                if (_profileImageSource != value)
                {
                    _profileImageSource = value;
                    NotifyPropertyChanged("ProfileImageSource");
                }
            }
        }

        public ObservableCollection<ItemViewModel> Posts
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

        public ObservableCollection<ItemViewModel> Mentions
        {
            get
            {
                return mentions;
            }
            set
            {
                if (mentions != value)
                {
                    mentions = value;
                    NotifyPropertyChanged("Mentions");
                }
            }
        }

        public ObservableCollection<ItemViewModel> Photos
        {
            get
            {
                return photos;
            }
            set
            {
                if (photos != value)
                {
                    photos = value;
                    NotifyPropertyChanged("Photos");
                }
            }
        }

        //Todo: implement properly
        public ObservableCollection<ItemViewModel> Active
        {
            get
            {
                return active;
            }
            set
            {
                if (active != value)
                {
                    active = value;
                    NotifyPropertyChanged("Active");
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

        private async void updateProfileImage()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                ProfileImageSource = new BitmapImage(new Uri(ProfileImage));
            }
            else
            {
                ProfileImageSource = (BitmapImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(ProfileImage);
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