using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Post
{
    /// <summary>
    /// JSON representation of a message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Conversation partner
        /// </summary>
        public int to { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string message { get; set; }
    }
}
