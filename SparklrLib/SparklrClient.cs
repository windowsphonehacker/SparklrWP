using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using SparklrLib.Objects;
using Newtonsoft.Json;
namespace SparklrLib
{
    public class SparklrClient
    {
        public string AuthToken { get; set; }
        public long UserId { get; private set; }

        public const string BaseURI = "https://sparklr.me/";

        public SparklrClient()
        {
        }

        public HttpWebRequest CreateRequest(string path)
        {
            return CreateRequest(path, "");
        }
        public HttpWebRequest CreateRequest(string path, string xdata)
        {
            if (path[0] == '/') path = path.Substring(1);
            HttpWebRequest newReq = HttpWebRequest.CreateHttp(BaseURI + path);
            if (AuthToken != null)
            {
                newReq.Headers["Cookie"] = "D=" + UserId.ToString() + ',' + AuthToken;
            }
            newReq.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.57 Safari/537.36";
            if (xdata != "")
            {
                newReq.Headers["X-Data"] = xdata;
            }
            if (AuthToken != null)
            {
                newReq.Headers["X-X"] = AuthToken;
            }
            return newReq;
        }

        private void requestJsonObject<T>(string path, Action<JSONRequestEventArgs<T>> Callback){
            requestJsonObject<T>(path,"","", Callback);
        }

        private void requestJsonObject<T>(string path, object xdata, Action<JSONRequestEventArgs<T>> Callback)
        {
            requestJsonObject<T>(path, JsonConvert.SerializeObject(xdata), "", Callback);
        }

        private void requestJsonObject<T>(string path, string xdata, Action<JSONRequestEventArgs<T>> Callback){
            requestJsonObject<T>(path,xdata,"", Callback);
        }

        private void requestJsonObject<T>(string path, object xdata, string postdata, Action<JSONRequestEventArgs<T>> Callback)
        {
            requestJsonObject<T>(path, JsonConvert.SerializeObject(xdata), postdata, Callback);
        }

        private void requestJsonObject<T>(string path, string xdata, string postdata, Action<JSONRequestEventArgs<T>> Callback)
        {
            HttpWebRequest streamReq = CreateRequest(path,xdata);
            Action getResponse = () => {
                streamReq.BeginGetResponse((res) =>
                {
                    HttpWebResponse streamResp = null;
                    try
                    {
                        streamResp = (HttpWebResponse)streamReq.EndGetResponse(res);
                    }
                    catch (WebException ex)
                    {
                        Callback(new JSONRequestEventArgs<T>()
                        {
                            IsSuccessful = false,
                            Error = ex,
                            Response = (HttpWebResponse)ex.Response
                        });
                        return;
                    }
                    catch (Exception ex)
                    {
                        Callback(new JSONRequestEventArgs<T>()
                        {
                            IsSuccessful = false,
                            Error = ex
                        });
                        return;
                    }
                    T desiredObject = default(T);
                    using (StreamReader strReader = new StreamReader(streamResp.GetResponseStream(), Encoding.UTF8))
                    {
                        try
                        {
                            string json = strReader.ReadToEnd();
                            desiredObject = JsonConvert.DeserializeObject<T>(json);
                        }
                        catch (Exception ex)
                        {
                            Callback(new JSONRequestEventArgs<T>()
                            {
                                IsSuccessful = false,
                                Error = ex,
                                Response = streamResp
                            });
                            return;
                        }
                    }
                    Callback(new JSONRequestEventArgs<T>()
                    {
                        IsSuccessful = true,
                        Error = null,
                        Object = desiredObject
                    });
                },null);
            };
            if(postdata == ""){
                getResponse();
            }else{
                streamReq.Method = "POST";
                streamReq.BeginGetRequestStream((res) => {
                    using (Stream postStream = streamReq.EndGetRequestStream(res))
                    {
                        // Create the post data
                        byte[] byteArray = Encoding.UTF8.GetBytes(postdata);
                        // Add the post data to the web request
                        postStream.Write(byteArray, 0, byteArray.Length);
                    }
                    getResponse();
                },null);
            }
        }

        public void Login(string Username, string Password, Action<LoginEventArgs> Callback)
        {
            HttpWebRequest loginReq = CreateRequest("work/signin/" + Username + "/" + Password + "/");
            loginReq.BeginGetResponse((res) =>
            {
                HttpWebResponse loginResp = null;
                try
                {
                    loginResp = (HttpWebResponse)loginReq.EndGetResponse(res);
                }
                catch (WebException ex)
                {
                    Callback(new LoginEventArgs()
                    {
                        IsSuccessful = false,
                        Error = ex,
                        Response = (HttpWebResponse)ex.Response
                    });
                    return;
                }
                catch (Exception ex)
                {
                    Callback(new LoginEventArgs()
                    {
                        IsSuccessful = false,
                        Error = ex,
                        Response = loginResp
                    });
                    return;
                }
                if (loginResp.Headers["Set-Cookie"] == null)
                {
                    Callback(new LoginEventArgs()
                    {
                        Error = new Exception("Didn't receive Auth token"),
                        IsSuccessful = false,
                        Response = loginResp
                    });
                    return;
                }
                string[] cookieParts = loginResp.Headers["Set-Cookie"].Split(';');
                string cookieD = "";
                foreach(string sortaCookie in cookieParts){
                    string sortaTrimmedCookie = sortaCookie.TrimStart();
                    if(sortaTrimmedCookie.StartsWith("D=")){
                        cookieD = sortaTrimmedCookie.Substring(2);
                        break;
                    }
                }
                if(cookieD.Length == 0){
                    Callback(new LoginEventArgs()
                    {
                        Error = new Exception("Auth token not included"),
                        IsSuccessful = false,
                        Response = loginResp
                    });
                    return;
                }
                string[] loginBits = cookieD.Split(',');
                if(loginBits.Length < 2){
                    Callback(new LoginEventArgs()
                    {
                        Error = new Exception("Auth token is corrupted"),
                        IsSuccessful = false,
                        Response = loginResp
                    });
                    return;
                }
                try
                {
                    UserId = long.Parse(loginBits[0]);
                }catch(Exception e){
                    Callback(new LoginEventArgs() { 
                        Error = new Exception("Auth token is corrupted"),
                        IsSuccessful = false,
                        Response = loginResp
                    });
                    return;
                }
                AuthToken = loginBits[1];
                Callback(new LoginEventArgs()
                {
                    Error = null,
                    IsSuccessful = true,
                    Response = loginResp,
                    AuthToken = AuthToken,
                    UserId = UserId
                });
            }, null);
        }

        public void GetBeaconStream(Action<JSONRequestEventArgs<Objects.Responses.Beacon.Stream>> Callback)
        {
            GetBeaconStream(0, 20, 0, Callback);
        }

        public void GetBeaconStream(int lastTime, Action<JSONRequestEventArgs<Objects.Responses.Beacon.Stream>> Callback)
        {
            GetBeaconStream(lastTime, 0, 0, Callback);
        }

        public void GetBeaconStream(int lastTime, int amount, Action<JSONRequestEventArgs<Objects.Responses.Beacon.Stream>> Callback)
        {
            GetBeaconStream(lastTime, amount, 0, Callback);
        }

        public void GetBeaconStream(int lastTime,int amount,int network,Action<JSONRequestEventArgs<Objects.Responses.Beacon.Stream>> Callback)
        {
            int stream = 0;
#if DEBUG
            stream = 2;
            network = 1;
#endif
            requestJsonObject<Objects.Responses.Beacon.Stream>("/beacon/stream/"+stream+"?since=" + lastTime.ToString() + "&n=" + amount.ToString() + (network != 0 ? "&network=" + network.ToString() : ""), Callback);
        }

        public void Post(string message, Stream image, Action<SparklrEventArgs> Callback)
        {
            string data64str = "";
            if(image != null){
                using (MemoryStream ms = new MemoryStream())
                {
#if PORTABLELIB
                    byte[] array = new byte[81920];
                    int count;
                    while ((count = image.Read(array, 0, array.Length)) != 0)
                    {
                       ms.Write(array, 0, count);
                    }
#else
                    image.CopyTo(ms);
#endif
                    data64str = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            requestJsonObject<Objects.Responses.Generic>("/work/post", new Objects.Requests.Work.Post()
            {
                body = message,
#if DEBUG
                network = 2,
#endif
                img = data64str != ""
            }, data64str, (args) =>
            {
                Callback(new SparklrEventArgs() { 
                    IsSuccessful = args.IsSuccessful && args.Object.error == null,
                    Error = args.IsSuccessful?args.Object.error==true?new Exception("Sparklr said noooooo"):null:args.Error
                });
            });
        }
    }
}
