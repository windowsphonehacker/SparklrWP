using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Sparklr
{
    /// <summary>
    /// Represents a User on the Sparklr service
    /// </summary>
    public class User
    {
        /// <summary>
        /// The ID
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The handle (without @)
        /// </summary>
        public string Handle { get; private set; }

        /// <summary>
        /// The avatar-id. -1 if no avatar is selected.
        /// </summary>
        public long AvatarId { get; private set; }

        /// <summary>
        /// True if the logged in user is following this user.
        /// </summary>
        public bool Following { get; private set; }

        /// <summary>
        /// The biography of the user.
        /// </summary>
        public string Bio { get; private set; }

        /// <summary>
        /// The recent posts of this user.
        /// </summary>
        public ReadOnlyCollection<Post> Timeline
        {
            get
            {
                return new ReadOnlyCollection<Post>(timeline);
            }
        }

        internal List<Post> timeline { get; private set; }

        private static Dictionary<int, User> userCache = new Dictionary<int, User>();

        /// <summary>
        /// Handles the creation of instances. Only one instance of a user can exist at a time.
        /// </summary>
        /// <param name="userid">The userid of the specified user</param>
        /// <param name="conn">The connection on which to run the identification</param>
        /// <returns></returns>
        internal static async Task<User> InstanciateUserAsync(int userid, Connection conn)
        {
            if (!userCache.ContainsKey(userid))
            {
                User u = await conn.GetUserAsync(userid);
                //This will always add the User to the cache
            }

            return userCache[userid];
        }

        internal static User InstanciateUser(int userid, string name, string handle, long avatarid, bool following, string bio)
        {
            if (!userCache.ContainsKey(userid))
            {
                User u = new User(userid, name, handle, avatarid, following, bio);
                userCache.Add(userid, u);
            }

            return userCache[userid];
        }

        /// <summary>
        /// You most likely don't want to call this. Use CreateUser instead.
        /// </summary>
        /// <param name="userid">the id</param>
        /// <param name="name">the name</param>
        /// <param name="handle">the handle</param>
        /// <param name="avatarid">the avatarid</param>
        /// <param name="following">true if following</param>
        /// <param name="bio">the biography</param>
        private User(int userid, string name, string handle, long avatarid, bool following, string bio)
        {
            this.UserId = userid;
            this.Name = name;
            this.Handle = handle;
            this.AvatarId = avatarid;
            this.Following = following;
            this.Bio = bio;

            this.timeline = new List<Post>();
        }
    }
}
