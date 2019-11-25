using Windows.UI;
using GroupDStegafy.Model.Extensions;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Class responsible for the implementation for the header pixels.
    /// </summary>
    public class HeaderPixels
    {
        #region Constants

        private readonly Color encryptionColorCode = Color.FromArgb(255,212,212,212);

        #endregion

        #region Data Members

        private Color firstPixelColor;
        private Color secondPixelColor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the color of the first pixel.
        /// </summary>
        /// <value>
        /// The color of the first pixel.
        /// </value>
        public Color FirstPixelColor => this.firstPixelColor;

        /// <summary>
        /// Gets the color of the second pixel.
        /// </summary>
        /// <value>
        /// The color of the second pixel.
        /// </value>
        public Color SecondPixelColor => this.secondPixelColor;

        /// <summary>
        /// Gets a value indicating whether the image has a secret message.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a secret message; otherwise, <c>false</c>.
        /// </value>
        public bool HasSecretMessage
        {
            get => this.firstPixelColor.Equals(this.encryptionColorCode);
            set => this.firstPixelColor = value ? this.encryptionColorCode : Colors.Black;
        }

        /// <summary>
        /// Gets a value indicating whether the image has encryption.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the image has encryption; otherwise, <c>false</c>.
        /// </value>
        public bool HasEncryption
        {
            get => this.secondPixelColor.R.IsLeastSignificantBitOne() && this.HasSecretMessage;
            set => this.secondPixelColor.R = this.secondPixelColor.R.SetLeastSignificantBit(value);
        }

        /// <summary>
        /// Gets the bits per color channel.
        /// </summary>
        /// <value>
        /// The bits per color channel.
        /// </value>
        public int BitsPerColorChannel
        {
            get => this.secondPixelColor.G.GetNumberOfOneBits();
            set => this.secondPixelColor.G = this.secondPixelColor.G.SetNumberOfOneBits(value);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is secret text
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is secret text; otherwise, <c>is secret image</c>.
        /// </value>
        public bool IsSecretText
        {
            get => this.secondPixelColor.B.IsLeastSignificantBitOne();
            set => this.secondPixelColor.B = this.secondPixelColor.B.SetLeastSignificantBit(value);
        } 

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HeaderPixels"/> class.
        ///     Precondition: source != null
        ///     Post-condition: this.firstPixelColor = pixelOne
        ///                     this.SecondPixelColor = pixelTwo
        /// </summary>
        /// <param name="pixelOne">The pixel one.</param>
        /// <param name="pixelTwo">The pixel two.</param>
        public HeaderPixels(Color pixelOne, Color pixelTwo)
        {
            this.firstPixelColor = pixelOne;
            this.secondPixelColor = pixelTwo;
        }

        #endregion
    }
}
