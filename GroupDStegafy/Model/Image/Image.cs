
using System;
using Windows.UI;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Holds generic implementation for an image.
    /// </summary>
    public abstract class Image
    {

        #region Properties

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public uint Width { get; protected set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public uint Height { get; protected set; }

        #endregion

        /// <summary>
        ///     Checks that the given coordinates are within the image bounds.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// x - X must be within image bounds
        /// or
        /// y - Y must be within image bounds
        /// </exception>
        protected void CheckBounds(int x, int y)
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

        /// <summary>
        ///     Gets the color of the pixel at the specified coordinates.
        ///     Precondition: X and Y are within image bounds
        ///     Postcondition: None
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The color of the specified pixel.</returns>
        public abstract Color GetPixelColor(int x, int y);
    }
}
