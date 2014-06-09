
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace SparklrForWindowsPhone.ViewModels
{
    public class MessagesViewModel : ViewModelBase
    {
        private const int DefaultUserId = 4;
        private ObservableCollection<CustomMessage> messages;
        private ObservableCollection<Person> people;
        private Person you;
        private Person conversationBuddy;
        private int currentGroup = 0;
        private CustomMessage previousMessage;

        private void InitializeMessages()
        {
            this.messages = new ObservableCollection<CustomMessage>();
            this.messages.CollectionChanged += this.OnMessagesCollectionChanged;
            for (int i = 1; i <= 4; i++)
            {
                CustomMessage message;
                message = new CustomMessage("Message" + (i * 3 - 2),
                    DateTime.Now.AddMinutes(-5 + i),
                    ConversationViewMessageType.Outgoing,
                    You != null ? You.PersonId : DefaultUserId);
                this.messages.Add(message);
                message = new CustomMessage("Message" + (i * 3 - 1), DateTime.Now.AddMinutes(-5 + i), ConversationViewMessageType.Incoming, i);
                this.messages.Add(message);
                message = new CustomMessage("Message" + (i * 3), DateTime.Now.AddMinutes(-5 + i), ConversationViewMessageType.Incoming, i);
                this.messages.Add(message);
            }
        }

        private void OnMessagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                return;
            }
            CustomMessage message = e.NewItems[0] as CustomMessage;
            if (previousMessage != null)
            {
                if (previousMessage.SenderId != message.SenderId)
                {
                    this.currentGroup++;
                }
            }
            if (message.Group == null)
            {
                message.Group = this.currentGroup;
            }
            previousMessage = message;
        }

        private void InitializePeople()
        {
            this.people = new ObservableCollection<Person>();

            for (int i = 1; i <= 5; i++)
            {
                Person person = new Person() { PersonId = i, Name = "PERSON " + i, Picture = new Uri("Images/FrameThumbnail.png", UriKind.RelativeOrAbsolute) };
                this.people.Add(person);
            }
        }

        public Person You
        {
            get
            {
                return this.you;
            }
            set
            {
                this.you = value;
                this.OnPropertyChanged("You");
            }
        }

        public Person ConversationBuddy
        {
            get
            {
                return this.conversationBuddy;
            }
            set
            {
                this.conversationBuddy = value;
                this.OnPropertyChanged("ConversationBuddy");
            }
        }

        public ObservableCollection<CustomMessage> Messages
        {
            get
            {
                if (this.messages == null)
                {
                    this.InitializeMessages();
                }
                return this.messages;
            }
            private set
            {
                this.messages = value;
            }
        }

        public ObservableCollection<Person> People
        {
            get
            {
                if (this.people == null)
                {
                    this.InitializePeople();
                }
                return this.people;
            }
            private set
            {
                this.people = value;
            }
        }
    }

    public class CustomMessage : ConversationViewMessage, IComparable
    {
        public CustomMessage(string text, DateTime timeStamp, ConversationViewMessageType type, int senderId, int? group = null)
            : base(text, timeStamp, type)
        {
            this.SenderId = senderId;
            this.Group = group;
        }

        public int SenderId
        {
            get;
            private set;
        }

        public int? Group
        {
            get;
            set;
        }

        public SolidColorBrush MessageBackground
        {
            get
            {
                int id = this.SenderId % 6;
                switch (id)
                {
                    case 0: return new SolidColorBrush(Color.FromArgb(255, 51, 153, 51));
                    case 1: return new SolidColorBrush(Color.FromArgb(255, 27, 161, 226));
                    case 2: return new SolidColorBrush(Color.FromArgb(255, 255, 0, 151));
                    case 3: return new SolidColorBrush(Color.FromArgb(255, 240, 150, 9));
                    case 4: return new SolidColorBrush(Color.FromArgb(255, 0, 171, 169));
                    case 5: return new SolidColorBrush(Color.FromArgb(255, 140, 191, 38));
                }
                return App.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
            }
        }

        public string FormattedTimeStamp
        {
            get
            {
                return this.TimeStamp.ToShortTimeString();
            }
        }

        public override bool Equals(object obj)
        {
            CustomMessage secondMessage = obj as CustomMessage;

            if (obj is DataGroup)
            {
                secondMessage = (obj as DataGroup).Key as CustomMessage;
            }

            return this.Group == secondMessage.Group;
        }

        public override int GetHashCode()
        {
            return this.Group.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            CustomMessage targetMessage = obj as CustomMessage;

            if (targetMessage != null)
            {
                return this.Group > targetMessage.Group ? 1 : this.Group == targetMessage.Group ? 0 : -1;
            }

            if (obj is DataGroup)
            {
                targetMessage = (obj as DataGroup).Key as CustomMessage;

                return this.Group > targetMessage.Group ? 1 : this.Group == targetMessage.Group ? 0 : -1;
            }

            return 0;
        }
    }

    public class Person : ViewModelBase
    {
        private int personId;
        private string name;
        private Uri picture;

        public int PersonId
        {
            get
            {
                return this.personId;
            }
            set
            {
                if (this.personId != value)
                {
                    this.personId = value;
                    this.OnPropertyChanged("PersonId");
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        public Uri Picture
        {
            get
            {
                return this.picture;
            }
            set
            {
                if (this.picture != value)
                {
                    this.picture = value;
                    this.OnPropertyChanged("Picture");
                }
            }
        }
    }
}
