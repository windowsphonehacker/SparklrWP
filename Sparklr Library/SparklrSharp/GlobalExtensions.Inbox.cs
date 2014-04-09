using SparklrSharp.Sparklr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp
{
    /// <summary>
    /// Contains Extensions that are globally available
    /// </summary>
    public static partial class GlobalExtensions
    {
        /// <summary>
        /// Updates the Inbox via the given connection
        /// </summary>
        /// <param name="conn">The connection on which to update the Messages</param>
        /// <returns></returns>
        public static async Task RefreshInboxAsync(this Connection conn)
        {
            conn.Inbox = await conn.GetInboxAsync();
        }
    }
}
