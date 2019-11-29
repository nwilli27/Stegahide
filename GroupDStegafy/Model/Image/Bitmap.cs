using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using GroupDStegafy.Model.Extensions;
using GroupDStegafy.Model.Text;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Implementation of a bitmap image that allows for low-level bit manipulation.
    /// </summary>
    public class Bitmap : Image
    {
        #region Data Members

        private readonly byte[] pixelBytes;
        private HeaderPixels headerPixels;

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

        /// <summary>
        /// Gets a value indicating whether this instance has secret message.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has secret message; otherwise, <c>false</c>.
        /// </value>
        public bool HasSecretMessage => this.headerPixels.HasSecretMessage;

        /// <summary>
        /// Gets a value indicating whether this instance is secret text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is secret text; otherwise, <c>false</c>.
        /// </value>
        public bool IsSecretText => this.headerPixels.IsSecretText;

        /// <summary>
        /// Gets a value indicating whether this instance has encryption.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has encryption; otherwise, <c>false</c>.
        /// </value>
        public bool HasEncryption => this.headerPixels.HasEncryption;

        /// <summary>
        /// Gets the bits per color channel.
        /// </summary>
        /// <value>
        /// The bits per color channel.
        /// </value>
        public int BitsPerColorChannel => this.headerPixels.BitsPerColorChannel;

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

            this.createHeaderPixels();
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
        ///     Post-condition: None
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
        ///     Precondition: bitmap is not null
        ///     Post-condition: least significant bit in the source image is replaced.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="encrypt">Whether or not to encrypt the image.</param>
        public void EmbedMonochromeImage(MonochromeBitmap bitmap, bool encrypt)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            for (var x = 0; x < bitmap.Width && x < this.Height; x++)
            {
                for (var y = 0; y < bitmap.Height && y < this.Height; y++)
                {
                    var pixelColor = this.GetPixelColor(x, y);
                    pixelColor.B = pixelColor.B.SetLeastSignificantBit(bitmap.GetPixelColor(x, y).Equals(Colors.White));
                    this.SetPixelColor(x, y, pixelColor);
                }
            }

            if (encrypt)
            {
                this.flipEmbeddedImage();
            }
            this.setUpHeaderForSecretImage(encrypt);
        }

        /// <summary>
        ///     Embeds the text message in the bitmap pixels color bytes.
        ///     Precondition: message != null, message will fit in the image,
        ///                   bits per color channel is between 1 and 8 inclusive
        ///     Post-condition: message embedded in bitmap
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel</param>
        /// <exception cref="ArgumentNullException">message</exception>
        /// <exception cref="ArgumentOutOfRangeException">bitsPerColorChannel</exception>
        /// <exception cref="MessageTooLargeException">Message cannot fit given the bits per color channel</exception>
        public void EmbedTextMessage(string message, string encryptionKey, int bitsPerColorChannel)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (bitsPerColorChannel < 1 || bitsPerColorChannel > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsPerColorChannel));
            }
            if (this.isMessageTooBig(message, bitsPerColorChannel))
            {
                throw new MessageTooLargeException();
            }

            var binaryMessage = setupTextMessage(message, encryptionKey);
            var binaryMessageBitQueue = binaryMessage.SplitInParts(bitsPerColorChannel);

            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    if (binaryMessageBitQueue.Count == 0)
                    {
                        break;
                    }

                    this.embedMessageBitsInPixel(x, y, binaryMessageBitQueue);
                }
            }

            this.setUpHeaderForSecretTextMessage(!string.IsNullOrEmpty(encryptionKey), bitsPerColorChannel);
        }

        /// <summary>
        ///     Decodes the text message from the bitmap.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <returns>The embedded text message from the color channel bytes.</returns>
        public string DecodeTextMessage()
        {
            var binaryMessage = string.Empty;

            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    if (TextDecoder.EndsWithStopIndicator(binaryMessage))
                    {
                        break;
                    } 

                    binaryMessage = this.extractMessageBitsFromPixel(x, y, binaryMessage);
                }
            }

            return this.headerPixels.HasEncryption ? TextCipher.DecryptText(binaryMessage.ConvertBinaryToString()) : 
                                                     TextDecoder.RemoveDecodeIndicator(binaryMessage.ConvertBinaryToString());
        }

        #endregion

        #region Private Helpers - Checks

        private bool isMessageTooBig(string message, int bitsPerColorChannel)
        {
            const int possibleColorChannels = 3;
            const int charByteLength = 8;
            var totalPossibleChannels = (this.Width * this.Height) * (bitsPerColorChannel * possibleColorChannels);
            var numberOfMessageBits = message.Length * charByteLength / bitsPerColorChannel;

            return numberOfMessageBits > totalPossibleChannels;
        }

        private static bool isHeaderPixel(int x, int y)
        {
            return (x == 0 && y == 0) || (x == 0 && y == 1);
        }

        #endregion

        #region Private Helpers - Action

        private void flipEmbeddedImage()
        {
            this.EmbedMonochromeImage(MonochromeBitmap.FromEmbeddedSecret(this).GetFlipped(), false);
        }

        private void embedMessageBitsInPixel(int x, int y, Queue<string> binaryMessageBitQueue)
        {
            var pixelColor = this.GetPixelColor(x, y);

            if (!isHeaderPixel(x, y))
            {
                pixelColor = TextEncoder.EmbedCharacterBitsToColor(pixelColor, binaryMessageBitQueue);
            }

            this.SetPixelColor(x, y, pixelColor);
        }

        private string extractMessageBitsFromPixel(int x, int y, string binaryMessage)
        {
            var pixelColor = this.GetPixelColor(x, y);

            if (!isHeaderPixel(x, y))
            {
                binaryMessage += TextDecoder.ExtractMessageBits(pixelColor, this.headerPixels.BitsPerColorChannel);
            }

            return binaryMessage;
        }

        #endregion

        #region Private Helpers - Setup

        private static string setupTextMessage(string message, string encryptionKey)
        {
            message = TextCipher.EncryptTextWithKey(message, encryptionKey);
            message += TextDecoder.DecodingStopIndicator + " ";
            return message.ConvertToBinary();
        }

        private void setUpHeaderForSecretTextMessage(bool hasEncryption, int bitsPerColorChannel)
        {
            this.headerPixels.HasSecretMessage = true;
            this.headerPixels.IsSecretText = true;
            this.headerPixels.HasEncryption = hasEncryption;
            this.headerPixels.BitsPerColorChannel = bitsPerColorChannel;
            this.updateFromHeaderPixels();
        }

        private void setUpHeaderForSecretImage(bool hasEncryption)
        {
            this.headerPixels.HasSecretMessage = true;
            this.headerPixels.HasEncryption = hasEncryption;
            this.headerPixels.IsSecretText = false;

            this.updateFromHeaderPixels();
        }

        private void updateFromHeaderPixels()
        {
            this.SetPixelColor(0, 0, this.headerPixels.FirstPixelColor);
            this.SetPixelColor(0, 1, this.headerPixels.SecondPixelColor);
        }

        private void createHeaderPixels()
        {
            var pixelOne = this.GetPixelColor(0, 0);
            var pixelTwo = this.GetPixelColor(0, 1);

            this.headerPixels = new HeaderPixels(pixelOne, pixelTwo);
        }

        #endregion

    }
}
