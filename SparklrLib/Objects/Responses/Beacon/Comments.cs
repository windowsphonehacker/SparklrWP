using System.Collections.Generic;

namespace SparklrLib.Objects.Responses.Beacon
{
    public class Comment
    {
        public int id { get; set; }
        public int postid { get; set; }
        public int from { get; set; }
        public string message { get; set; }
        public int time { get; set; }
    }

    public class Comments
    {
        public List<Comment> data { get; set; }
    }
}
