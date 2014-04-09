using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparklrSharp;
using SparklrSharp.Sparklr;
using System.Threading.Tasks;

namespace SparklrTests
{
    [TestClass]
    public class TestUsers
    {
        private const int testUserId = 591;
        private const string testHandle = "ChrisK";
        private const long testAvatarId = 1390042494;
        private const bool testFollowing = true;
        private const string testName = "ChrisK";
        private const string testBio = "Can't really tell much about myself. I'm from Germany and currently going to university, studying medicine. If you have any questions, feel free to ask :)";

        [TestMethod]
        [ExpectedException(typeof(SparklrSharp.Exceptions.NotAuthorizedException))]
        public async Task TestUsersWithoutAuth()
        {
            Connection conn = new Connection();
            User u = await conn.GetUserAsync(100);
        }

        [TestMethod]
        public async Task TestValidUser()
        {
            Connection conn = await Credentials.CreateSession();

            User u = await conn.GetUserAsync(testUserId);

            Assert.AreEqual(u.AvatarId, testAvatarId);
            Assert.AreEqual(u.Bio, testBio);
            Assert.AreEqual(u.Following, testFollowing);
            Assert.AreEqual(u.Handle, testHandle);
            Assert.AreEqual(u.Name, testName);
            Assert.AreEqual(u.UserId, testUserId);
        }

        [TestMethod]
        [ExpectedException(typeof(SparklrSharp.Exceptions.NoDataFoundException))]
        public async Task TestInvalidUser()
        {
            Connection conn = await Credentials.CreateSession();
            User u = await conn.GetUserAsync(int.MaxValue);
        }
    }
}
