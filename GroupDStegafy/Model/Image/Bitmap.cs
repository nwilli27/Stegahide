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
        public byte[] PixelBytes { get; }

        #region Properties

        public uint Width { get; }
        public uint Height => (uint) this.PixelBytes.Length / (4 * this.Width);
        public uint DpiX { get; }
        public uint DpiY { get; }

        #endregion

        public Bitmap(byte[] pixelBytes, uint width, uint dpix, uint dpiy)
        {
            this.PixelBytes = pixelBytes;
            this.Width = width;
            this.DpiX = dpix;
            this.DpiY = dpiy;
        }

        public async Task<WriteableBitmap> AsWritableBitmapAsync()
        {
            var writeableBitmap = new WriteableBitmap((int)this.Width, (int)this.Height);
            using (var writeStream = writeableBitmap.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.PixelBytes, 0, this.PixelBytes.Length);
                return writeableBitmap;
            }
        }

        public Color GetPixelBgra8(int x, int y)
        {
            var offset = (y * (int)this.Width + x) * 4;
            var r = this.PixelBytes[offset + 2];
            var g = this.PixelBytes[offset + 1];
            var b = this.PixelBytes[offset + 0];
            return Color.FromArgb(255, r, g, b);
        }

        public void SetPixelBgra8(int x, int y, Color color)
        {
            var offset = (y * (int)this.Width + x) * 4;
            this.PixelBytes[offset + 3] = color.A;
            this.PixelBytes[offset + 2] = color.R;
            this.PixelBytes[offset + 1] = color.G;
            this.PixelBytes[offset + 0] = color.B;
        }

        public void EmbedMonochromeImage(bool[] pixels, uint width)
        {
            for (var i = 0; i < pixels.Length; i++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixelColor = this.GetPixelBgra8(x, (int) (i / width));
                    pixelColor.B = this.changeLeastSignificantBit(pixelColor.B, pixels[i]);
                    this.SetPixelBgra8(x, (int) (i / width), pixelColor);
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
