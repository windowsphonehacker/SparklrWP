using System;
using System.ComponentModel;

namespace SparklrWP
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        public int Id { get; private set; }

        public NotificationViewModel(int Id)
        {
            this.Id = Id;
        }
        //For design:
        public NotificationViewModel()
        {

        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }
            set
            {
                if (value != _imageUrl)
                {
                    _imageUrl = value;
                    NotifyPropertyChanged("ImageUrl");
                }
            }
        }

        private int _from;
        public string FromName { get; private set; }
        public int From
        {
            get
            {
                return _from;
            }
            set
            {
                if (_from != value)
                {
                    _from = value;
                    NotifyPropertyChanged("From");
                    updateUsername();
                }
            }
        }

        private async void updateUsername()
        {
            SparklrLib.Objects.JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.User> result = await App.Client.GetUserAsync(From);
            FromName = result.IsSuccessful ? result.Object.name : String.Format("#{0}", From);
            NotifyPropertyChanged("FromName");
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