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
        /// Retreives the conversation with the given partner, this call blocks while waiting for responses.
        /// </summary>
        /// <param name="conn">The connection on which to run the query</param>
        /// <param name="conversationPartner">The ID of te partner</param>
        private static IEnumerable<Message> ConversationWith(this Connection conn, int conversationPartner)
        {
            Message[] messages = conn.GetConversationAsync(conversationPartner).Result;

            do
            {
                long lastStart = -1;

                foreach (Message m in messages)
                {
                    yield return m;
                    lastStart = m.Timestamp;
                }

                messages = conn.GetConversationAsync(conversationPartner, lastStart).Result;
            } while (messages.Length > 0);
        }

        /// <summary>
        /// Retreives the conversation with the given user on the given connection, this is a blocking call
        /// </summary>
        /// <param name="u">The conversation partner</param>
        /// <param name="conn">The connection on which to run the query</param>
        /// <returns></returns>
        public static IEnumerable<Message> ConversationWith(this User u, Connection conn)
        {
            return ConversationWith(conn, u.UserId);
        }

        /// <summary>
        /// Retreives the conversation with the given user on the given connection, this is a blocking call
        /// </summary>
        /// <param name="u">The conversation partner</param>
        /// <param name="conn">The connection on which to run the query</param>
        /// <returns></returns>
        public static IEnumerable<Message> ConversationWith(this Connection conn, User u)
        {
            return ConversationWith(conn, u.UserId);
        }

        /// <summary>
        /// Retreives (or creates) a conversation on the given connection. You can use this to handle conversations asynchronously
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Conversation GetConversationWith(this Connection conn, User u)
        {
            return new Conversation(u, conn);
        }

        /// <summary>
        /// Retreives (or creates) a conversation on the given connection. You can use this to handle conversations asynchronously
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Conversation GetConversationWith(this User u, Connection conn)
        {
            return GetConversationWith(conn, u);
        }

        /// <summary>
        /// Retreives (or creates) a conversation with the user with the given id.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static async Task<Conversation> GetConversationWithUserIdAsync(this Connection conn, int userid)
        {
            User u = await User.InstanciateUserAsync(userid, conn);
            return GetConversationWith(conn, u);
        }
    }
}
