using System;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Exception thrown when a message cannot be embedded in an image due to size.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class MessageTooLargeException : Exception
    {
    }
}
