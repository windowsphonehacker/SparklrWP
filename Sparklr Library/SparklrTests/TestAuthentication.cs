using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparklrSharp;
using System.Threading.Tasks;
using System.Threading;

namespace SparklrTests
{
    [TestClass]
    public class TestAuthentication
    {
        [TestMethod]
        public async Task TestInvalidAuthentication()
        {
            Connection conn = new Connection();
            bool result = await conn.SigninAsync(Credentials.ValidUsername, Credentials.InvalidPassword);
            Assert.IsFalse(result);
        }

        private bool eventRaised = false;
        private EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.ManualReset);

        [TestMethod]
        public async Task TestValidAuthentication()
        {
            Connection conn = new Connection();
            conn.CurrentUserIdentified += conn_CurrentUserIdentified;
            bool result = await conn.SigninAsync(Credentials.ValidUsername, Credentials.ValidPassword);
            Assert.IsTrue(result);
            Assert.IsTrue(conn.Authenticated);

            //Wait a maximum of 5 seconds.
            handle.WaitOne(5000);

            Assert.IsTrue(eventRaised);
            Assert.AreEqual(Credentials.ValidUsername, conn.CurrentUser.Name, true);

            await conn.SignoffAsync();

            Assert.IsFalse(conn.Authenticated);
            Assert.IsNull(conn.CurrentUser);
        }

        void conn_CurrentUserIdentified(object sender, SparklrSharp.Sparklr.UserIdentifiedEventArgs e)
        {
            eventRaised = true;
            handle.Set();
        }

        [TestMethod]
        public async Task TestLogout()
        {
            Connection conn = new Connection();
            bool result = await conn.SigninAsync(Credentials.ValidUsername, Credentials.ValidPassword);
            Assert.IsTrue(result);
            Assert.IsTrue(conn.Authenticated);
            result = await conn.SignoffAsync();
            Assert.IsTrue(result);
            Assert.IsNull(conn.CurrentUser);
            Assert.IsFalse(conn.Authenticated);
        }
    }
}
