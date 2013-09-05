using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using System;
using System.ComponentModel;
using System.Windows;

namespace SparklrWP
{
    public class ChatMessageModel : INotifyPropertyChanged
    {
        public ChatMessageModel()
        {
        }

        private bool _currentUser = false;
        public bool CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    NotifyPropertyChanged("CurrentUser");
                }
            }
        }

        private int _from;
        public int From
        {
            get
            {
                return _from;
            }
            set
            {
                if (value != _from)
                {
                    _from = value;
                    NotifyPropertyChanged("From");
                }
            }
        }

        private int _to;
        public int To
        {
            get
            {
                return _to;
            }
            set
            {
                if (value != _to)
                {
                    _to = value;
                    NotifyPropertyChanged("To");
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

        private int _time;
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value != _time)
                {
                    _time = value;
                    NotifyPropertyChanged("Time");
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
    }

    public class ChatViewModel : INotifyPropertyChanged
    {
        public ChatViewModel()
        {
        }

        public event EventHandler LoadingFinished;

        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private int _from;
        public int From
        {
            get
            {
                return _from;
            }
            set
            {
                if (value != _from)
                {
                    _from = value;
                    Image = "http://d.sparklr.me/i/t" + From + ".jpg";
                    NotifyPropertyChanged("From");
                    updateName();
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

        public void LoadMessages()
        {
            loadMessages();
        }

        private async void loadMessages()
        {
            JSONRequestEventArgs<Chat[]> result = await App.Client.GetChatAsync(From);

            if (result.IsSuccessful)
            {
                Messages.Clear();

                foreach (Chat i in result.Object)
                {
                    Messages.Add(new ChatMessageModel()
                    {
                        From = i.from,
                        To = i.to,
                        CurrentUser = i.from == App.Client.UserId,
                        Message = i.message,
                        Time = i.time
                    });
                }

                if (LoadingFinished != null)
                    LoadingFinished(this, null);
            }
            else
            {
                MessageBox.Show("We're having trouble talking with sparklr. Please try again later.");
            }
        }

        private async void updateName()
        {
            int updated = _from;
            JSONRequestEventArgs<Username[]> result = await App.Client.GetUsernamesAsync(new int[] { updated });

            if (result.IsSuccessful && From == updated && result.Object.Length > 0)
            {
                Name = result.Object[0].username;
            }
        }

        private ObservableCollectionWithItemNotification<ChatMessageModel> messages = new ObservableCollectionWithItemNotification<ChatMessageModel>();
        public ObservableCollectionWithItemNotification<ChatMessageModel> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                if (messages != value)
                {
                    messages = value;
                    NotifyPropertyChanged("Messages");
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
    }
}
