using System;
using Windows.UI;
using GroupDStegafy.Model.Extensions;

namespace GroupDStegafy.Model.Text
{
    /// <summary>
    ///     Class responsible for decoding text from a Bitmap image.
    /// </summary>
    internal class TextDecoder
    {

        #region Constants

        public const string DecodingStopIndicator = "#.-.-.-#";

        #endregion

        #region Methods

        /// <summary>
        ///     Extracts the message bits from the color channel and returns them as a binary string.
        ///     Precondition: pixelColor != null
        ///     Post-condition: none
        /// </summary>
        /// <param name="pixelColor">Color of the pixel.</param>
        /// <param name="bitsPerColorChannel">The bits per color channel.</param>
        /// <returns>A string of bits from color channels.</returns>
        public static string ExtractMessageBits(Color pixelColor, int bitsPerColorChannel)
        {
            if (pixelColor == null)
            {
                throw new ArgumentNullException(nameof(pixelColor));
            }

            var binaryMessage = string.Empty;

            binaryMessage += getMessageBitsFromChannel(pixelColor.R, bitsPerColorChannel);
            binaryMessage += getMessageBitsFromChannel(pixelColor.G, bitsPerColorChannel);
            binaryMessage += getMessageBitsFromChannel(pixelColor.B, bitsPerColorChannel);

            return binaryMessage;
        }

        /// <summary>
        ///     Removes the stop indicator from the string.
        ///     Precondition: message is not null
        ///     Post-condition: none
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A string with the decode indicator removed.</returns>
        public static string RemoveDecodeIndicator(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return message.Replace(DecodingStopIndicator, string.Empty);
        }

        /// <summary>
        ///     Determines whether the passed in binary string ends with the stop indicator.
        ///     Precondition: binary message is not null
        ///     Post-condition: none
        /// </summary>
        /// <param name="binaryMessage">The binary message.</param>
        /// <returns>
        ///   <c>true</c> if message ends with stop indicator; otherwise, <c>false</c>.
        /// </returns>
        public static bool EndsWithStopIndicator(string binaryMessage)
        {
            if (binaryMessage == null)
            {
                throw new ArgumentNullException(nameof(binaryMessage));
            }
            var message = binaryMessage.ConvertBinaryToString().Trim();
            return message.EndsWith(DecodingStopIndicator);
        }

        #endregion

        #region Private Helpers

        private static string getMessageBitsFromChannel(byte colorChannel, int bitsPerColorChannel)
        {
            var byteConversion = colorChannel.ConvertToBaseFormTwo();
            return byteConversion.Substring(ByteExtensions.ByteLength - bitsPerColorChannel, bitsPerColorChannel);
        }

        #endregion
    }
}
