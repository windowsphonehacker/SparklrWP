using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrForWindowsPhone.Helpers
{
    /// <summary>
    /// Handles the display of the global loading Indicator. The indicator will be visible, until Stop() has been called for each Start().
    /// </summary>
    public static class GlobalLoadingIndicator
    {
        static GlobalLoadingIndicator()
        {
            if (Microsoft.Phone.Shell.SystemTray.ProgressIndicator == null)
                Microsoft.Phone.Shell.SystemTray.ProgressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator.IsIndeterminate = true;
        }

        private static int loadingCount = 0;

        /// <summary>
        /// Signals the start of a time consuming process
        /// </summary>
        public static void Start()
        {
            
            loadingCount++;
            refreshVisibility();
        }

        /// <summary>
        /// Signals the end of a time consuming process
        /// </summary>
        public static void Stop()
        {
            loadingCount--;
            refreshVisibility();
        }

        private static void refreshVisibility()
        {
            Microsoft.Phone.Shell.SystemTray.ProgressIndicator.IsVisible = (loadingCount != 0);
        }
    }
}
