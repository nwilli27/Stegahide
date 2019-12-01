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

        #region Constants

        private const int PixelByteSize = 4;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonochromeBitmap"/> class.
        ///     Precondition: pixels != null
        ///     Post-condition: this.Width = width
        ///                     this.Height = height
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException">pixels</exception>
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
        ///     Post-condition: None
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

        /// <summary>
        /// Extracts the embedded monochrome image from the source image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The embedded monochrome image from the source image</returns>
        /// <exception cref="ArgumentNullException">source</exception>
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
            var bytes = new byte[this.pixels.Length * PixelByteSize];
            var bitmap = new Bitmap(bytes, this.Width, 1, 1);
            for (var i = 0; i < this.pixels.Length; i++)
            {
                var negatedColor = this.pixels[i] ? Colors.White : Colors.Black;
                bitmap.SetPixelColor((int) (i % this.Width), (int)(i / this.Width), negatedColor);
            }

            return bitmap;
        }

        /// <summary>
        ///     Gets the color of the pixel at the specified coordinates.
        ///     Precondition: X and Y are within image bounds
        ///     Post-condition: None
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

        /// <summary>
        ///     Returns a copy of the image with the top and bottom halves of the image flipped.
        ///     Precondition: None
        ///     Post-condition: None
        /// </summary>
        /// <returns>The flipped version of the bitmap.</returns>
        public MonochromeBitmap GetFlipped()
        {
            var halfHeight = this.Height / 2;
            var pixels = new bool[this.pixels.Length];

            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < halfHeight; y++)
                {
                    pixels[y * this.Width + x] = this.pixels[(y + halfHeight) * this.Width + x];
                    pixels[(y + halfHeight) * this.Width + x] = this.pixels[y * this.Width + x];
                }
            }

            return new MonochromeBitmap(pixels, this.Width, this.Height);
        }

        #endregion
    }
}
