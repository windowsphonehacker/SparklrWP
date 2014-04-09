using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.Exceptions
{
    /// <summary>
    /// Occurs when the service returns 403. Make sure you have authenticated with Connection.Signin.
    /// </summary>
    public class NotAuthorizedException : Exception
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public NotAuthorizedException()
        {
        
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="message">A message containing details</param>
        public NotAuthorizedException(string message) : base(message)
        {
        }
    }
}
