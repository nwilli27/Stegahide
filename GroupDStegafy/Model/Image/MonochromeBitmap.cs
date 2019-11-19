using System;
using Windows.UI;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Responsible for the implementation of a Monochrome bitmap image.
    /// </summary>
    public class MonochromeBitmap : Image
    {
        #region Properties 

        private readonly bool[] pixels;

        #endregion

        #region Constructors

        private MonochromeBitmap(bool[] pixels, uint width, uint height)
        {
            this.pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates a monochrome image from a bitmap, where all non-black pixels are white, and returns it.
        ///     Precondition: source is not null
        ///     Postcondition: None
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A monochrome image.</returns>
        /// <exception cref="ArgumentNullException">source</exception>
        public static MonochromeBitmap FromBitmap(Bitmap source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var pixels = new bool[source.Width * source.Height];
            for (var x = 0; x < source.Width; x++)
            {
                for (var y = 0; y < source.Height; y++)
                {
                    var isWhite = !source.GetPixelColor(x, y).Equals(Colors.Black);
                    pixels[y*source.Width + x] = isWhite;
                }
            }
            return new MonochromeBitmap(pixels, source.Width, source.Height);
        }

        public static MonochromeBitmap FromEmbeddedSecret(Bitmap source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var pixels = new bool[source.Width * source.Height];
            for (var x = 0; x < source.Width; x++)
            {
                for (var y = 0; y < source.Height; y++)
                {
                    var isWhite = (source.GetPixelColor(x, y).B & 1) == 1;
                    pixels[y * source.Width + x] = isWhite;
                }
            }
            return new MonochromeBitmap(pixels, source.Width, source.Height);
        }

        /// <summary>
        ///     Converts the monochrome image to the standard Bitmap image.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <returns>The original bitmap image.</returns>
        public Bitmap ToBitmap()
        {
            var bytes = new byte[this.pixels.Length * 4];
            var bitmap = new Bitmap(bytes, this.Width, 1, 1);
            for (var i = 0; i < this.pixels.Length; i++)
            {
                var negatedColor = this.pixels[i] ? Colors.White : Colors.Black;
                bitmap.SetPixelColor((int) (i % this.Width), (int)(i / this.Width), negatedColor);
            }

            return bitmap;
        }

        #endregion

        /// <summary>
        ///     Gets the color of the pixel at the specified coordinates.
        ///     Precondition: X and Y are within image bounds
        ///     Postcondition: None
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The color of the specified pixel.
        /// </returns>
        public override Color GetPixelColor(int x, int y)
        {
            this.CheckBounds(x, y);
            var pixel = this.pixels[y*this.Width + x];
            return pixel ? Colors.White : Colors.Black;
        }
    }
}
