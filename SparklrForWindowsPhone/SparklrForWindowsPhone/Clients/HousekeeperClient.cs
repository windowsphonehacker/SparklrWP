using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SparklrForWindowsPhone.Clients
{
    public class HousekeeperClient
    {
        public bool HasLoggedin { get; set; }
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
    }
}
