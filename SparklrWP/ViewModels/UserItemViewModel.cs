using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using System;
using System.ComponentModel;

namespace SparklrWP
{
    public class UserItemViewModel : INotifyPropertyChanged
    {
        public UserItemViewModel(int Id)
        {
            this.Id = Id;
            this.Image = "http://d.sparklr.me/i/t" + Id + ".jpg";
            loadUserdata();
        }

        private async void loadUserdata()
        {
            JSONRequestEventArgs<Username[]> result = await App.Client.GetUsernamesAsync(new int[] { Id });

            if (result.IsSuccessful && result.Object.Length > 0)
            {
                this.Name = result.Object[0].username;
            }
        }

        public UserItemViewModel(int Id, string Name = null, string Image = null, bool isOnline = false)
        {
            this.Id = Id;
            this.Name = Name;
            this.Image = Image;
            this.IsOnline = isOnline;
        }

        //For design
        public UserItemViewModel()
        {
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            private set
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
            private set
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
            private set
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
