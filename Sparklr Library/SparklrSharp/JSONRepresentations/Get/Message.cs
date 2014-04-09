using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Get
{
    /// <summary>
    /// The JSON representation of a message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Content of the message
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// A timestamp
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// The author of the message
        /// </summary>
        public int from { get; set; }
    }
}
