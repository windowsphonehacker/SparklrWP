using SparklrSharp.Communications;
using SparklrSharp.Sparklr;
using SparklrSharp.Extensions;
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
        /// Contains the current Inbox. Needs to be refreshed manually
        /// </summary>
        public IList<Message> Inbox { get; internal set; }

        /// <summary>
        /// Retreives all messages
        /// </summary>
        /// <returns></returns>
        internal async Task<Sparklr.Message[]> GetInboxAsync()
        {
            SparklrResponse<JSONRepresentations.Get.Message[]> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Message[]>("inbox");

            Sparklr.Message[] messages = await parseJSONMessages(response);

            return messages;
        }

        /// <summary>
        /// Retreives a conversation asynchronously
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="starttime"></param>
        /// <returns></returns>
        internal async Task<Message[]> GetConversationAsync(int userId, long? starttime = null)
        {
            SparklrResponse<JSONRepresentations.Get.Message[]> response;

            if (starttime == null)
                response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Message[]>("chat", userId);
            else
                response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Message[]>("chat", userId + "?starttime=" + starttime);

            Sparklr.Message[] messages = await parseJSONMessages(response);

            return messages;
        }

        /// <summary>
        /// Retreives a conversation asynchronously
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        internal async Task<Message[]> GetConversationSinceAsync(int userId, long since)
        {
            SparklrResponse<JSONRepresentations.Get.Message[]> response = await webClient.GetJSONResponseAsync<JSONRepresentations.Get.Message[]>("chat", userId + "?since=" + since);

            Sparklr.Message[] messages = await parseJSONMessages(response);

            return messages;
        }

        /// <summary>
        /// Extracts message information from a JSON object and turns them into a Sparklr.Message
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<Message[]> parseJSONMessages(SparklrResponse<JSONRepresentations.Get.Message[]> response)
        {
            Sparklr.Message[] messages = new Sparklr.Message[response.Response.Length];

            for (int i = 0; i < response.Response.Length; i++)
            {
                messages[i] = await Message.CreateMessageAsync(
                        response.Response[i].message,
                        response.Response[i].time,
                        response.Response[i].from,
                        this
                    );
            }
            return messages;
        }

        internal Task<bool> SendMessageAsync(string content, User recipient)
        {
            return SendMessageAsync(content, recipient.UserId);
        }

        internal async Task<bool> SendMessageAsync(string content, int userid)
        {
            try
            {
                JSONRepresentations.Post.Message m = new JSONRepresentations.Post.Message()
                                                            {
                                                                message = content,
                                                                to = userid
                                                            };

                SparklrResponse<string> result = await webClient.PostJsonAsyncRawResponse("chat", m);

                if (result.IsOkAndTrue())
                {
                    return true;
                }
                else if (result.Code == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new ArgumentException("The message you wanted to send was not accepted by the server, maybe it was too long?", content);
                }
                else
                {
                    return false;
                }
            }
            catch (Exceptions.NotAuthorizedException)
            {
                throw new Exceptions.NotAuthorizedException("You are either not authorized or blocked by the specified user.");
            }
        }
    }
}
