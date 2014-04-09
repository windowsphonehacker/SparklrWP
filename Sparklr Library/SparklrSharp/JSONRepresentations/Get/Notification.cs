using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Get
{
    /// <summary>
    /// JSON structure for notifiactions
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// The id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// A user id
        /// </summary>
        public int from { get; set; }
        /// <summary>
        /// A user id
        /// </summary>
        public int to { get; set; }
        /// <summary>
        /// Type (e.g. mention, message, like, etc.)
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// Timestamp
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// Some content, can be null
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// An action (e.g. post id), can be null
        /// </summary>
        public string action { get; set; }
    }
}
