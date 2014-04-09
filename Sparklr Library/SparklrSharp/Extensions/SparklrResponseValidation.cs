using SparklrSharp.Communications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparklrSharp.Extensions
{
    internal static class SparklrResponseValidation
    {
        internal static bool IsOkAndTrue(this SparklrResponse<string> response)
        {
            return (response.Code == System.Net.HttpStatusCode.OK) && (response.Response == "true");
        }

        internal static bool IsOkAndFalse(this SparklrResponse<string> response)
        {
            return (response.Code == System.Net.HttpStatusCode.OK) && (response.Response == "false");
        }
    }
}
