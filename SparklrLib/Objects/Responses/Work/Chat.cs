
using System.Collections.Generic;
namespace SparklrLib.Objects.Responses.Work
{
    public class ChatItem
    {
        public int to { get; set; }
        public int from { get; set; }
        public int time { get; set; }
        public string message { get; set; }
    }

    public class Chat : List<ChatItem>
    {

    }
}
