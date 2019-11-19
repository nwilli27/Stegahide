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
        /// <exception cref="ArgumentNullException">pixelBytes</exception>
        public Bitmap(byte[] pixelBytes, uint width, uint dpix, uint dpiy)
        {
            this.pixelBytes = pixelBytes ?? throw new ArgumentNullException(nameof(pixelBytes));

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
        ///     Gets the color of the pixel at the specified coordinates.
        ///     Precondition: X and Y are within image bounds
        ///     Postcondition: None
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The color of the specified pixel.</returns>
        public override Color GetPixelColor(int x, int y)
        {
            this.CheckBounds(x, y);

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
        public void SetPixelColor(int x, int y, Color color)
        {
            this.CheckBounds(x, y);

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
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var pixelColor = this.GetPixelColor(x, y);
                    //TODO could maybe make this an extension for a Color object.
                    pixelColor.B = changeLeastSignificantBit(pixelColor.B, bitmap.GetPixelColor(x, y).Equals(Colors.White));
                    this.SetPixelColor(x, y, pixelColor);
                }
            }
        }

        #endregion

        #region Private Helpers

        private static byte changeLeastSignificantBit(byte input, bool isWhite)
        {
            var changeLastBitTo0 = (byte) (input & 0xFE);
            var changeLastBitTo1 = (byte) (input | 1);

            return isWhite ? changeLastBitTo1 : changeLastBitTo0;
        }

        #endregion
    }
}
