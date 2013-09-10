using System;

namespace SparklrWP.Utils
{
    public static class NetworkHelpers
    {
        public static string FormatNetworkName(string name)
        {
            if (name == "0")
                return "/";

            return String.Format("/{0}", name);
        }
    }
}
