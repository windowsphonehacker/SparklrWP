using SparklrSharp.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Sparklr
{
    /// <summary>
    /// Represents a post on the Sparklr service
    /// </summary>
    public class Post : IComparable<Post>
    {
        private static Dictionary<int, Post> postCache = new Dictionary<int,Post>();

        /// <summary>
        /// The Post-ID
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The author of the post
        /// </summary>
        public User Author { get; private set; }

        /// <summary>
        /// The network this post was posted on
        /// </summary>
        public string Network { get; private set; }

        /// <summary>
        /// The message type
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Meta-Information. TODO: What is it? Maybe an image?
        /// </summary>
        public string Meta { get; set; }

        /// <summary>
        /// Original timestamp
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Indicates if the post is visible.
        /// </summary>
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Content of the post
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// The original post of a reposted post.
        /// </summary>
        public Post OriginalPost { get; private set; }

        /// <summary>
        /// The original author of a reposted post
        /// </summary>
        public User ViaUser { get; private set; }

        /// <summary>
        /// The number of comments
        /// </summary>
        public int CommentCount { get; private set; }

        /// <summary>
        /// Indicates when the post was last modified
        /// </summary>
        public long ModifiedTimestamp { get; private set; }

        /// <summary>
        /// Contains the comments
        /// </summary>
        private SortedList<Comment> comments;

        /// <summary>
        /// Retreives a post on the given connection. Uses caching.
        /// </summary>
        /// <param name="id">The identifier of the post</param>
        /// <param name="conn">The connection on which to run the query</param>
        /// <returns></returns>
        public static async Task<Post> GetPostByIdAsync(int id, Connection conn)
        {
            if (postCache.ContainsKey(id))
                return postCache[id];

            Post p = await conn.GetPostByIdAsync(id);
            return p;
        }

        internal static Post InstanciatePost(int id, User author, string network, int type, string meta, long timestamp, bool isPublic, string content, Post originalPost, User viaUser, int commentCount, long modifiedTimestamp)
        {
            if (!postCache.ContainsKey(id))
                postCache.Add(id, new Post(id, author, network, type, meta, timestamp, isPublic, content, originalPost, viaUser, commentCount, modifiedTimestamp));
            
            return postCache[id];
        }

        internal async static Task<Post> InstanciatePostAsync(JSONRepresentations.Get.Post p, Connection conn)
        {
            return InstanciatePost(p.id,
                            await User.InstanciateUserAsync(p.from, conn),
                            p.network,
                            p.type,
                            p.meta,
                            p.time,
                            p.@public != null ? p.@public == 1 : false,
                            p.message,
                            p.origid != null ? (await Post.GetPostByIdAsync((int)p.origid, conn)) : null,
                            p.via != null ? await User.InstanciateUserAsync((int)p.via, conn) : null,
                            p.commentcount ?? 0,
                            p.modified ?? -1);
        }

        private Post(int id, User author, string network, int type, string meta, long timestamp, bool isPublic, string content, Post originalPost, User viaUser, int commentCount, long modifiedTimestamp)
        {
            this.Id = id;
            this.Author = author;
            this.Network = network;
            this.Type = type;
            this.Meta = meta;
            this.Timestamp = timestamp;
            this.IsPublic = IsPublic;
            this.Content = content;
            this.OriginalPost = originalPost;
            this.ViaUser = viaUser;
            this.CommentCount = commentCount;
            this.ModifiedTimestamp = modifiedTimestamp;
        }

        /// <summary>
        /// Likes/Unlikes the given post. TODO: implement
        /// </summary>
        /// <returns></returns>
        public bool ToggleLike()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a comment on this post. TODO: implement
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool Comment(string comment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares the items
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int CompareTo(Post item)
        {
            return this.Timestamp.CompareTo(item.Timestamp);
        }

        /// <summary>
        /// Retreives the comments, or returns the comments if they are already retreived
        /// </summary>
        /// <returns></returns>
        public async Task<ReadOnlyCollection<Comment>> GetCommentsAsync(Connection conn)
        {
            if (comments != null)
                return new ReadOnlyCollection<Comment>(comments);

            comments = new SortedList<Comment>();

            Comment[] comms = await conn.GetCommentsForPostAsync(this.Id);

            foreach (Comment c in comms)
                comments.Add(c);

            return new ReadOnlyCollection<Comment>(comments);
        }
    }
}
