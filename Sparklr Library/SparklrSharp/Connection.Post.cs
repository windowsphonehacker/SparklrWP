using SparklrSharp.Communications;
using SparklrSharp.Sparklr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp
{
    public partial class Connection
    {
        /// <summary>
        /// retreives the post by the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<Post> GetPostByIdAsync(int id)
        {
            SparklrResponse<JSONRepresentations.Get.Post> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Post>("post", id);

            Post p = await Post.InstanciatePostAsync(response.Response, this);

            return p;
        }
    }
}
