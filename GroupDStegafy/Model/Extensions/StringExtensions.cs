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

        #region Constants

        private const int ByteLength = 8;
        private const int BaseForm = 2;
        
        #endregion

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
            var numberOfBytes = binaryString.Length / ByteLength;
            var charList = new byte[numberOfBytes];

            for (var i = 0; i < numberOfBytes; i++)
            {
                var asciiCharacter = binaryString.Substring(i * ByteLength, ByteLength);
                Console.WriteLine(asciiCharacter);
                charList[i] = Convert.ToByte(asciiCharacter, BaseForm);
            }

            return Encoding.ASCII.GetString(charList);
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

            foreach (var currentChar in stringInput.Select(c => Convert.ToByte(c).ConvertToBaseFormTwo()))
            {
                binaryOutput += currentChar;
            }

            return binaryOutput;
        }

        /// <summary>
        ///     Splits the string in parts based on the [partLength]. Returns the split string
        ///     as a list.
        ///     Precondition: partLength > 0
        ///     Post-condition: none
        /// </summary>
        /// <param name="stringInput">The string input.</param>
        /// <param name="partLength">Length of the part.</param>
        /// <returns>A list of the split string</returns>
        /// <exception cref="ArgumentException">Part length has to be positive. - partLength</exception>
        public static IList<string> SplitInParts(this string stringInput, int partLength)
        {
            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            var splitList = new List<string>();
            
            for (var i = 0; i < stringInput.Length; i += partLength)
            {
                var part = stringInput.Substring(i, Math.Min(partLength, stringInput.Length - i));
                splitList.Add(part);
            }

            return splitList;
        }

        /// <summary>
        ///     Replaces a certain index in a string with the string [replacement]
        /// </summary>
        /// <param name="stringInput">The string.</param>
        /// <param name="index">The index.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>A string with the new replacement.</returns>
        public static string ReplaceAt(this string stringInput, int index, string replacement)
        {
            return stringInput.Remove(index, Math.Min(replacement.Length, stringInput.Length - index))
                      .Insert(index, replacement);
        }
    }
}
