using System;
using System.Collections.Generic;
using Windows.UI;
using GroupDStegafy.Model.Extensions;

namespace GroupDStegafy.Model.Text
{
    /// <summary>
    ///     Class responsible for holding methods to help encoding text in a bitmap
    /// </summary>
    internal class TextEncodeUtility
    {
        
        #region Methods 

        /// <summary>
        ///     Embeds the character bits in the color channels and returns the color object.
        ///     Precondition: pixelColor != null
        ///                   binaryBitsPerChannel != null
        /// </summary>
        /// <param name="pixelColor">Color of the pixel.</param>
        /// <param name="binaryBitsPerChannel">The binary bits per channel.</param>
        /// <returns></returns>
        public static Color EmbedCharacterBitsToColor(Color pixelColor, IList<string> binaryBitsPerChannel)
        {
            if (pixelColor == null)
            {
                throw new ArgumentNullException(nameof(pixelColor));
            }
            if (binaryBitsPerChannel == null)
            {
                throw new ArgumentNullException(nameof(binaryBitsPerChannel));
            }

            pixelColor.R = addCharacterBitsToChannel(binaryBitsPerChannel, pixelColor.R);
            pixelColor.G = addCharacterBitsToChannel(binaryBitsPerChannel, pixelColor.G);
            pixelColor.B = addCharacterBitsToChannel(binaryBitsPerChannel, pixelColor.B);

            return pixelColor;
        }

        #endregion

        #region Private Helpers

        private static byte addCharacterBitsToChannel(IList<string> binaryBits, byte colorChannel)
        {
            if (binaryBits.Count > 0)
            {
                var byteToMask = binaryBits[0];
                binaryBits.Remove(byteToMask);
                return colorChannel.MaskByteWithBitSequence(byteToMask);
            }

            return colorChannel;
        }

        #endregion
    }
}
