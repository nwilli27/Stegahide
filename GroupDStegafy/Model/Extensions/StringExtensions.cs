using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupDStegafy.Model.Extensions
{
    /// <summary>
    ///     Class offers additional functionality to strings.
    /// </summary>
    internal static class StringExtensions
    {

        /// <summary>
        ///     Gets the bytes from the string as binaryString and converts it to
        ///     a readable string composed of ASCII characters.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="binaryString">The binaryString.</param>
        /// <returns>A string composed of ASCII characters converted from a byte.</returns>
        public static string GetBytesFromBinaryString(this string binaryString)
        {
            var charList = new List<byte>();

            for (var i = 0; i < binaryString.Length; i += 8)
            {
                var binaryChar = binaryString.Substring(i, 8);

                charList.Add(Convert.ToByte(binaryChar, 2));
            }

            return Encoding.ASCII.GetString(charList.ToArray());
        }

        /// <summary>
        ///     Converts a string into a list of items indicating 8 bits for each character in the string.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="stringInput">The string input.</param>
        /// <returns>A list of all characters turned into a byte of 8 bits.</returns>
        public static List<string> ConvertToBinaryList(this string stringInput)
        {
            var binaryList = new List<string>();

            foreach (var currentChar in stringInput.Select(c => Convert.ToString(c, 2).PadLeft(8, '0')))
            {
                binaryList.Add(currentChar);
            }

            return binaryList;
        }
    }
}
