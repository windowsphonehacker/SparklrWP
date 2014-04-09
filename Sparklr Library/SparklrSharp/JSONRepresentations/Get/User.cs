using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Get
{
    /// <summary>
    /// Represents a JSON-User
    /// </summary>
    public class User
    {
        /// <summary>
        /// User-ID
        /// </summary>
        public int user { get; set; }

        /// <summary>
        /// Handle without @
        /// </summary>
        public string handle { get; set; }

        /// <summary>
        /// Avatar-ID, can be null
        /// </summary>
        public long? avatarid { get; set; }

        /// <summary>
        /// True when the current user is following the specified user
        /// </summary>
        public bool following { get; set; }

        /// <summary>
        /// The name oif the user
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The biography of the user
        /// </summary>
        public string bio { get; set; }

        /// <summary>
        /// Recent posts by this user
        /// </summary>
        public List<Post> timeline { get; set; }
    }
}
