using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Get
{
    /// <summary>
    /// A JSON-Post with all necessary fields
    /// </summary>
    public class Post
    {
        /// <summary>
        /// The ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Author-id
        /// </summary>
        public int from { get; set; }

        /// <summary>
        /// The post's network
        /// </summary>
        public string network { get; set; }

        /// <summary>
        /// Type TODO: What is this?
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// Meta-information
        /// </summary>
        public string meta { get; set; }

        /// <summary>
        /// Original creation timestamp
        /// </summary>
        public long time { get; set; }

        /// <summary>
        /// True when visible to the public
        /// </summary>
        public int? @public { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Original id of a reposted post
        /// </summary>
        public int? origid { get; set; }

        /// <summary>
        /// Original author of a reposted post
        /// </summary>
        public int? via { get; set; }

        /// <summary>
        /// Amount of comments
        /// </summary>
        public int? commentcount { get; set; }

        /// <summary>
        /// Last modification date
        /// </summary>
        public long? modified { get; set; }

        /// <summary>
        /// Currently unknwon
        /// </summary>
        public int? to { get; set; }

        /// <summary>
        /// Unknown, seems to default to null
        /// </summary>
        public object flags { get; set; }

        /// <summary>
        /// The comments associated with this post
        /// </summary>
        List<Comment> comments { get; set; }
    }
}
