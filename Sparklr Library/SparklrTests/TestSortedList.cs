using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparklrSharp.Collections;

namespace SparklrTests
{
    [TestClass]
    public class TestSortedList
    {
        [TestMethod]
        public void TestIntSorting()
        {
            SortedList<Int32> list = new SortedList<Int32>();

            Int32[] sortUs = new Int32[] { 1, 5, 7, 4, 3, 2, 2, 8, 10, -15, 18, 27, 98, 76 };

            foreach (Int32 i in sortUs)
                list.Add(i);

            for (int i = 1; i < list.Count; i++)
                Assert.IsTrue(list[i - 1] <= list[i]);
        }
    }
}
