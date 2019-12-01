using System;

namespace GroupDStegafy.Model.Extensions
{
    /// <summary>
    ///     Class offers additional functionality to the type byte
    /// </summary>
    internal static class ByteExtensions
    {
        #region Constants

        private const char ZeroBit = '0';

        public const int BaseFormTwo = 2;
        public const int ByteLength = 8;

        #endregion

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

            return (byte) (byteInput & 0xfe);
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
                newByteValue += Math.Pow(BaseFormTwo, i);
            }

            return Convert.ToByte(newByteValue);
        }

        /// <summary>
        ///     Masks the byte with bit sequence that is applied on the end of the 8 bits.
        ///     Precondition: bitSequence != null
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <param name="bitSequence">The bit sequence.</param>
        /// <returns>A new byte that is masked by the bit sequence appended on the end.</returns>
        public static byte MaskByteWithBitSequence(this byte byteInput, string bitSequence)
        {
            var inputBaseTwoForm = byteInput.ConvertToBaseFormTwo();
            var sequenceAdded = inputBaseTwoForm.ReplaceAt(inputBaseTwoForm.Length - bitSequence.Length, bitSequence);

            return Convert.ToByte(sequenceAdded, BaseFormTwo);
        }

        /// <summary>
        ///     Converts the byte to base 2 form with 8 bits.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="byteInput">The byte input.</param>
        /// <returns>A string (binary) in the base 2 form.</returns>
        public static string ConvertToBaseFormTwo(this byte byteInput)
        {
            return Convert.ToString(byteInput, BaseFormTwo).PadLeft(ByteLength, ZeroBit);
        }

        #endregion

    }
}
