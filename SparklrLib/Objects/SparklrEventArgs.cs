using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrLib.Objects
{
    public class SparklrEventArgs
    {
        public Exception Error { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
