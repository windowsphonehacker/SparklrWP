
namespace SparklrLib.Objects.Responses.Beacon
{
    public class ChatMessage
    {
        public int to { get; set; }
        public int from { get; set; }
        public int time { get; set; }
        public string message { get; set; }
    }

    public class Chat : BeaconBase
    {
        public ChatMessage[] data { get; set; }
    }
}
