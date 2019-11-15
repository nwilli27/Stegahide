using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Implementation of a bitmap image that allows for low-level bit manipulation.
    /// </summary>
    public class Bitmap
    {
        private readonly byte[] pixels;

        #region Properties

        public uint Width { get; }
        public uint Height => (uint) this.pixels.Length / this.Width;
        public uint DpiX { get; }
        public uint DpiY { get; }

        #endregion

        public Bitmap(byte[] pixels, uint width, uint dpix, uint dpiy)
        {
            this.pixels = pixels;
            this.Width = width;
            this.DpiX = dpix;
            this.DpiY = dpiy;
        }

        public async Task<WriteableBitmap> AsWritableBitmapAsync()
        {
            var writeableBitmap = new WriteableBitmap((int)this.Width, (int)this.Height);
            using (var writeStream = writeableBitmap.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.pixels, 0, this.pixels.Length);
                return writeableBitmap;
            }
        }

        private Color getPixelBgra8(int x, int y)
        {
            var offset = (x * (int)this.Width + y) * 4;
            var r = this.pixels[offset + 2];
            var g = this.pixels[offset + 1];
            var b = this.pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        public void EmbedMonochromeImage(bool[] pixels, uint width)
        {
            for (var i = 0; i < pixels.Length; i++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixelColor = this.getPixelBgra8(x, (int) (i / width));
                    pixelColor.B = this.changeLeastSignificantBit(pixelColor.B, pixels[i]);
                }
            }
        }

        private byte changeLeastSignificantBit(byte input, bool white)
        {
            if (white)
            {
                return (byte) (input | 1);
            }
            else
            {
                return (byte) (input & 0xFE);
            }
        }
    }
}
