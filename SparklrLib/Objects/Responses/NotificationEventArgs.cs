using SparklrLib.Objects.Responses.Beacon;
using System;

namespace SparklrLib.Objects.Responses
{
    public class NotificationEventArgs : EventArgs
    {
        public Notification[] Notifications { get; private set; }

        public NotificationEventArgs(Notification[] notifications)
        {
            this.Notifications = notifications;
        }
    }
}
