using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage;
using GroupDStegafy.Model.Image;

namespace GroupDStegafy.FileIO
{
    /// <summary>
    ///     Responsible for the writing of an image (bitmap, .png)
    /// </summary>
    public static class BitmapWriter
    {
        /// <summary>
        ///     Saves the writable bitmap image that is passed in.
        ///     Precondition: 
        /// </summary>
        /// <param name="bitmap">The modified image.</param>
        public static async void SaveWritableBitmap(StorageFile savefile, Bitmap bitmap)
        {
            var modifiedImage = await bitmap.AsWritableBitmapAsync();
            var stream = await savefile.OpenAsync(FileAccessMode.ReadWrite);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            var pixelStream = modifiedImage.PixelBuffer.AsStream();
            var pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                (uint)modifiedImage.PixelWidth,
                (uint)modifiedImage.PixelHeight, bitmap.DpiX, bitmap.DpiY, pixels);
            await encoder.FlushAsync();

            stream.Dispose();
        }
    }
}
