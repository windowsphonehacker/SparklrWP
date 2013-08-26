using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SparklrLib.Objects
{
    public class JSONRequestEventArgs<T> : SparklrEventArgs
    {
        public T Object { get; set; }
        public HttpWebResponse Response { get; set; }
    }
}
