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

        /// <summary>
        /// Gets the pixels.
        /// </summary>
        /// <value>
        /// The pixels.
        /// </value>
        public bool[] Pixels { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonochromeBitmap"/> class.
        ///     Precondition: source != null
        ///     Post-condition: none
        /// </summary>
        /// <param name="source">The source.</param>
        public MonochromeBitmap(Bitmap source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            this.Width = source.Width;
            this.Pixels = new bool[source.Width * source.Height];

            this.changePixelsToMonochrome(source);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Converts the monochrome image to the standard Bitmap image.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <returns>The original bitmap image.</returns>
        public Bitmap ToBitmap()
        {
            var bytes = new byte[this.Pixels.Length * 4];
            var bitmap = new Bitmap(bytes, this.Width, 1, 1);
            for (var i = 0; i < this.Pixels.Length; i++)
            {
                var negatedColor = this.Pixels[i] ? Colors.White : Colors.Black;
                bitmap.SetPixelColor((int) (i % this.Width), (int)(i / this.Width), negatedColor);
            }

            return bitmap;
        }

        #endregion

        #region Private Helpers

        private void changePixelsToMonochrome(Bitmap source)
        {
            for (var i = 0; i < this.Pixels.Length; i++)
            {
                var pixelBlueByteValue = (source.GetPixelColor((int)(i % this.Width), (int)(i / this.Width)).B & 1);
                this.Pixels[i] = pixelBlueByteValue == 1;
            }
        }

        #endregion
    }
}
