using System;

namespace SparklrWP.Utils
{
    public static class NetworkHelpers
    {
        public static string FormatNetworkName(string name)
        {
            if (name == null || name == "0")
                return "/";

            if (name.StartsWith("/"))
                return name;

            return String.Format("/{0}", name);
        }

        public static string UnformatNetworkName(string name)
        {
            if (String.IsNullOrEmpty(name) && name == "/")
                return "0";

            if (name.StartsWith("/"))
                return name.TrimStart('/');

            return name;
        }
    }
}
