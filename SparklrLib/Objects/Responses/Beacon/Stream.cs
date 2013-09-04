using System.Collections.Generic;

namespace SparklrLib.Objects.Responses.Beacon
{
    public class Timeline
    {
        public int from { get; set; }
        public int id { get; set; }
        public int? to { get; set; }
        public int type { get; set; }
        public object flags { get; set; }
        public string meta { get; set; }
        public string imageUrl
        {
            get
            {
                return meta.Split(',')[0];
            }
        }
        public int time { get; set; }
        public int @public { get; set; }
        public string message { get; set; }
        public object lat { get; set; }
        public object @long { get; set; }
        public int? via { get; set; }
        public int? origid { get; set; }
        public int? commentcount { get; set; }
        public int modified { get; set; }
        public int network { get; set; }
    }

    public class Data
    {
        public List<Timeline> timeline { get; set; }
        public int length { get; set; }
    }

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

    public class Stream
    {
        public Data data { get; set; }
        public List<Notification> notifications { get; set; }
    }
}
