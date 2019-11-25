using System;
using Windows.UI;
using GroupDStegafy.Model.Extensions;

namespace GroupDStegafy.Model.Text
{
    /// <summary>
    ///     Class responsible for decoding text from a Bitmap image.
    /// </summary>
    internal class TextDecodeUtility
    {

        #region Constants

        private const int BaseForm = 2;
        private const int TotalByteBits = 8;
        private const char ZeroBit = '0';

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

            var binaryMessage = "";

            binaryMessage += getMessageBitsFromChannel(pixelColor.R, bitsPerColorChannel);
            binaryMessage += getMessageBitsFromChannel(pixelColor.G, bitsPerColorChannel);
            binaryMessage += getMessageBitsFromChannel(pixelColor.B, bitsPerColorChannel);

            return binaryMessage;
        }

        /// <summary>
        ///     Removes the decode indicator from the string.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A string with the decode indicator removed.</returns>
        public static string RemoveDecodeIndicator(string message)
        {
            return message.Replace(DecodingStopIndicator, "");
        }

        /// <summary>
        ///     Determines whether [is finished decoding] [the specified binary message].
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="binaryMessage">The binary message.</param>
        /// <returns>
        ///   <c>true</c> if [is finished decoding] [the specified binary message]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFinishedDecoding(string binaryMessage)
        {
            var message = binaryMessage.ConvertBinaryToString().Trim();
            message = message.Substring(Math.Max(0, message.Length - DecodingStopIndicator.Length));
            return message.Contains(DecodingStopIndicator);
        }

        #endregion

        #region Private Helpers

        private static string getMessageBitsFromChannel(byte colorChannel, int bitsPerColorChannel)
        {
            var byteConversion = Convert.ToString(colorChannel, BaseForm);
            var binaryConversion = byteConversion.PadLeft(TotalByteBits, ZeroBit);

            return binaryConversion.Substring(TotalByteBits - bitsPerColorChannel, bitsPerColorChannel);
        }

        #endregion
    }
}
