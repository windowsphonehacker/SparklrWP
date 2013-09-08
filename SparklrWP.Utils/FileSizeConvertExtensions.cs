
namespace SparklrWP.Utils
{
    /// <summary>
    /// Contains extension methods to convert file sizes
    /// </summary>
    static class FileSizeConvertExtensions
    {
        /// <summary>
        /// Converts the number of bytes to megabytes
        /// </summary>
        /// <param name="bytes">The number of bytes</param>
        /// <returns>The number of megabytes</returns>
        public static double ConvertBytesToMegabytes(this long bytes)
        {
            return (bytes / 1024L) / 1024L;
        }

        /// <summary>
        /// Converts the number of kilobytes to megabtes
        /// </summary>
        /// <param name="kilobytes">the number of kilobytes</param>
        /// <returns>the number of megabytes</returns>
        public static double ConvertKilobytesToMegabytes(this long kilobytes)
        {
            return kilobytes / 1024d;
        }

    }
}
