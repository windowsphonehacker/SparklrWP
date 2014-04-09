using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrTests
{
    [TestClass]
    public class Benchmarking
    {
        private static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        [AssemblyInitialize]
        public static void Initialze(TestContext ctx)
        {
            sw.Start();
        }

        [AssemblyCleanup]
        public static void Finish()
        {
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Finnished in {0}ms", sw.ElapsedMilliseconds);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Number of responses received: {0}", SparklrSharp.Communications.WebClient.DEBUG_NumberOfRequestReceived);
#endif
        }
    }
}
