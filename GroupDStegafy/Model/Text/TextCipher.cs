using System;
using System.Text;
using GroupDStegafy.Model.Extensions;

namespace GroupDStegafy.Model.Text
{
    /// <summary>
    ///     Class responsible for methods dealing with encrypting text
    /// </summary>
    public static class TextCipher
    {
        #region Properties

        /// <summary>
        /// Gets the encryption key.
        /// </summary>
        /// <value>
        /// The encryption key.
        /// </value>
        public static string EncryptionKey { get; private set; }

        /// <summary>
        /// Gets the encrypted message.
        /// </summary>
        /// <value>
        /// The encrypted message.
        /// </value>
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
        ///     decryption purposes. Returns the passed in plain text as
        ///     is if no keyword is provided.
        ///     Precondition: plainText is not null
        ///     Post-condition: none
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns>The encrypted text with the keyword added to the front</returns>
        /// <exception cref="ArgumentNullException">plainText</exception>
        public static string EncryptTextWithKey(string plainText, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return plainText;
            }
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
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
            EncryptionKey = extractKeyword(encryptedText);
            EncryptedMessage = extractMessage(encryptedText);

            return decryptText(EncryptedMessage, EncryptionKey);
        }

        #endregion

        #region Private Helpers

        private static string extractKeyword(string encryptedText)
        {
            var keywordEndIndex = encryptedText.IndexOf(KeywordEnd, StringComparison.Ordinal);
            return encryptedText.Substring(0, keywordEndIndex);
        }

        private static string extractMessage(string encryptedText)
        {
            var keywordEndIndex = encryptedText.IndexOf(KeywordEnd, StringComparison.Ordinal) + KeywordEnd.Length;
            return encryptedText.Substring(keywordEndIndex);
        }

        private static string encryptText(string plainText, string keyword)
        {
            var textBuilder = getStringBuilderForCipher(plainText);
            keyword = keyword.ToUpper();
            var keywordIndex = 0;

            for (var i = 0; i < textBuilder.Length; i++)
            {
                textBuilder[i] = (char)(textBuilder[i] + keyword[keywordIndex] - FirstCharacter);

                if (textBuilder[i] > LastCharacter)
                {
                    textBuilder[i] = (char)(textBuilder[i] - LastCharacter + FirstCharacter - 1);
                }
                
                keywordIndex = incrementKeywordIndex(keyword, keywordIndex);
            }

            return textBuilder.ToString();
        }

        private static string decryptText(string encryptedText, string keyword)
        {
            var textBuilder = getStringBuilderForCipher(encryptedText);
            keyword = keyword.ToUpper();
            var keywordIndex = 0;

            for (var i = 0; i < textBuilder.Length; i++)
            {
                textBuilder[i] = textBuilder[i] >= keyword[keywordIndex] ? 
                    (char)(textBuilder[i] - keyword[keywordIndex] + FirstCharacter) : 
                    (char)(FirstCharacter + (LastCharacter - keyword[keywordIndex] + textBuilder[i] - FirstCharacter) + 1);

                keywordIndex = incrementKeywordIndex(keyword, keywordIndex);
            }

            return textBuilder.ToString();
        }

        private static int incrementKeywordIndex(string keyword, int keywordIndex)
        {
            return keywordIndex + 1 == keyword.Length ? 0 : keywordIndex + 1;
        }

        private static StringBuilder getStringBuilderForCipher(string cipherText)
        {
            cipherText = cipherText.ToAlphabeticalOnly();
            return new StringBuilder(cipherText.ToUpper());
        }

        #endregion
    }
}
