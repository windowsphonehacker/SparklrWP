using SparklrLib.Objects.Responses.Beacon;
using SparklrWP.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SparklrWP.ViewModels
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
        private ObservableCollection<NotificationViewModel> _notifications = new ObservableCollection<NotificationViewModel>();
        public ObservableCollection<NotificationViewModel> Notifications
        {
            get
            {
                return _notifications;
            }
            private set
            {
                if (_notifications != value)
                {
                    _notifications = value;
                    NotifyPropertyChanged("Notifications");
                }
            }
        }
        public async void UpdateNotifications(Notification[] notifications)
        {
            if (notifications != null)
            {
                NewCount = notifications.Length;

                SmartDispatcher.BeginInvoke(() =>
                {
                    Notifications.Clear();
                });

                foreach (Notification n in notifications)
                {
                    string message = await SparklrWP.Utils.NotificationHelpers.Format(n.type, n.body, n.from, App.Client);

                    Uri navigationUri = null;

                    navigationUri = NotificationHelpers.GenerateActionUri(n);

                    SmartDispatcher.BeginInvoke(() =>
                    {
                        Notifications.Add(new NotificationViewModel(n.id)
                        {
                            //Replacing it with the above assignment will crash the compiler
                            Message = message,
                            From = n.from,
                            NavigationUri = navigationUri
                        });
                    });
                }
            }
        }
        private int _newCount = 0;
        public int NewCount
        {
            get
            {
                return _newCount;
            }
            set
            {
                if (value != _newCount)
                {
                    _newCount = value;
                    SmartDispatcher.BeginInvoke(() =>
                    {
                        NotifyPropertyChanged("NewCount");
                    });
                }
            }
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

        public Uri NavigationUri;

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