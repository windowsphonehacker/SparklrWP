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
        /// Retreives the post with the given ID from the sparklr service. Uses caching.
        /// </summary>
        /// <param name="conn">The connection on which to perform the request</param>
        /// <param name="id">The id of the post</param>
        /// <returns></returns>
        public static Task<Post> GetPostByIdAsync(this Connection conn, int id)
        {
            return Post.GetPostByIdAsync(id, conn);
        }
    }
}
