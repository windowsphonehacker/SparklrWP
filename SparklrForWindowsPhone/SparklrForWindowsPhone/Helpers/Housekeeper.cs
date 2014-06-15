using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using SparklrSharp;
using SparklrSharp.Sparklr;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;


namespace SparklrForWindowsPhone.Helpers
{
    public class Housekeeper
    {
        private IsolatedStorageSettings appSettings =
           IsolatedStorageSettings.ApplicationSettings;

        public static Connection ServiceConnection = new Connection();

        public bool LoginDataAvailable
        {
            get
            {
                return appSettings.Contains("username") && appSettings.Contains("password");
            }
        }

        public string SparklrUsername{ get; set; }
        public string SparklrPassword { get; set; }

        /// <summary>
        /// A helper to save the Sparklr login info -Suraj
        /// </summary>
        /// <param name="SparklrUsername">The User's Username</param>
        /// <param name="SparklrPassword">The User's Password</param>
        public void SaveCreds(string SparklrUsername, string SparklrPassword)
        {
            RemoveCreds();

            appSettings.Add("username", SparklrUsername);
            appSettings.Add("password", EncryptionHelper.EncryptString(SparklrPassword));

            appSettings.Save();
        }

        public void RemoveCreds()
        {
            if (appSettings.Contains("username"))
                appSettings.Remove("username");

            if (appSettings.Contains("password"))
                appSettings.Remove("password");

            appSettings.Save();
        }

        public void GetCreds()
        {
            SparklrUsername = (string)appSettings["username"];
            SparklrPassword = (byte[])appSettings["password"] == null ? null : EncryptionHelper.DecryptToString((byte[])appSettings["password"]);
        }
    }
}
