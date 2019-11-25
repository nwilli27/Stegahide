using System;
using System.Collections.Generic;
using Windows.UI;
using GroupDStegafy.Model.Extensions;

namespace GroupDStegafy.Model.Text
{
    /// <summary>
    ///     Class responsible for holding methods to help encoding text in a bitmap
    /// </summary>
    internal class TextEncoder
    {

        #region Methods 

        /// <summary>
        ///     Embeds the character bits in the color and returns the modified copy.
        ///     Precondition: pixelColor != null
        ///                   binaryMessageBits != null
        ///     Post-condition: none
        /// </summary>
        /// <param name="pixelColor">Color of the pixel.</param>
        /// <param name="binaryMessageBitQueue">The binary message bits.</param>
        /// <returns>The new color</returns>
        public static Color EmbedCharacterBitsToColor(Color pixelColor, Queue<string> binaryMessageBitQueue)
        {
            if (pixelColor == null)
            {
                throw new ArgumentNullException(nameof(pixelColor));
            }

            pixelColor.R = addCharacterBitsToChannel(binaryMessageBitQueue, pixelColor.R);
            pixelColor.G = addCharacterBitsToChannel(binaryMessageBitQueue, pixelColor.G);
            pixelColor.B = addCharacterBitsToChannel(binaryMessageBitQueue, pixelColor.B);

            return pixelColor;
        }

        #endregion

        #region Private Helpers

        private static byte addCharacterBitsToChannel(Queue<string> binaryMessageBitQueue, byte colorChannel)
        {
            if (binaryMessageBitQueue.Count == 0)
            {
                return colorChannel;
            }

            var byteToMask = binaryMessageBitQueue.Dequeue();
            return colorChannel.MaskByteWithBitSequence(byteToMask);
        }

        #endregion
    }
}
