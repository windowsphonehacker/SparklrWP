using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Get
{
    /// <summary>
    /// JSON structure for comments
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Comment id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Related post
        /// </summary>
        public int postid { get; set; }
        /// <summary>
        /// Author id
        /// </summary>
        public int from { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        public int time { get; set; }
    }
}
