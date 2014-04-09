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
        /// Retreives the comments for the given post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task<Comment[]> GetCommentsForPostAsync(int id)
        {
            SparklrResponse<JSONRepresentations.Get.Comment[]> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Comment[]>("comments", id);

            return await extractComments(response);
        }

        /// <summary>
        /// Retreives the comments for the given post
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time">The time after which to retreive the comments</param>
        /// <returns></returns>
        internal async Task<Comment[]> GetCommentsForPostAsync(int id, int time)
        {
            SparklrResponse<JSONRepresentations.Get.Comment[]> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Comment[]>("comments", id + "?since=" + time);

            return await extractComments(response);
        }

        private async Task<Comment[]> extractComments(SparklrResponse<JSONRepresentations.Get.Comment[]> response)
        {
            Comment[] comments = new Comment[response.Response.Length];

            int i = 0;
            foreach (JSONRepresentations.Get.Comment c in response.Response)
            {
                comments[i] = Comment.InstanciateComment(
                                            c.id,
                                            await Post.GetPostByIdAsync(c.postid, this),
                                            await User.InstanciateUserAsync(c.from, this),
                                            c.message,
                                            c.time
                                        );
                i++;
            }

            return comments;
        }
    }
}
