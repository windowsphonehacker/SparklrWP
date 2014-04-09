using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.Exceptions
{
    /// <summary>
    /// Occurs when data is expected but not found on the sparklr server.
    /// </summary>
    public class NoDataFoundException : Exception
    {
        internal NoDataFoundException() : base()
        {

        }

        internal NoDataFoundException(string message)
            : base(message)
        { }
    }
}
