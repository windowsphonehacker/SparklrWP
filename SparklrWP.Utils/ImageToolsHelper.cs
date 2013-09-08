using ImageTools.IO;
using  ImageTools.IO.Gif;

namespace SparklrWP.Utils
{
    static class ImageToolsHelper
    {
        public static void InitializeImageTools()
        {
            Decoders.AddDecoder<GifDecoder>();
        }
    }
}
