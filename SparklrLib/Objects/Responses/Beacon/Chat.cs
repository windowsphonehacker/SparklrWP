using System.Collections.Generic;

namespace SparklrLib.Objects.Responses.Beacon
{
    public class ChatMessage
    {
        public int to { get; set; }
        public int from { get; set; }
        public int time { get; set; }
        public string message { get; set; }
    }

    public class Chat
    {
        public List<ChatMessage> data { get; set; }
    }
}
