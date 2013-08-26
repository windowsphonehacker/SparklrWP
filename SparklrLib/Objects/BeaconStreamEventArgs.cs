using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SparklrLib.Objects.Responses.Beacon;
namespace SparklrLib.Objects
{
    public class BeaconStreamEventArgs : SparklrEventArgs
    {
        public Stream Stream { get; set; }
        public HttpWebResponse Response { get; set; }
    }
}
