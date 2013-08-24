using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SparklrLib
{
    public class SparklrClient
    {
        private string Cookies { get; set; }
        public string LoginToken { get; set; }
        public int UserID { get; private set; }

        public const string BaseURI = "https://sparklr.me/";

        public SparklrClient()
        {
        }

        public void BeginRequest(Func<string, bool> callback, string url, string data = "", string postData = "")
        {
            // Create a HttpWebRequest.
            Uri uri = new Uri(BaseURI + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.Headers["User-Agent"] = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0";
            //request.Headers["Referer"] = loginBrowser.Source.ToString();
            if (Cookies != null)
            {
                request.Headers["Cookie"] = Cookies;
            }
            if (data != "")
            {
                request.Headers["X-Data"] = data;
            } if (LoginToken != null)
            {
                request.Headers["X-X"] = LoginToken;
            }
            request.ContentType = "application/json";
            if (postData != "")
            {
                request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), new object[] { request, callback, postData });
            }
            else
            {
                // Send the request.
                request.BeginGetResponse(new AsyncCallback(ReadCallback), new object[] { request, callback });
            }
        }

        void GetRequestStreamCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)((object[])ar.AsyncState)[0];
            // End the stream request operation
            using (Stream postStream = request.EndGetRequestStream(ar))
            {
                // Create the post data
                string postData = (string)((object[])ar.AsyncState)[2];
                ((object[])ar.AsyncState)[2] = "";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Add the post data to the web request
                postStream.Write(byteArray, 0, byteArray.Length);
            }

            // Send the request.
            request.BeginGetResponse(new AsyncCallback(ReadCallback), ar.AsyncState);
        }


        private void ReadCallback(IAsyncResult ar)
        {
            WebResponse response;
            Func<string, bool> newCallback = (Func<string, bool>)((object[])ar.AsyncState)[1];
            try
            {
                HttpWebRequest request = (HttpWebRequest)((object[])ar.AsyncState)[0];
                response = request.EndGetResponse(ar);
                string setcooks = response.Headers["Set-Cookie"];
                if (setcooks != null && setcooks.Length > 0)
                {
                    Cookies = setcooks;
                    var cooks = setcooks.Split(';');
                    foreach (var cook in cooks)
                    {
                        if (cook.ToLower().StartsWith("d="))
                        {
                            var split = cook.Substring(3).Split(',');
                            try
                            {
                                UserID = Int32.Parse(split[0]);
                            }
                            catch (Exception) { }
                            LoginToken = split[1];
                        }
                    }
                }
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Got auth cookie: " + LoginToken);
#endif
            }
            catch (WebException ex)
            {
                response = ex.Response;
            }
#if !DEBUG
            catch (Exception ex)
            {
                return;
            }
#endif

            string str = "";
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                str = sr.ReadToEnd();
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Got response: " + str);
#endif
                //TODO: Handle responses and add a response event
                if (newCallback != null)
                {
                    newCallback.Invoke(str);
                }
            }
        }

        //TODO: Separate these:
        public event LoggedInEvent LoggedIn;
        public delegate void LoggedInEvent(object sender, LoggedInEventArgs e);
        public bool IsLoggedIn { get; set; }
        public class LoggedInEventArgs : EventArgs{
            public bool Error { get; set; }
        }

        public void Login(string username, string password)
        {
            BeginRequest((string str) =>
            {
                if (str.ToLower().Contains("error"))
                {
                    IsLoggedIn = false;
                }
                else
                {
                    IsLoggedIn = true;
                }
                if (LoggedIn != null)
                {
                    LoggedIn.Invoke(this, new LoggedInEventArgs() { Error = !IsLoggedIn });
                }
                return true;
            }, "work/signin/" + username + "/" + password + "/");
        }
    }
}
