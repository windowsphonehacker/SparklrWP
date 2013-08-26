﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace SparklrLib.Objects
{
    public class LoginEventArgs : SparklrEventArgs
    {
        public long UserId { get; set; }
        public string AuthToken { get; set; }
        public HttpWebResponse Response { get; set; }
    }
}
