using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparklrSharp;
using System.Threading.Tasks;

namespace SparklrTests
{
    [TestClass]
    public class TestNotifications
    {
        [TestMethod]
        [ExpectedException(typeof(SparklrSharp.Exceptions.NotAuthorizedException))]
        public async Task TestNotificationsNotAuthorized()
        {
            Connection conn = new Connection();
            await conn.GetNotificationsAsync();
        }

        [TestMethod]
        public async Task TestNotificationsAuthorized()
        {
            Connection conn = await Credentials.CreateSession();
            await conn.GetNotificationsAsync();
        }
    }
}
