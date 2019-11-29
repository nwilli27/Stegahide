using System;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Exception thrown when a secret image or text cannot be embedded in an image due to size.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class SecretTooLargeException : Exception
    {
    }
}
