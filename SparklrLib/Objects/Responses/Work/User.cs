using System.Collections.Generic;

namespace SparklrLib.Objects.Responses.Work
{
    public class Timeline
    {
        public int from { get; set; }
        public int id { get; set; }
        public int? to { get; set; }
        public int type { get; set; }
        public object flags { get; set; }
        public string meta { get; set; }
        public int time { get; set; }
        public int @public { get; set; }
        public string message { get; set; }
        public object lat { get; set; }
        public object @long { get; set; }
        public int? via { get; set; }
        public int? origid { get; set; }
        public int? commentcount { get; set; }
        public int? modified { get; set; }
        public int network { get; set; }
    }

    public class User
    {
        public int user { get; set; }
        public string handle { get; set; }
        public int avatarid { get; set; }
        public int background { get; set; }
        public bool following { get; set; }
        public string name { get; set; }
        public string bio { get; set; }
        public List<Timeline> timeline { get; set; }
    }
}
