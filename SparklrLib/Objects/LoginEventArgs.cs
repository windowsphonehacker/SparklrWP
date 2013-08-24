using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace SparklrLib.Objects
{
    public class LoginEventArgs
    {
        public Exception Error {get;set;}
        public bool IsSuccessful { get; set; }
        public long UserId { get; set; }
        public string AuthToken { get; set; }
        public HttpWebResponse Response { get; set; }
    }
}
