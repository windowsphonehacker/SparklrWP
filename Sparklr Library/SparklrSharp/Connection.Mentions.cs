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
        internal async Task<Post[]> GetMentionsAsync(int userid)
        {
            SparklrResponse<JSONRepresentations.Get.Post[]> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Post[]>("mentions", userid);
            return await extractPostsAsync(response);
        }

        internal async Task<Post[]> GetMentionsSinceAsync(int userid, int timestamp)
        {
            SparklrResponse<JSONRepresentations.Get.Post[]> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Post[]>("mentions", userid + "?since=" + timestamp);
            return await extractPostsAsync(response);
        }

        internal async Task<Post[]> GetMentionsAsync(int userid, int starttime)
        {
            SparklrResponse<JSONRepresentations.Get.Post[]> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Post[]>("mentions", userid + "?starttime=" + starttime);
            return await extractPostsAsync(response);
        }
    }
}
