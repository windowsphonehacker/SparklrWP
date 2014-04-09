using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Sparklr
{
    /// <summary>
    /// Represents a comment on the sparklr service
    /// </summary>
    public class Comment : IComparable<Comment>
    {
        /// <summary>
        /// The id of the comment
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The post that this comment it related to
        /// </summary>
        public Post Parent { get; private set; }

        /// <summary>
        /// The author of the comment
        /// </summary>
        public User Author { get; private set; }

        /// <summary>
        /// The message of the comment
        /// </summary>
        public String Message { get; private set; }

        /// <summary>
        /// The timestamp of the comment
        /// </summary>
        public long Timestamp { get; private set; }

        private static Dictionary<int, Comment> commentCache = new Dictionary<int, Comment>();

        internal static Comment InstanciateComment(int id, Post parent, User author, string message, long timestamp)
        {
            if (!commentCache.ContainsKey(id))
                commentCache.Add(id, new Comment(
                        id,
                        parent,
                        author,
                        message,
                        timestamp
                    ));

            return commentCache[id];
        }

        private Comment(int id, Post parent, User author, string message, long timestamp)
        {
            this.Id = id;
            this.Parent = parent;
            this.Author = author;
            this.Message = message;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Compares the items
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int CompareTo(Comment item)
        {
            return this.Timestamp.CompareTo(item.Timestamp);
        }
    }
}
