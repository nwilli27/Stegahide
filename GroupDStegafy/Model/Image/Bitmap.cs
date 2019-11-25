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
        public bool HasSecretMessage => this.HeaderPixels.HasSecretMessage;

        /// <summary>
        /// Gets a value indicating whether this instance is secret text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is secret text; otherwise, <c>false</c>.
        /// </value>
        public bool IsSecretText => this.HeaderPixels.IsSecretText;

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
        ///     Precondition: none
        ///     Post-condition: @each significant bit in the source image is replaced.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="encrypt">Whether or not to encrypt the image.</param>
        public void EmbedMonochromeImage(MonochromeBitmap bitmap, bool encrypt)
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
                    pixelColor.B = pixelColor.B.SetLeastSignificantBit(bitmap.GetPixelColor(x, y).Equals(Colors.White));
                    this.SetPixelColor(x, y, pixelColor);
                }
            }

            if (encrypt)
            {
                this.EmbedMonochromeImage(MonochromeBitmap.FromEmbeddedSecret(this).GetFlipped(), false);
            }

            this.setUpHeaderForSecretImage(encrypt);
        }

        /// <summary>
        ///     Embeds the text message in the bitmap pixels color bytes.
        ///     Precondition: message != null
        ///     Post-condition: message embedded in bitmap pixel color bytes
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <exception cref="ArgumentNullException">message</exception>
        public void EmbedTextMessage(string message, string encryptionKey)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var binaryMessage = this.setupTextMessage(message, encryptionKey);
            var binaryMessageBitQueue = binaryMessage.SplitInParts(this.HeaderPixels.BitsPerColorChannel);

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

            this.setUpHeaderForSecretTextMessage();
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
                    if (TextDecodeUtility.IsFinishedDecoding(binaryMessage))
                    {
                        break;
                    }

                    binaryMessage = this.extractMessageBitsFromPixel(x, y, binaryMessage);
                }
            }

            return this.HeaderPixels.HasEncryption ? TextCipher.DecryptText(binaryMessage.ConvertBinaryToString()) : 
                                                     TextDecodeUtility.RemoveDecodeIndicator(binaryMessage.ConvertBinaryToString());
        }

        #endregion

        #region Private Helpers

        private void embedMessageBitsInPixel(int x, int y, Queue<string> binaryMessageBitQueue)
        {
            var pixelColor = this.GetPixelColor(x, y);
            if (!areHeaderPixels(x, y))
            {
                TextEncodeUtility.EmbedCharacterBitsToColor(pixelColor, binaryMessageBitQueue);
            }

            this.SetPixelColor(x, y, pixelColor);
        }

        private string extractMessageBitsFromPixel(int x, int y, string binaryMessage)
        {
            var pixelColor = this.GetPixelColor(x, y);
            if (!areHeaderPixels(x, y))
            {
                binaryMessage += TextDecodeUtility.ExtractMessageBits(pixelColor, this.HeaderPixels.BitsPerColorChannel);
            }

            return binaryMessage;
        }

        private string setupTextMessage(string message, string encryptionKey)
        {
            message = this.checkToEncryptText(message, encryptionKey);
            message += TextDecodeUtility.DecodingStopIndicator + " ";
            return message.ConvertToBinary();
        }

        private string checkToEncryptText(string message, string encryptionKey)
        {
            if (!string.IsNullOrEmpty(encryptionKey))
            {
                this.HeaderPixels.HasEncryption = true;
                message = TextCipher.EncryptTextWithKey(message, encryptionKey);
            }

            this.HeaderPixels.HasEncryption = false;
            return message;
        }

        private void setUpHeaderForSecretTextMessage()
        {
            this.HeaderPixels.HasSecretMessage = true;
            this.HeaderPixels.IsSecretText = true;
            this.setHeaderPixels();
        }

        private void setUpHeaderForSecretImage(bool hasEncryption)
        {
            this.HeaderPixels.HasSecretMessage = true;
            this.HeaderPixels.BitsPerColorChannel = 1;
            this.HeaderPixels.HasEncryption = hasEncryption;
            this.HeaderPixels.IsSecretText = false;

            this.setHeaderPixels();
        }

        private void setHeaderPixels()
        {
            this.SetPixelColor(0, 0, this.HeaderPixels.FirstPixelColor);
            this.SetPixelColor(0, 1, this.HeaderPixels.SecondPixelColor);
        }

        private void createHeaderPixels()
        {
            var pixelOne = this.GetPixelColor(0, 0);
            var pixelTwo = this.GetPixelColor(0, 1);

            this.HeaderPixels = new HeaderPixels(pixelOne, pixelTwo);
        }

        private static bool areHeaderPixels(int x, int y)
        {
            return (x == 0 && y == 0) || (x == 0 && y == 1);
        }

        #endregion
    }
}
