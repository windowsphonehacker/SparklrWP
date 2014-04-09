using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Sparklr
{
    /// <summary>
    /// Provides a representation of messages on the Sparklr service.
    /// </summary>
    public class Message : IComparable<Message>
    {
        /// <summary>
        /// The content of the message
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// The timestamp, when the message was sent.
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// The author of the message.
        /// </summary>
        public User Author { get; private set; }

        private Message(string content, long timestamp, User conversationPartner)
        {
            Content = content;
            Timestamp = timestamp;
            Author = conversationPartner;
        }

        /// <summary>
        /// Creates a message and fills the user details
        /// </summary>
        /// <param name="content"></param>
        /// <param name="timestamp"></param>
        /// <param name="userid"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        internal static async Task<Message> CreateMessageAsync(string content, long timestamp, int userid, Connection conn)
        {
            User conversationPartner = await User.InstanciateUserAsync(userid, conn);
            return new Message(content, timestamp, conversationPartner);
        }

        /// <summary>
        /// Compares the given message to the
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int CompareTo(Message item)
        {
            return this.Timestamp.CompareTo(item.Timestamp);
        }
    }
}
