using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupDStegafy.Model.Extensions
{
    /// <summary>
    ///     Extension methods for bitmaps.
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        ///     Converts a storage file with a bitmap to a BitmapImage asynchronously.
        ///     Precondition: None
        ///     Postcondition None
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <returns>The image contained in the file.</returns>
        public static async Task<BitmapImage> ToBitmapImageAsync(this StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }
    }
}
