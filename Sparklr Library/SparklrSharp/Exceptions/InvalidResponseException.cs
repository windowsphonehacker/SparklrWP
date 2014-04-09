using SparklrSharp.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.Exceptions
{
    /// <summary>
    /// Occurs when an unexpected response is retreived by the server.
    /// </summary>
    public class InvalidResponseException : Exception
    {
        internal SparklrResponse<string> Response { get; set; }
    }
}
