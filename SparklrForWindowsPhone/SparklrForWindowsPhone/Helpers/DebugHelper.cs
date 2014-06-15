using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrForWindowsPhone.Helpers
{
    public static class DebugHelper
    {
        public static void LogDebugMessage(string message)
        {
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
#endif
        }

        public static void LogDebugMessage(string format, params object[] args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(format, args);
            }
#endif
        }
    }
}
