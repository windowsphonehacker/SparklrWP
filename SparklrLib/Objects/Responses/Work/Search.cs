using System.Collections.Generic;

namespace SparklrLib.Objects.Responses.Work
{
    public class SearchUser
    {
        public string username { get; set; }
        public int id { get; set; }
    }

    public class SearchPost
    {
        public int from { get; set; }
        public int id { get; set; }
        public object to { get; set; }
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
        public int modified { get; set; }
        public string network { get; set; }
    }

    public class Search
    {
        public List<SearchUser> users { get; set; }
        public List<SearchPost> posts { get; set; }
    }
}
