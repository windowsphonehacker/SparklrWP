using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.JSONRepresentations.Get
{
    /// <summary>
    /// Represents a userid - username construct
    /// </summary>
    public class UserMinimal
    {
        /// <summary>
        /// The userid
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// The username
        /// </summary>
        public string username { get; set; }
    }
}
