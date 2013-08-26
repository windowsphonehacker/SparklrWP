using System.Net;

namespace SparklrLib.Objects
{
    public class JSONRequestEventArgs<T> : SparklrEventArgs
    {
        public T Object { get; set; }
        public HttpWebResponse Response { get; set; }
    }
}
