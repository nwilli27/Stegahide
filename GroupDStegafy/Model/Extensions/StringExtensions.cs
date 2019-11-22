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
        public static string ConvertBinaryToString(this string binaryString)
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
        ///     Converts a string into a string of items indicating 8 bits for each character in the string.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="stringInput">The string input.</param>
        /// <returns>A string of all characters turned into a byte of 8 bits.</returns>
        public static string ConvertToBinary(this string stringInput)
        {
            var binaryOutput = "";

            foreach (var currentChar in stringInput.Select(c => Convert.ToString(c, 2).PadLeft(8, '0')))
            {
                binaryOutput += currentChar;
            }

            return binaryOutput;
        }

        /// <summary>
        ///     Splits the string in parts based on the [partLength]. Returns the split string
        ///     as a stack.
        ///     Precondition: partLength > 0
        ///     Post-condition: none
        /// </summary>
        /// <param name="stringInput">The string input.</param>
        /// <param name="partLength">Length of the part.</param>
        /// <returns>A stack of the split string</returns>
        /// <exception cref="ArgumentException">Part length has to be positive. - partLength</exception>
        public static Stack<string> SplitInParts(this string stringInput, int partLength)
        {
            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            var splitStack = new Stack<string>();
            
            for (var i = 0; i < stringInput.Length; i += partLength)
            {
                splitStack.Push(stringInput.Substring(i, Math.Min(partLength, stringInput.Length - i)));
            }

            return splitStack;
        }
    }
}
