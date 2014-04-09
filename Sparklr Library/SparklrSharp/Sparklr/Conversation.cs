using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SparklrSharp.Collections;

namespace SparklrSharp.Sparklr
{
    /// <summary>
    /// Represents a conversation on the sparklr service
    /// </summary>
    public class Conversation
    {
        /// <summary>
        /// Contains a list of all messages in the conversation. Is not complete until NeedsRefresh is false. Use LoadMoreAsync to load more messages.
        /// </summary>
        public ReadOnlyCollection<Message> Messages
        {
            get
            {
                return new ReadOnlyCollection<Message>(messageBuffer);
            }
        }

        internal Conversation(User conversationPartner, Connection conn)
        {
            this.conn = conn;
            this.ConversationPartner = conversationPartner;
            NeedsRefresh = true;
        }

        /// <summary>
        /// Contains all messages of the conversation
        /// </summary>
        /// TODO: make ordered by timestamp
        private SortedList<Message> messageBuffer = new SortedList<Message>();

        /// <summary>
        /// The timestamp of the oldest message that was loaded
        /// </summary>
        private long oldestTimestamp = -1;

        /// <summary>
        /// The timestamp of the most recent loaded message
        /// </summary>
        private long newestTimestamp = -1;

        /// <summary>
        /// True when all messages in the conversation were loaded, otherwise false.
        /// </summary>
        public bool NeedsRefresh { get; private set; }

        /// <summary>
        /// The partner of this conversation
        /// </summary>
        public User ConversationPartner { get; private set; }

        /// <summary>
        /// The connection associated with this conversation
        /// </summary>
        private Connection conn;

        /// <summary>
        /// Loads more messages.
        /// </summary>
        /// <returns>true if more messages are available, otherwise false</returns>
        public async Task<bool> LoadMore()
        {
            List<Message> messages = new List<Message>(
                    oldestTimestamp >= 0 ? await conn.GetConversationAsync(ConversationPartner.UserId, oldestTimestamp) : await conn.GetConversationAsync(ConversationPartner.UserId)
                );

            if (newestTimestamp >= 0)
            {
                List<Message> newerMessages = new List<Message>(
                    await conn.GetConversationSinceAsync(ConversationPartner.UserId, newestTimestamp)
                );

                messages.AddRange(newerMessages);
            }

            if (messages.Count == 0)
            {
                NeedsRefresh = false;
                return false;
            }
            else
            {
                NeedsRefresh = true;

                foreach (Message m in messages)
                {
                    messageBuffer.Add(m);

                    if (oldestTimestamp == -1)
                        oldestTimestamp = m.Timestamp;
                    else if (oldestTimestamp > m.Timestamp)
                        oldestTimestamp = m.Timestamp;

                    if (newestTimestamp == -1)
                        newestTimestamp = m.Timestamp;
                    else if (newestTimestamp < m.Timestamp)
                        newestTimestamp = m.Timestamp;
                }

                return true;
            }
        }

        /// <summary>
        /// Sends a message in the current conversation.
        /// </summary>
        /// <param name="content">The message to send</param>
        /// <returns>true on success, otherwise an exception is thrown</returns>
        public Task<bool> SendMessage(string content)
        {
            NeedsRefresh = true;
            return conn.SendMessageAsync(content, ConversationPartner);
        }
    }
}
