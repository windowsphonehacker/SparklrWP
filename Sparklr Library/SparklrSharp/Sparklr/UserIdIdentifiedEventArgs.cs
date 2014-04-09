using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.Sparklr
{
    internal class UserIdIdentifiedEventArgs : EventArgs
    {
        internal int IdentifiedUserId { get; private set; }

        internal UserIdIdentifiedEventArgs(int u)
        {
            IdentifiedUserId = u;
        }
    }
}
