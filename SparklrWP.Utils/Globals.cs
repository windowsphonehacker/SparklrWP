
namespace SparklrWP.Utils
{
    public static class Globals
    {
        public delegate void LoggingDelegate(string format, params object[] objects);
        public static LoggingDelegate LoggingFunction;

        internal static void log(string format, params object[] objects)
        {
            if (LoggingFunction != null)
                LoggingFunction(format, objects);
        }
    }
}
