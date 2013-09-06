using SparklrLib.Objects;
using SparklrLib.Objects.Responses.Work;
using System;
using System.ComponentModel;

namespace SparklrWP.ViewModels
{
    public class CommentModel : INotifyPropertyChanged
    {
        public int ID { get; private set; }

        public CommentModel(int id)
        {
            this.ID = id;
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
                    updateName();
                }
            }
        }

        private async void updateName()
        {
            JSONRequestEventArgs<Username[]> result = await App.Client.GetUsernamesAsync(new int[] { From });

            if (result.IsSuccessful && result.Object.Length > 0)
            {
                AuthorName = result.Object[0].username;
            }
        }

        private string _authorName;
        /// <summary>
        /// Contains the friendly name of the author.
        /// </summary>
        public string AuthorName
        {
            get
            {
                return _authorName;
            }
            private set
            {
                if (value != _authorName)
                {
                    _authorName = value;
                    NotifyPropertyChanged("AuthorName");
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

        private bool _deletable;
        /// <summary>
        /// Indicates if the post is deletable
        /// </summary>
        public bool Deletable
        {
            get
            {
                return _deletable;
            }
            set
            {
                if (value != _deletable)
                {
                    _deletable = value;
                    NotifyPropertyChanged("Deletable");
                }
            }
        }
    }
}
