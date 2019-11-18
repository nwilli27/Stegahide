using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using GroupDStegafy.Model.Extensions;
using GroupDStegafy.Model.Image;

namespace GroupDStegafy.FileIO
{
    /// <summary>
    ///     Responsible for the reading in of an image (bitmap, .png)
    /// </summary>
    public static class BitmapReader
    {
        #region Methods 

        /// <summary>
        ///     Reads in an source image, makes a copy and transforms it   
        ///     to a writable image and returns a Bitmap object.
        ///     Precondition: none
        ///     Post-condition: none
        /// </summary>
        /// <returns>
        ///     A Bitmap object with now a writable bitmap image
        /// </returns>
        public static async Task<Bitmap> ReadBitmap(StorageFile sourceImageFile)
        {
            var bitmapImage = await sourceImageFile.ToBitmapImageAsync();

            using (var fileStream = await sourceImageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(bitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(bitmapImage.PixelHeight)
                };

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                    );

                var sourcePixels = pixelData.DetachPixelData();

                return new Bitmap(sourcePixels, transform.ScaledWidth, (uint)decoder.DpiX, (uint)decoder.DpiY);
            }
        }

        #endregion
    }
}
