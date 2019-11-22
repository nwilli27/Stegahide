using System;

namespace GroupDStegafy.Model.Extensions
{
    /// <summary>
    ///     Class offers additional functionality to the type byte
    /// </summary>
    internal static class ByteExtensions
    {

        #region Methods

        /// <summary>
        ///     Determines whether [is last bit set] [the specified byte input].
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <returns>
        ///   <c>true</c> if [is last bit set] [the specified byte input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLeastSignificantBitOne(this byte byteInput)
        {
            return (0 != (byteInput & 0x01));
        }

        /// <summary>
        /// Gets the number of bits of a byte value.
        /// Ex: 30 = 5 bits
        ///    255 = 8 bits
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <returns>
        ///     The number of bits in a byte value.
        /// </returns>
        public static int Size(this byte bits)
        {
            return (int)(Math.Log(bits, 2)) + 1;
        }

        /// <summary>
        ///     Returns the last bit.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <param name="lastBitValue">if set to <c>true</c> [last bit value].</param>
        public static byte SetLeastSignificantBit(this byte byteInput, bool lastBitValue)
        {
            if (lastBitValue)
            {
                return (byte) (byteInput | 1);
            }
            else
            {
                return (byte) (byteInput & 0xfe);
            }
        }

        /// <summary>
        ///     Gets the number of set bits.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <returns>
        ///     The number of set bits.
        /// </returns>
        public static int GetNumberOfOneBits(this byte byteInput)
        {
            var count = 0;

            while (byteInput > 0)
            {
                count += byteInput & 1;
                byteInput >>= 1;
            }

            return count;
        }

        /// <summary>
        ///     Sets the number of one bits.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <param name="numberOfBits">The number of bits.</param>
        /// <returns>
        ///     Sets The number of set bits and returns a byte.
        /// </returns>
        public static byte SetNumberOfOneBits(this byte byteInput, int numberOfBits)
        {
            var newByteValue = 0.0;

            for (var i = 0; i < numberOfBits; i++)
            {
                newByteValue += Math.Pow(2, i);
            }

            return Convert.ToByte(newByteValue);
        }

        #endregion

    }
}
