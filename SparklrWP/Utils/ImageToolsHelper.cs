extern alias ImageToolsDLL;
using ImageToolsDLL::ImageTools.IO;
using ImageToolsDLL::ImageTools.IO.Bmp;
using ImageToolsDLL::ImageTools.IO.Gif;

namespace SparklrWP.Utils
{
    static class ImageToolsHelper
    {
        public static void InitializeImageTools()
        {
            Decoders.AddDecoder<GifDecoder>();
            Decoders.AddDecoder<BmpDecoder>();
        }
    }
}
