using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if PORTABLELIB
namespace System.Net
{
    internal class HttpUtility
    {
        internal static string UrlEncode(string keyword)
        {
            return Uri.EscapeDataString(keyword);
        }
    }
}
#endif