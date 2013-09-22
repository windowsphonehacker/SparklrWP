using SparklrLib.Objects;
using SparklrLib.Objects.Responses;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using System;
using System.ComponentModel;
using System.Windows;

namespace SparklrWP.ViewModels
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

    public sealed class ChatViewModel : INotifyPropertyChanged, IDisposable
    {
        PeriodicTimer chatUpdater;
        private const int updateInterval = 2000;
        int lastTime = 0;
        public event EventHandler LoadingFinished;

        public ChatViewModel()
        {
            chatUpdater = new PeriodicTimer(updateInterval, false);
            chatUpdater.TimeoutElapsed += chatUpdater_TimeoutElapsed;
            loadMessages();
        }

        void chatUpdater_TimeoutElapsed(object sender, EventArgs e)
        {
            updateMessages();
        }

        private async void updateMessages()
        {
            GlobalLoading.Instance.IsLoading = true;

            App.SuppressNotifications = true;
            JSONRequestEventArgs<SparklrLib.Objects.Responses.Beacon.Chat> response = await App.Client.GetBeaconChatAsync(From, lastTime);
            App.SuppressNotifications = false;

            if (response.IsSuccessful)
            {
                foreach (SparklrLib.Objects.Responses.Beacon.ChatMessage i in response.Object.data)
                {
                    AddMessage(i);
                }
            }

            GlobalLoading.Instance.IsLoading = false;
        }

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

        private string _message = "";
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (Message != value)
                {
                    _message = value;
                    NotifyPropertyChanged("Message");
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
                    AddMessage(i);
                }

                //TODO: prevent the last message from showing again

                //Start automatic updates
                chatUpdater.Start();

                if (LoadingFinished != null)
                    LoadingFinished(this, null);
            }
            else
            {
                MessageBox.Show("We're having trouble talking with sparklr. Please try again later.");
            }
        }

        private void AddMessage(Chat item)
        {
            ChatMessageModel m = new ChatMessageModel()
            {
                From = item.from,
                To = item.to,
                CurrentUser = item.from == App.Client.UserId,
                Message = item.message,
                Time = item.time
            };

            insertItem(m);

            if (item.time > lastTime)
            {
                lastTime = item.time;
            }
        }

        private void AddMessage(SparklrLib.Objects.Responses.Beacon.ChatMessage item)
        {
            ChatMessageModel m = new ChatMessageModel()
            {
                From = item.from,
                To = item.to,
                CurrentUser = item.from == App.Client.UserId,
                Message = item.message,
                Time = item.time
            };

            insertItem(m);

            if (item.time > lastTime)
            {
                lastTime = item.time;
            }
        }

        private void insertItem(ChatMessageModel m)
        {
            if (Messages.Count == 0)
            {
                SmartDispatcher.BeginInvoke(() =>
                            {
                                Messages.Add(m);
                            });
            }
            else
            {
                for (int i = 0; i < Messages.Count; i++)
                {
                    if (Messages[i].Time < m.Time)
                    {
                        SmartDispatcher.BeginInvoke(() =>
                            {
                                Messages.Insert(i, m);
                            });
                        break;
                    }
                    else if (i + 1 == Messages.Count)
                    {
                        SmartDispatcher.BeginInvoke(() =>
                            {
                                Messages.Add(m);
                            });
                        break;
                    }
                }
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
                SmartDispatcher.BeginInvoke(() =>
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        public async void sendMessage()
        {
            if (!String.IsNullOrEmpty(Message))
            {
                GlobalLoading.Instance.IsLoading = true;

                JSONRequestEventArgs<Generic> result = await App.Client.PostChatMessageAsync(From, Message);

                if (result.IsSuccessful)
                {
                    Message = "";
                }
                else
                {
                    Helpers.Notify("Couldnt send your message :(");
                }

                GlobalLoading.Instance.IsLoading = false;
            }
            else
            {
                Helpers.Notify("Don't be so shy. Say something ;)");
            }

        }

        public void Dispose()
        {
            if (chatUpdater != null)
                chatUpdater.Dispose();
        }
    }
}
