using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Sparklr
{
    /// <summary>
    /// Represents a type of a notification
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// The notification is a comment or like
        /// </summary>
        CommentOrLike = 1,
        /// <summary>
        /// The notification is for a mention
        /// </summary>
        Mention = 2,
        /// <summary>
        /// The notification is for a received message
        /// </summary>
        Message = 3
    }

    /// <summary>
    /// Represents a notification on the sparklr service
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// The ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The user that caused this notification
        /// </summary>
        public User From { get; private set; }

        /// <summary>
        /// Someone who is affected by this notification
        /// </summary>
        public User To { get; private set; }

        /// <summary>
        /// The nature of the notification
        /// </summary>
        public NotificationType Type { get; private set; }

        /// <summary>
        /// The timestamp
        /// </summary>
        public long TimeStamp { get; private set; }

        /// <summary>
        /// Some content, can be null
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// An action (i.e. post link), can be null
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// Indicates if this notification has been dismissed.
        /// </summary>
        public bool Dismissed { get; private set; }

        /// <summary>
        /// A verbose text representing this notification.
        /// Examples:   - "USER likes your post."
        ///             - "USER commented COMMENT"
        ///             - "USER mentioned you."
        ///             - "USER messaged you: MESSAGE"
        /// </summary>
        public string NotificationText
        {
            get
            {
                switch (Type)
                {
                    case NotificationType.CommentOrLike:
                        if (Body == "☝")
                        {
                            return String.Format("{0} likes your post.", From.Name);
                        }
                        else
                        {
                            return String.Format("{0} commented {1}", From.Name, Body);
                        }
                    case NotificationType.Mention:
                        return String.Format("{0} mentioned you.", From.Name);
                    case NotificationType.Message:
                        return String.Format("{0} messaged you: {1}", From.Name, Body);
                    default:
                        return "Unknown notification";
                }
            }
        }

        internal Notification(int id, User from, User to, NotificationType type, long timestamp, string body, string action)
        {
            this.Id = id;
            this.From = from;
            this.To = to;
            this.Type = type;
            this.TimeStamp = timestamp;
            this.Body = body;
            this.Action = action;
        }

        /// <summary>
        /// Marks the notification as read and dismisses it.
        /// </summary>
        /// TODO: Write UnitTests
        public async Task DismissAsync(Connection conn)
        {
            if (Dismissed)
                throw new InvalidOperationException("The notification has already been dismissed");

            await conn.DismissNotificationAsync(this);

            Dismissed = true;
        }
    }
}
