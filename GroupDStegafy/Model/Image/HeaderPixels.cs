using System;
using Windows.UI;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Class responsible for the implementation for the header pixels.
    /// </summary>
    internal class HeaderPixels
    {

        #region Data Members

        private Color firstPixelColor;
        private Color secondPixelColor;
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the image has a secret message.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a secret message; otherwise, <c>false</c>.
        /// </value>
        public bool HasSecretMessage
        {
            get => this.firstPixelColor.R == EncryptionColorCode &&
                   this.firstPixelColor.G == EncryptionColorCode &&
                   this.firstPixelColor.B == EncryptionColorCode;

            set => this.setFirstPixelToSecretMessageColorCode(value);
        }

        /// <summary>
        /// Gets a value indicating whether the image has encryption.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the image has encryption; otherwise, <c>false</c>.
        /// </value>
        public bool HasEncryption
        {
            get => (this.secondPixelColor.R & LeastSignificantBit) == 1;
            set => this.setSecondPixelEncryptionStatus(value);
        }

        /// <summary>
        /// Gets the bits per color channel.
        /// </summary>
        /// <value>
        /// The bits per color channel.
        /// </value>
        public int BitsPerColorChannel
        {
            get => (this.secondPixelColor.G >> 1);
            //TODO not actually sure if this will work. As the lsb of green will be used to indicate the secret image.
            set => this.secondPixelColor.G = Convert.ToByte(value);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is secret image.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is secret image; otherwise, <c>false</c>.
        /// </value>
        public bool HasSecretImage
        {
            get => (this.secondPixelColor.G & LeastSignificantBit) == 0;
            set => this.setSecondPixelImageStatus(value);
        } 

        #endregion

        #region Constants

        private const int EncryptionColorCode = 212;
        //TODO make color extension to get least significant bit of color channels if possible.
        private const uint LeastSignificantBit = 0x0000FFFF;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HeaderPixels"/> class.
        ///     Precondition: none
        ///     Post-condition: this.firstPixelColor = source.FirstPixel
        ///                     this.secondPixelColor = source.SecondPixel
        /// </summary>
        /// <param name="source">The source.</param>
        public HeaderPixels(Image source)
        {
            this.firstPixelColor = source.GetPixelColor(0, 0);
            this.secondPixelColor = source.GetPixelColor(1, 0);
        }

        #endregion

        #region Private Helpers

        private void setFirstPixelToSecretMessageColorCode(bool value)
        {
            if (value)
            {
                this.firstPixelColor.R = EncryptionColorCode;
                this.firstPixelColor.G = EncryptionColorCode;
                this.firstPixelColor.B = EncryptionColorCode;
            }
        }

        private void setSecondPixelEncryptionStatus(bool value)
        {
            if (value)
            {
                this.secondPixelColor.R |= 1;
            }
            else
            {
                this.secondPixelColor.R &= 0xfe;
            }
        }

        private void setSecondPixelImageStatus(bool value)
        {
            if (value)
            {
                this.secondPixelColor.G |= 1;
            }
            else
            {
                this.secondPixelColor.G &= 0xfe;
            }
        }

        #endregion
    }
}
