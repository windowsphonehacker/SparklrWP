using System.Collections.Generic;

namespace SparklrLib.Objects.Responses.Work
{
    public class Comment
    {
        public int id { get; set; }
        public int postid { get; set; }
        public int from { get; set; }
        public string message { get; set; }
        public int time { get; set; }
    }

    public class Post
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
        public string network { get; set; }
        public List<Comment> comments { get; set; }
    }
}
