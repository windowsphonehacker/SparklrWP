using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using SparklrSharp;
using SparklrSharp.Sparklr;
using System.IO.IsolatedStorage;


namespace SparklrForWindowsPhone.Helpers
{
    public class Housekeeper
    {
        private IsolatedStorageSettings appSettings =
           IsolatedStorageSettings.ApplicationSettings;

        public bool HasLoggedin { get; set; }
        public string SparklrUsername{ get; set; }
        public string SparklrPassword { get; set; }
        /// <summary>
        /// Checks if the user has logged in
        /// </summary>
        public void CheckCreds()
        {
            if(System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("username") && System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Contains("password"))
            {
                HasLoggedin = true;
            }
            else
            {
                HasLoggedin = false;
            }
            
        }
        /// <summary>
        /// A helper to save the Sparklr login info -Suraj
        /// </summary>
        /// <param name="SparklrUsername">The User's Username</param>
        /// <param name="SparklrPassword">The User's Password</param>
        public void SaveCreds(string SparklrUsername, string SparklrPassword)
        {
            try
            {
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Add("username", SparklrUsername);
                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.Add("password", SparklrPassword);
            }
            catch
            {
                //ToDo: Add logic to delete and re-enter the key
            }
        }

        public void GetCreds()
        {
            SparklrUsername = (string)appSettings["username"];
            SparklrPassword = (string)appSettings["password"];
        }
    }
}
