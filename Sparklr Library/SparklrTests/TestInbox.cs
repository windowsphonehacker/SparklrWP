using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SparklrSharp;
using SparklrSharp.Sparklr;

namespace SparklrTests
{
    [TestClass]
    public class TestInbox
    {
        [TestMethod]
        [ExpectedException(typeof(SparklrSharp.Exceptions.NotAuthorizedException))]
        public async Task TestNotAuthorized()
        {
            Connection conn = new Connection();
            var response = await conn.GetInboxAsync();
        }

        [TestMethod]
        public async Task TestInboxAuthorized()
        {
            Connection conn = await Credentials.CreateSession();
            var result = await conn.GetInboxAsync();

            Assert.IsTrue(result.Length >= 1);
        }

        [TestMethod]
        public async Task TestInboxRefresh()
        {
            Connection conn = await Credentials.CreateSession();

            Assert.IsNull(conn.Inbox);
            await conn.RefreshInboxAsync();
            Assert.IsTrue(conn.Inbox.Count >= 1);
        }
    }
}
