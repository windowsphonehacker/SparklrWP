using System;

namespace SparklrLib.Objects
{
    public class SparklrEventArgs
    {
        public Exception Error { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
