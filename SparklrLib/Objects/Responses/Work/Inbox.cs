using System.Collections.Generic;

namespace SparklrLib.Objects.Responses.Work
{
    public class InboxItem
    {
        public int time { get; set; }
        public int from { get; set; }
        public string message { get; set; }
    }

    public class Inbox : List<InboxItem>
    {

    }
}
