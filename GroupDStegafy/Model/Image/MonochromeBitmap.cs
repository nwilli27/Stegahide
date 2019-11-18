using System;
using Windows.UI;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Class representing a monochrome image.
    /// </summary>
    public class MonochromeBitmap
    {
        /// <summary>
        ///     Gets the pixels.
        /// </summary>
        /// <value>
        ///     The pixels.
        /// </value>
        public bool[] Pixels { get; }
        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public uint Width { get; }
        private uint Height => (uint)(this.Pixels.Length / this.Width);

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonochromeBitmap"/> class.
        ///     Precondition: source is not null
        ///     Postcondition: image is constructed
        /// </summary>
        /// <param name="source">The source.</param>
        public MonochromeBitmap(Bitmap source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            this.Width = source.Width;
            this.Pixels = new bool[source.Width * source.Height];
            for (var i = 0; i < this.Pixels.Length; i++)
            {
                var lsb = (source.GetPixelColor((int) (i % this.Width), (int) (i / this.Width)).B & 1) == 1;
                this.Pixels[i] = lsb;
            }
        }

        /// <summary>
        ///     Converts this monochrome image to a normal bitmap.
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            var bytes = new byte[this.Pixels.Length * 4];
            var bitmap = new Bitmap(bytes, this.Width, 1, 1);
            for (var i = 0; i < this.Pixels.Length; i++)
            {
                bitmap.SetPixelColor((int) (i % this.Width), (int)(i / this.Width), this.Pixels[i] ? Colors.White : Colors.Black);
            }

            return bitmap;
        }
    }
}
