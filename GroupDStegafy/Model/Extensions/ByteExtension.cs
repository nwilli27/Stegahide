using System;

namespace GroupDStegafy.Model.Extensions
{
    /// <summary>
    ///     Class offers additional functionality to the type byte
    /// </summary>
    internal static class ByteExtension
    {

        #region Constants

        private const uint LeastSignificantBit = 0x0000FFFF;

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the least significant bit.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <returns></returns>
        public static uint GetLeastSignificantBit(this byte byteInput)
        {
            return (byteInput & LeastSignificantBit);
        }

        /// <summary>
        ///     Shifts the bit right one.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <returns></returns>
        public static int ShiftByteRightOne(this byte byteInput)
        {
            return (byteInput >> 1);
        }

        /// <summary>
        ///     Shifts the bit left one.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <returns></returns>
        public static int ShiftByteLeftOne(this byte byteInput)
        {
            return (byteInput << 1);
        }

        /// <summary>
        ///     Changes the last bit.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <param name="lastBitValue">if set to <c>true</c> [last bit value].</param>
        public static byte ChangeLastBit(this byte byteInput, bool lastBitValue)
        {
            return byteInput |= 1;
        }

        /// <summary>
        ///     Gets the number of set bits.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <returns></returns>
        public static int GetNumberOfSetBits(this byte byteInput)
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
        public static void SetNumberOfOneBits(this byte byteInput, int numberOfBits)
        {
            var newByteValue = 0.0;

            for (var i = 0; i < numberOfBits; i++)
            {
                newByteValue += Math.Pow(2, i);
            }

            byteInput = Convert.ToByte(newByteValue);
        }

        #endregion

    }
}
