using SparklrSharp.Sparklr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp
{
    public static partial class GlobalExtensions
    {
        /// <summary>
        /// Retreives the stream for the given network
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Task<Stream> GetStreamAsync(this Connection conn, string name)
        {
            return Stream.InstanciateStreamAsync(name, conn);
        }

        /// <summary>
        /// Retreives the stream for the given user
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Task<Stream> GetStreamAsync(this Connection conn, User u)
        {
            return Stream.InstanciateStreamAsync(u, conn);
        }

        /// <summary>
        /// Retreives the stream for the given user
        /// </summary>
        /// <param name="u"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Task<Stream> GetStreamAsync(this User u, Connection conn)
        {
            return Stream.InstanciateStreamAsync(u, conn);
        }
    }
}
