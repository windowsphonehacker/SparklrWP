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
        /// Retreives the notifications on the given connection
        /// </summary>
        /// <param name="conn">The connection on which to run the query</param>
        /// <returns>An array containing all notifications or an empty array if no notifications are present.</returns>
        public static async Task<Notification[]> GetNotificationsAsync(this Connection conn)
        {
            return await conn.GetNotificationsAsync();
        }

        /// <summary>
        /// Dismisses the given notification on the given connection
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static async Task DismissNotificationAsync(this Connection conn, Notification n)
        {
            await n.DismissAsync(conn);
        }
    }
}
