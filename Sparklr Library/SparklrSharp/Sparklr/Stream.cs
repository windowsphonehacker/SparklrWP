using SparklrSharp.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Sparklr
{
    //TODO: Create base class for refreshable message lists (stream, mentions, etc.)

    /// <summary>
    /// Represents a stream of messages. The stream can be either a network or all posts by a specified user.
    /// </summary>
    public class Stream
    {
        private static Dictionary<string, Stream> streamCache = new Dictionary<string, Stream>();

        private SortedList<Post> posts = new SortedList<Post>();

        /// <summary>
        /// The name of the stream
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The current posts in this stream
        /// </summary>
        public ReadOnlyCollection<Post> Posts
        {
            get
            {
                return new ReadOnlyCollection<Post>(posts);
            }
        }

        /// <summary>
        /// Returns an instance of the given stream. Streams are cached
        /// </summary>
        /// <param name="name">The name of the stream</param>
        /// <param name="conn">The connection on which to retreive the stream</param>
        /// <returns></returns>
        public static async Task<Stream> InstanciateStreamAsync(string name, Connection conn)
        {
            if (!streamCache.ContainsKey(name))
            {
                Stream s = new Stream(name);

                Post[] initialPosts = await conn.GetStreamAsync(name);

                foreach (Post p in initialPosts)
                    s.posts.Add(p);

                streamCache.Add(name, s);
            }

            return streamCache[name];
        }

        /// <summary>
        /// Returns a stream of posts for the given user. Streams are cached.
        /// </summary>
        /// <param name="u">The user for which to retreive the posts</param>
        /// <param name="conn">The conenction on which to run the query</param>
        /// <returns></returns>
        public static Task<Stream> InstanciateStreamAsync(User u, Connection conn)
        {
            return InstanciateStreamAsync(u.UserId.ToString(), conn);
        }

        private Stream(string name)
        {
            this.Name = name;
        }

        //TODO: Support for refreshing
    }
}
