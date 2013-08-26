using SparklrLib.Objects.Responses.Beacon;
using System.Net;
namespace SparklrLib.Objects
{
    public class BeaconStreamEventArgs : SparklrEventArgs
    {
        public Stream Stream { get; set; }
        public HttpWebResponse Response { get; set; }
    }
}
