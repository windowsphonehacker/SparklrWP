using SparklrSharp.Communications;
using SparklrSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SparklrSharp.Sparklr;

namespace SparklrSharp
{
    public partial class Connection
    {
        /// <summary>
        /// handles communication with the SparklrServer
        /// </summary>
        private Communications.WebClient webClient;

        /// <summary>
        /// The currently authenticated user
        /// </summary>
        public User CurrentUser { get; private set; }

        /// <summary>
        /// Contains if the connection is authenticated TODO: not reliable when errors are thrown.
        /// </summary>
        internal bool Authenticated { get; private set; }

        /// <summary>
        /// Occurs when the current user was identified.
        /// </summary>
        public event EventHandler<UserIdentifiedEventArgs> CurrentUserIdentified;

        /// <summary>
        /// Creates a new instance of Connection
        /// </summary>
        public Connection()
        {
            webClient = new Communications.WebClient();

            webClient.CurrentUserIdReceived += webClient_CurrentUserIdReceived;
        }

        private async void webClient_CurrentUserIdReceived(object sender, UserIdIdentifiedEventArgs e)
        {
            User cu = await this.GetUserAsync(e.IdentifiedUserId);

            if (Authenticated)
            {
                CurrentUser = cu;

                if (CurrentUserIdentified != null)
                    CurrentUserIdentified(this, new UserIdentifiedEventArgs(this.CurrentUser));
            }
        }

        /// <summary>
        /// Checks if sparklr is running
        /// </summary>
        /// <returns>true if it is, otherwise false</returns>
        public async Task<bool> GetAwakeAsync()
        {
            SparklrResponse<string> response = await webClient.GetRawResponseAsync("areyouawake");

            return response.IsOkAndTrue();
        }

        /// <summary>
        /// Retreives the staff members from sparklr
        /// </summary>
        /// <returns>An Array of Staff members</returns>
        public async Task<User[]> GetStaffAsync()
        {
            SparklrResponse<SparklrSharp.JSONRepresentations.Get.UserMinimal[]> response = await webClient.GetJSONResponseAsync<SparklrSharp.JSONRepresentations.Get.UserMinimal[]>("staff");

            User[] staffMembers = new User[response.Response.Length];

            for (int i = 0; i < response.Response.Length; i++)
            {
                staffMembers[i] = await User.InstanciateUserAsync(response.Response[i].id, this);
            }

            return staffMembers;
        }
    }
}
