using System;
using System.Net;

namespace SparklrWP.Utils.Extensions
{
    public static class StringExtensions
    {
        public static String EncodeUrl(this String source)
        {
            return HttpUtility.UrlEncode(source);
        }

        public static String DecodeUrl(this String source)
        {
            return HttpUtility.UrlDecode(source);
        }
    }
}
