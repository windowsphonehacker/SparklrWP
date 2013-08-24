using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using SparklrLib.Objects;
namespace SparklrLib
{
    public class SparklrClientEOC
    {
        public string AuthToken { get; set; }
        public long UserId { get; private set; }

        public const string BaseURI = "https://sparklr.me/";

        public SparklrClientEOC()
        {
        }

        public HttpWebRequest CreateRequest(string Path)
        {
            return CreateRequest(Path, "");
        }
        public HttpWebRequest CreateRequest(string Path, string XData)
        {

            HttpWebRequest newReq = HttpWebRequest.CreateHttp(BaseURI + Path);
            if (AuthToken != null)
            {
                newReq.Headers["Cookie"] = UserId.ToString() + ',' + AuthToken;
            }
            newReq.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.57 Safari/537.36";
            if (XData != "")
            {
                newReq.Headers["X-Data"] = XData;
            }
            if (AuthToken != null)
            {
                newReq.Headers["X-X"] = AuthToken;
            }
            return newReq;
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
    }
}
