using SparklrSharp.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Sparklr
{
    public class Mentions
    {
        private static Dictionary<int, Mentions> streamCache = new Dictionary<int, Mentions>();

        private SortedList<Post> posts = new SortedList<Post>();

        /// <summary>
        /// The User that is being mentioned
        /// </summary>
        public User User { get; private set; }

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
        /// Returns an instance of a streamn with mentions. Mentions are cached
        /// </summary>
        /// <param name="user">The user that is mentioned</param>
        /// <param name="conn">The connection on which to retreive the mentions</param>
        /// <returns></returns>
        public static async Task<Mentions> InstanciateMentionsAsync(User user, Connection conn)
        {
            if (!streamCache.ContainsKey(user.UserId))
            {
                Mentions m = new Mentions(user);

                Post[] initialPosts = await conn.GetMentionsAsync(user.UserId);

                foreach (Post p in initialPosts)
                    m.posts.Add(p);

                streamCache.Add(user.UserId, m);
            }

            return streamCache[user.UserId];
        }

        private Mentions(User u)
        {
            this.User = u;
        }

        //TODO: Support for refreshing
    }
}
