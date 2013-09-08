using System;
using System.Windows.Threading;

namespace SparklrWP.Utils
{
    /// <summary>
    /// Provides access to a smart invoke method, that only dispatchs if required.
    /// </summary>
    public static class SmartDispatcher
    {
        private static Dispatcher dispatcher;

        static SmartDispatcher()
        {
            dispatcher = System.Windows.Deployment.Current.Dispatcher;
        }

        /// <summary>
        /// Dispatches the given action to the UI thread
        /// </summary>
        /// <param name="a">The action to execute</param>
        public static void BeginInvoke(Action a)
        {
            if (dispatcher.CheckAccess())
            {
                a();
            }
            else
            {
                dispatcher.BeginInvoke(a);
            }
        }
    }
}
