using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.Sparklr
{
    /// <summary>
    /// Carries information for an event when an user has been identified
    /// </summary>
    public class UserIdentifiedEventArgs : EventArgs
    {
        /// <summary>
        /// The user that was identified
        /// </summary>
        public User IdentifiedUser { get; private set; }

        internal UserIdentifiedEventArgs(User u)
        {
            IdentifiedUser = u;
        }
    }
}
