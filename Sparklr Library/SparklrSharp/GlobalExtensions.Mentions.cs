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
        /// Retreives mentions of a user on the given connection
        /// </summary>
        /// <param name="u">The user for which to retreive the mentions</param>
        /// <param name="conn">The connection on which the query is run</param>
        /// <returns>Mentions (cached)</returns>
        public static async Task<Mentions> GetMentionsAsync(this User u, Connection conn)
        {
            return await Mentions.InstanciateMentionsAsync(u, conn);
        }

        /// <summary>
        /// Retreives mentions of a user on the given connection
        /// </summary>
        /// <param name="u">The user for which to retreive the mentions</param>
        /// <param name="conn">The connection on which the query is run</param>
        /// <returns>Mentions (cached)</returns>
        public static async Task<Mentions> GetMentionsAsync(this Connection conn, User u)
        {
            return await Mentions.InstanciateMentionsAsync(u, conn);
        }
    }
}
