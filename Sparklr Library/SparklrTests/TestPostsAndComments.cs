using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparklrSharp;
using SparklrSharp.Sparklr;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SparklrTests
{
    //TODO: Split into multiple classes once more functionality is implemented
    [TestClass]
    public class TestPostsAndComments
    {
        //ToDo: User more solid test cases
        [TestMethod]
        public async Task TestRegularPostAndCommentRetreival()
        {
            Connection conn = await Credentials.CreateSession();
            
            Post p = await conn.GetPostByIdAsync(151568);

            Assert.AreEqual(p.Author.UserId, 591);

            IReadOnlyCollection<Comment> comments = await p.GetCommentsAsync(conn);
            Assert.IsTrue(comments.Count == p.CommentCount);
        }

        [TestMethod]
        public async Task TestRepostedAndCommentRetreival()
        {
            Connection conn = await Credentials.CreateSession();

            Post p = await conn.GetPostByIdAsync(156397);

            Assert.AreEqual(p.Author.UserId, 591);

            IReadOnlyCollection<Comment> comments = await p.GetCommentsAsync(conn);
            Assert.IsTrue(comments.Count == p.CommentCount);

            Post original = await conn.GetPostByIdAsync(151568);

            Assert.IsNotNull(p.OriginalPost);
            Assert.AreSame(original, p.OriginalPost);
        }
    }
}
