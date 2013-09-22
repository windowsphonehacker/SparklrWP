extern alias ImageToolsDLL;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SparklrWP.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private string _handle;
        private string _profileImage;
        private string _backgroundImage;
        private string _bio;
        private ImageSource _profileImageSource;
        private int _id;
        private bool _following;

        //todo: implement properly
        private ObservableCollection<PostItemViewModel> active = new ObservableCollection<PostItemViewModel>();
        private ObservableCollection<PostItemViewModel> posts = new ObservableCollection<PostItemViewModel>();
        private ObservableCollection<PostItemViewModel> mentions = new ObservableCollection<PostItemViewModel>();
        private ObservableCollection<PostItemViewModel> photos = new ObservableCollection<PostItemViewModel>();

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

        public string FollowButtonCaption
        {
            get
            {
                return _following ? "unfollow" : "follow";
            }
        }

        public bool Following
        {
            get
            {
                return _following;
            }
            set
            {
                if (_following != value)
                {
                    _following = value;
                    NotifyPropertyChanged("Following");
                    NotifyPropertyChanged("FollowButtonCaption");
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

        public ObservableCollection<PostItemViewModel> Posts
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

        public ObservableCollection<PostItemViewModel> Mentions
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

        public ObservableCollection<PostItemViewModel> Photos
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
        public ObservableCollection<PostItemViewModel> Active
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