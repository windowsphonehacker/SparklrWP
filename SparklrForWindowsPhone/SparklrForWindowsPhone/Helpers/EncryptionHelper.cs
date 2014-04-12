using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SparklrForWindowsPhone.Helpers
{
    public static class EncryptionHelper
    {
        public static byte[] EncryptString(string s)
        {
            //TODO: Use optional entropy
            return ProtectedData.Protect(StringToByteArray(s), null);
        }

        public static string DecryptToString(byte[] s)
        {
            byte[] unprotectedS = ProtectedData.Unprotect(s, null);
            return ByteArrayToString(unprotectedS);
        }

        private static byte[] StringToByteArray(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        private static string ByteArrayToString(byte [] b)
        {
            return Encoding.UTF8.GetString(b, 0, b.Length);
        }
    }
}
