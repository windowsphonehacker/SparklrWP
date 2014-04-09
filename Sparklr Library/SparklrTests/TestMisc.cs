using System;
using SparklrSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SparklrSharp.Sparklr;

namespace SparklrTests
{
    [TestClass]
    public class TestMisc
    {
        [TestMethod]
        public async Task TestAwake()
        {
            Connection conn = new Connection();

            bool result = await conn.GetAwakeAsync();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task TestGetStaff()
        {
            Connection conn = await Credentials.CreateSession();
            User[] staff = await conn.GetStaffAsync();

            Assert.AreEqual(staff.Length, 6);
        }
    }
}
