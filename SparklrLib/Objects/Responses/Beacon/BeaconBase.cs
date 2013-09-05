
namespace SparklrLib.Objects.Responses.Beacon
{
    public class Notification
    {
        public int id { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public int type { get; set; }
        public int time { get; set; }
        public string body { get; set; }
        public string action { get; set; }
    }

    public abstract class BeaconBase
    {
        private Notification[] _notifications;
        public Notification[] notifications
        {
            get
            {
                return _notifications;
            }
            set
            {
                if (_notifications != value)
                {
                    _notifications = value;

                    if (_notifications != null && _notifications.Length > 0)
                    {
                        SparklrClient.RaiseNotificationReceived(this, new NotificationEventArgs(_notifications));
                    }
                }
            }
        }
    }
}
