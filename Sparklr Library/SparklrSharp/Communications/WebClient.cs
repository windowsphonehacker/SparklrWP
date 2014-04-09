using Newtonsoft.Json;
using SparklrSharp.Exceptions;
using SparklrSharp.Sparklr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SparklrSharp.Communications
{
    /// <summary>
    /// Provides some extended functionality to do API calls. Keeps track on cookies.
    /// </summary>
    internal class WebClient
    {
        internal event EventHandler<UserIdIdentifiedEventArgs> CurrentUserIdReceived;

#if DEBUG
        /// <summary>
        /// Contains the number of requests sent
        /// </summary>
        internal static int DEBUG_NumberOfRequestReceived = 0;
#endif

        /// <summary>
        /// The location of the api
        /// </summary>
#if DEBUG
        private const string baseUrl = "http://sparklr.me/api/";
#else
        private const string baseUrl = "https://sparklr.me/api/";
#endif

        /// <summary>
        /// The base uri - used to identify cookies
        /// </summary>
        private readonly Uri baseUri = new Uri("https://sparklr.me");

        //TODO: switch to OAuth when ready
        private string X = null;

        private Dictionary<string, string> cookies = new Dictionary<string,string>();

        /// <summary>
        /// Performs a GET-Request on the given location.
        /// </summary>
        /// <param name="uri">The location of the given ressource. Will be appended to the baseUrl, i.e. sparklr.me/api/</param>
        /// <param name="parameters">A set of parameters to append. Will be urlencoded and joined to the Url.</param>
        /// <returns>The result of the request</returns>
        internal async Task<SparklrResponse<string>> GetRawResponseAsync(string uri, params object[] parameters)
        {
            string url = buildUrl(uri, parameters);
            HttpWebRequest request = prepareRequest(url);

            WebException exception;

            try
            {
                return await CreateResponseAsync((HttpWebResponse)(await request.GetResponseAsync()));
            }
            catch (WebException ex)
            {
                exception = ex;
            }

            //Will only happen when the request fails fails
            return await CreateResponseAsync((HttpWebResponse)(exception.Response));
        }

        private static string buildUrl(string uri, object[] parameters)
        {
            string url = baseUrl + uri;

            //TODO: Urlencode
            if (parameters.Length > 0)
                url = url + "/" + String.Join("/", parameters);
            return url;
        }

        internal async Task<SparklrResponse<string>> PostJsonAsyncRawResponse<T>(string uri, T jsonObject, params object[] parameters)
        {
            string url = buildUrl(uri, parameters);
            HttpWebRequest request = prepareRequest(url);

            string xdata = JsonConvert.SerializeObject(jsonObject);

            request.Method = "POST";
            request.Headers["X-Data"] = xdata;

            WebException exception;

            try
            {
                return await CreateResponseAsync((HttpWebResponse)(await request.GetResponseAsync()));
            }
            catch (WebException ex)
            {
                exception = ex;
            }

            //The download has failed
            return await CreateResponseAsync((HttpWebResponse)(exception.Response));
        }

        /// <summary>
        /// Performs a GET request on the server and deserializes it into the given type
        /// </summary>
        /// <typeparam name="T">The desired class</typeparam>
        /// <param name="uri">The URI to request</param>
        /// <param name="parameters">The paramteres to append. Will be URLencoded</param>
        /// <returns>A response containing the status code and the created object</returns>
        internal async Task<SparklrResponse<T>> GetJSONResponseAsync<T>(string uri, params object[] parameters)
        {
            SparklrResponse<string> response = await GetRawResponseAsync(uri, parameters);

            try
            {
                SparklrResponse<T> result = new SparklrResponse<T>(response.Code, JsonConvert.DeserializeObject<T>(response.Response));
                return result;
            }
            catch (JsonSerializationException)
            {
                throw new Exceptions.InvalidResponseException() { Response = response };
            }
        }

        /// <summary>
        /// Handles the creation of a SparklrResponse from a HttpWebResponse. Will treat three digit responses as status codes.
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>A SparklrResponse with the correct status code and content</returns>
        private async Task<SparklrResponse<string>> CreateResponseAsync(HttpWebResponse response)
        {
#if DEBUG
            DEBUG_NumberOfRequestReceived++;
#endif

            if (response == null)
                throw new Exceptions.NoDataFoundException("Could not connect to Sparklr");

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new Exceptions.NotAuthorizedException();

            using (System.IO.Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                String responseString = await reader.ReadToEndAsync();
                HttpStatusCode statusCode = response.StatusCode;

                if (response.Headers.AllKeys.Contains("Set-Cookie"))
                {
                    //TODO: this is really ugly...
                    string s = response.Headers["Set-Cookie"];

                    string[] parts = s.Split(';');
                    string[] data = parts[0].Split('=');

                    for (int i = 0; i < data.Length; i = i + 2)
                    {
                        string name = data[i];
                        string value = data[i + 1];

                        if (cookies.ContainsKey(name))
                            cookies.Remove(name);

                        cookies.Add(name, value);

                        if (name == "D" && !String.IsNullOrEmpty(value))
                        {
                            X = value.Split(',')[1];

                            //This will cause other requests to run, every info that is needed to authenticate should be set until now!
                            if(CurrentUserIdReceived != null)
                                CurrentUserIdReceived(this, new UserIdIdentifiedEventArgs(Convert.ToInt32(value.Split(',')[0])));
                        }
                    }
                }

                if (responseString.Length == 3 && isDigitsOnly(responseString))
                {
                    statusCode = (HttpStatusCode)Convert.ToInt32(responseString);

                    if (statusCode == HttpStatusCode.Forbidden)
                        throw new NotAuthorizedException();
                }

                return new SparklrResponse<string>(statusCode, responseString);
            }
        }

        /// <summary>
        /// Checks if the given string only consists of numbers
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool isDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Creates an HttpWebRequest with the appropriate headers
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpWebRequest prepareRequest(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);

            if (cookies.Count > 0)
                request.Headers["Cookie"] = String.Join(";", buildCookies());

            if (X != null)
                request.Headers["X-X"] = X;
            return request;
        }

        private IEnumerable<string> buildCookies()
        {
            foreach (KeyValuePair<string, string> kvp in cookies)
            {
                yield return String.Format("{0}={1}", kvp.Key, kvp.Value);
            }
        }
    }
}
