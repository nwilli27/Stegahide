using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GroupDStegafy.Model.Extensions
{
    /// <summary>
    ///     Class offers additional functionality to strings.
    /// </summary>
    public static class StringExtensions
    {

        #region Constants

        public const string AlphabetOnlyRegex = "[^a-zA-Z]";

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
            var numberOfBytes = binaryString.Length / ByteExtensions.ByteLength;
            var charList = new byte[numberOfBytes];

            for (var i = 0; i < numberOfBytes; i++)
            {
                var asciiCharacter = binaryString.Substring(i * ByteExtensions.ByteLength, ByteExtensions.ByteLength);
                Console.WriteLine(asciiCharacter);
                charList[i] = Convert.ToByte(asciiCharacter, ByteExtensions.BaseFormTwo);
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
        ///     as a queue.
        ///     Precondition: partLength > 0
        ///     Post-condition: none
        /// </summary>
        /// <param name="stringInput">The string input.</param>
        /// <param name="partLength">Length of the part.</param>
        /// <returns>A queue of the split string</returns>
        /// <exception cref="ArgumentException">Part length has to be positive. - partLength</exception>
        public static Queue<string> SplitInParts(this string stringInput, int partLength)
        {
            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            var splitList = new Queue<string>();
            
            for (var i = 0; i < stringInput.Length; i += partLength)
            {
                var part = stringInput.Substring(i, Math.Min(partLength, stringInput.Length - i));
                splitList.Enqueue(part);
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

        /// <summary>
        ///     Keeps the only alphabetical letters in string.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="stringInput">The string input.</param>
        /// <returns>A string of only alphabetical letters</returns>
        public static string ToAlphabeticalOnly(this string stringInput)
        {
            return Regex.Replace(stringInput, AlphabetOnlyRegex, string.Empty);
        }
    }
}
