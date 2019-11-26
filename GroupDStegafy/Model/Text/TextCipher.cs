using System;
using System.Text;
using System.Text.RegularExpressions;
using GroupDStegafy.Model.Extensions;

namespace GroupDStegafy.Model.Text
{
    /// <summary>
    ///     Class responsible for methods dealing with encrypting text
    /// </summary>
    internal class TextCipher
    {
        #region Properties

        public static string EncryptionKey { get; private set; }

        public static string EncryptedMessage { get; private set; }

        #endregion

        #region Constants

        private const string KeywordEnd = "#KEY#";
        private const char FirstCharacter = 'A';
        private const char LastCharacter = 'Z';

        #endregion

        #region Methods

        /// <summary>
        ///     Encrypts the text and adds the keyword to the front for
        ///     decryption purposes.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>The encrypted text with the keyword added to the front</returns>
        public static string EncryptTextWithKey(string plainText, string keyword)
        {
            var encryptedText = encryptText(plainText, keyword);
            return keyword + KeywordEnd + encryptedText;
        }

        /// <summary>
        ///     Decrypts the text by extracting the keyword and the message
        ///     to decrypt the underlying message.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns>The decrypted text message.</returns>
        public static string DecryptText(string encryptedText)
        {
            EncryptionKey = ExtractKeyword(encryptedText);
            EncryptedMessage = extractMessage(encryptedText);

            return decryptText(EncryptedMessage, EncryptionKey);
        }

        /// <summary>
        ///     Extracts the keyword from the encrypted text.
        ///     Precondition: none
        ///     Post-condition: nome
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <returns>The keyword from the encrypted text.</returns>
        public static string ExtractKeyword(string encryptedText)
        {
            var keywordEndIndex = encryptedText.IndexOf(KeywordEnd, StringComparison.Ordinal);
            return encryptedText.Substring(0, keywordEndIndex);
        }

        #endregion

        #region Private Helpers

        private static string extractMessage(string encryptedText)
        {
            var keywordEndIndex = encryptedText.IndexOf(KeywordEnd, StringComparison.Ordinal) + KeywordEnd.Length;
            return encryptedText.Substring(keywordEndIndex);
        }

        private static string encryptText(string plainText, string keyword)
        {
            var textInput = getStringBuilderForCipher(plainText);
            keyword = keyword.ToUpper();
            var keywordIndex = 0;

            for (var i = 0; i < textInput.Length; i++)
            {
                textInput[i] = (char)(textInput[i] + keyword[keywordIndex] - FirstCharacter);

                if (textInput[i] > LastCharacter)
                {
                    textInput[i] = (char)(textInput[i] - LastCharacter + FirstCharacter - 1);
                }
                
                keywordIndex = incrementKeywordIndex(keyword, keywordIndex);
            }

            return textInput.ToString();
        }

        private static string decryptText(string encryptedText, string keyword)
        {
            var textInput = getStringBuilderForCipher(encryptedText);
            keyword = keyword.ToUpper();
            var keywordIndex = 0;

            for (var i = 0; i < textInput.Length; i++)
            {
                textInput[i] = textInput[i] >= keyword[keywordIndex] ? 
                    (char)(textInput[i] - keyword[keywordIndex] + FirstCharacter) : 
                    (char)(FirstCharacter + (LastCharacter - keyword[keywordIndex] + textInput[i] - FirstCharacter) + 1);

                keywordIndex = incrementKeywordIndex(keyword, keywordIndex);
            }

            return textInput.ToString();
        }

        private static int incrementKeywordIndex(string keyword, int keywordIndex)
        {
            return keywordIndex + 1 == keyword.Length ? 0 : keywordIndex + 1;
        }

        private static StringBuilder getStringBuilderForCipher(string cipherText)
        {
            cipherText = cipherText.KeepOnlyAlphabetical();
            return new StringBuilder(cipherText.ToUpper());
        }

        #endregion
    }
}
