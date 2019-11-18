using System;
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

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public uint Width { get; }
        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public uint Height => (uint) this.pixelBytes.Length / (4 * this.Width);
        /// <summary>
        ///     Gets the horizontal DPI of the image.
        /// </summary>
        /// <value>
        ///     The horizontal DPI.
        /// </value>
        public double DpiX { get; }
        /// <summary>
        ///     Gets the vertical DPI of the image.
        /// </summary>
        /// <value>
        ///     The vertical DPI.
        /// </value>
        public double DpiY { get; }

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bitmap"/> class.
        ///     Precondition: pixelBytes is not null
        ///     Postcondition: image is built
        /// </summary>
        /// <param name="pixelBytes">The pixel bytes.</param>
        /// <param name="width">The width.</param>
        /// <param name="dpix">The dpix.</param>
        /// <param name="dpiy">The dpiy.</param>
        /// <exception cref="ArgumentNullException">pixelBytes</exception>
        public Bitmap(byte[] pixelBytes, uint width, double dpix, double dpiy)
        {
            this.pixelBytes = pixelBytes ?? throw new ArgumentNullException(nameof(pixelBytes));
            this.Width = width;
            this.DpiX = dpix;
            this.DpiY = dpiy;
        }

        /// <summary>
        ///     Converts the bitmap to a writable bitmap asynchronously.
        /// </summary>
        /// <returns>This bitmap as a writable bitmap</returns>
        public async Task<WriteableBitmap> AsWritableBitmapAsync()
        {
            var writeableBitmap = new WriteableBitmap((int)this.Width, (int)this.Height);
            using (var writeStream = writeableBitmap.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.pixelBytes, 0, this.pixelBytes.Length);
                return writeableBitmap;
            }
        }

        /// <summary>
        ///     Gets the color of the specified pixel.
        ///     Precondition: Pixel is within image bounds
        ///     Postcondition: None
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The color of the pixel</returns>
        public Color GetPixelColor(int x, int y)
        {
            this.checkBounds(x, y);
            var offset = (y * (int)this.Width + x) * 4;
            var r = this.pixelBytes[offset + 2];
            var g = this.pixelBytes[offset + 1];
            var b = this.pixelBytes[offset + 0];
            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        ///     Sets the color of the specified pixel.
        ///     Precondition: Pixel is within image bounds
        ///     Postcondition: Color of the pixel is changed
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        public void SetPixelColor(int x, int y, Color color)
        {
            this.checkBounds(x, y);
            var offset = (y * (int)this.Width + x) * 4;
            this.pixelBytes[offset + 3] = color.A;
            this.pixelBytes[offset + 2] = color.R;
            this.pixelBytes[offset + 1] = color.G;
            this.pixelBytes[offset + 0] = color.B;
        }

        /// <summary>
        ///     Embeds a monochrome image into the least significant bit of the blue channel of this image.
        ///     Precondition: monochrome bitmap is not null
        ///     Postcondition: image is embedded
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <exception cref="ArgumentNullException">bitmap</exception>
        public void EmbedMonochromeImage(MonochromeBitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }
            for (var i = 0; i < bitmap.Pixels.Length; i++)
            {
                var pixelColor = this.GetPixelColor((int) (i % bitmap.Width), (int) (i / bitmap.Width));
                pixelColor.B = changeLeastSignificantBit(pixelColor.B, bitmap.Pixels[i]);
                this.SetPixelColor((int)(i % bitmap.Width), (int)(i / bitmap.Width), pixelColor);
            }
        }

        private void checkBounds(int x, int y)
        {
            if (x < 0 || x >= this.Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "X must be within image bounds");
            }
            if (y < 0 || y >= this.Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "Y must be within image bounds");
            }
        }

        private static byte changeLeastSignificantBit(byte input, bool white)
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
