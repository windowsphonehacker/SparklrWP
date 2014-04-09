using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SparklrSharp.Communications
{
    /// <summary>
    /// Provides abstraction for a response from the sparklr server
    /// </summary>
    /// <typeparam name="T">The type of content</typeparam>
    internal class SparklrResponse<T>
    {
        /// <summary>
        /// The code returned by the server
        /// </summary>
        internal HttpStatusCode Code { get; private set; }

        /// <summary>
        /// The content returned by the server
        /// </summary>
        internal T Response { get; private set; }

        public SparklrResponse(HttpStatusCode code, T response)
        {
            this.Code = code;
            this.Response = response;
        }
    }
}
