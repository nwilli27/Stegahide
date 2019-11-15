using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupDStegafy.Model.Extensions
{
    public static class BitmapExtensions
    {
        public static async Task<BitmapImage> ToBitmapImageAsync(this StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }
    }
}
