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
        private readonly byte[] pixelBytes;

        #region Properties

        public uint Width { get; }
        public uint Height => (uint) this.pixelBytes.Length / (4 * this.Width);
        public double DpiX { get; }
        public double DpiY { get; }

        #endregion

        public Bitmap(byte[] pixelBytes, uint width, double dpix, double dpiy)
        {
            this.pixelBytes = pixelBytes;
            this.Width = width;
            this.DpiX = dpix;
            this.DpiY = dpiy;
        }

        public async Task<WriteableBitmap> AsWritableBitmapAsync()
        {
            var writeableBitmap = new WriteableBitmap((int)this.Width, (int)this.Height);
            using (var writeStream = writeableBitmap.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.pixelBytes, 0, this.pixelBytes.Length);
                return writeableBitmap;
            }
        }

        public Color GetPixelColor(int x, int y)
        {
            var offset = (y * (int)this.Width + x) * 4;
            var r = this.pixelBytes[offset + 2];
            var g = this.pixelBytes[offset + 1];
            var b = this.pixelBytes[offset + 0];
            return Color.FromArgb(255, r, g, b);
        }

        public void SetPixelColor(int x, int y, Color color)
        {
            var offset = (y * (int)this.Width + x) * 4;
            this.pixelBytes[offset + 3] = color.A;
            this.pixelBytes[offset + 2] = color.R;
            this.pixelBytes[offset + 1] = color.G;
            this.pixelBytes[offset + 0] = color.B;
        }

        public void EmbedMonochromeImage(MonochromeBitmap bitmap)
        {
            for (var i = 0; i < bitmap.Pixels.Length; i++)
            {
                var pixelColor = this.GetPixelColor((int) (i % bitmap.Width), (int) (i / bitmap.Width));
                pixelColor.B = this.changeLeastSignificantBit(pixelColor.B, bitmap.Pixels[i]);
                this.SetPixelColor((int)(i % bitmap.Width), (int)(i / bitmap.Width), pixelColor);
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
