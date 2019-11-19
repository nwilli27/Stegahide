using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Implementation of a bitmap image that allows for low-level bit manipulation.
    /// </summary>
    public class Bitmap : Image
    {
        #region Data Members

        private readonly byte[] pixelBytes;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dpi x.
        /// </summary>
        /// <value>
        /// The dpi x.
        /// </value>
        public uint DpiX { get; }

        /// <summary>
        /// Gets the dpi y.
        /// </summary>
        /// <value>
        /// The dpi y.
        /// </value>
        public uint DpiY { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Bitmap"/> class.
        /// </summary>
        /// <param name="pixelBytes">The pixel bytes.</param>
        /// <param name="width">The width.</param>
        /// <param name="dpix">The dpix.</param>
        /// <param name="dpiy">The dpiy.</param>
        public Bitmap(byte[] pixelBytes, uint width, uint dpix, uint dpiy)
        {
            this.pixelBytes = pixelBytes;
            this.Width = width;
            this.Height = (uint)this.pixelBytes.Length / (4 * this.Width);
            this.DpiX = dpix;
            this.DpiY = dpiy;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a Bitmap as a writable bitmap.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <returns>A writable bitmap image.</returns>
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
        ///     Gets the pixel bgra8 color values.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>A color object with its bgra8 values.</returns>
        public Color GetPixelBgra8(int x, int y)
        {
            var offset = (y * (int)this.Width + x) * 4;
            var r = this.pixelBytes[offset + 2];
            var g = this.pixelBytes[offset + 1];
            var b = this.pixelBytes[offset + 0];
            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        ///     Sets the pixel bgra8 to the [color] object.
        ///     Precondition: none
        ///     Post-condition: sets each color
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        public void SetPixelBgra8(int x, int y, Color color)
        {
            var offset = (y * (int)this.Width + x) * 4;
            this.pixelBytes[offset + 3] = color.A;
            this.pixelBytes[offset + 2] = color.R;
            this.pixelBytes[offset + 1] = color.G;
            this.pixelBytes[offset + 0] = color.B;
        }

        /// <summary>
        ///     Embeds the monochrome image in the source bitmap image by
        ///     using the least significant bit to alter the color.
        ///     Precondition: none
        ///     Post-condition: @each significant bit in the source image is replaced.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public void EmbedMonochromeImage(MonochromeBitmap bitmap)
        {
            for (var i = 0; i < bitmap.Pixels.Length; i++)
            {
                var pixelColor = this.GetPixelBgra8((int) (i % bitmap.Width), (int) (i / bitmap.Width));
                pixelColor.B = this.changeLeastSignificantBit(pixelColor.B, bitmap.Pixels[i]);
                this.SetPixelBgra8((int)(i % bitmap.Width), (int)(i / bitmap.Width), pixelColor);
            }
        }

        #endregion

        #region Private Helpers

        private byte changeLeastSignificantBit(byte input, bool isWhite)
        {
            var changeLastBitTo0 = (byte) (input & 0xFE);
            var changeLastBitTo1 = (byte) (input | 1);

            return isWhite ? changeLastBitTo1 : changeLastBitTo0;
        }

        #endregion
    }
}
