using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Utils;
using System;
using System.ComponentModel;

namespace SparklrWP.ViewModels
{
    public class ConversationModel : INotifyPropertyChanged
    {

        public ConversationModel(int Id)
        {
            this.From = Id;
        }

        //For design
        public ConversationModel()
        {
        }

        private int _from;
        public int From
        {
            get
            {
                return _from;
            }
            private set
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

        private async void updateName()
        {
            int updated = _from;
            JSONRequestEventArgs<Username[]> result = await App.Client.GetUsernamesAsync(new int[] { updated });

            if (result.IsSuccessful && From == updated && result.Object.Length > 0)
            {
                Name = result.Object[0].username;
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

    public class InboxViewModel : INotifyPropertyChanged
    {
        public InboxViewModel()
        {
        }

        public async void Load()
        {
            GlobalLoading.Instance.IsLoading = true;
            JSONRequestEventArgs<Inbox> result = await App.Client.GetInboxAsync();

            if (result.IsSuccessful)
            {
                Conversations.Clear();

                foreach (InboxItem i in result.Object)
                {
                    Conversations.Add(new ConversationModel(i.from)
                        {
                            Message = i.message,
                            Time = i.time
                        });
                }
            }
            else
            {
                Helpers.Notify("We could not refresh your inbox :( Are you connected to the internet?");
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        private ObservableCollectionWithItemNotification<ConversationModel> conversations = new ObservableCollectionWithItemNotification<ConversationModel>();
        public ObservableCollectionWithItemNotification<ConversationModel> Conversations
        {
            get
            {
                return conversations;
            }
            set
            {
                if (conversations != value)
                {
                    conversations = value;
                    NotifyPropertyChanged("Conversations");
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
