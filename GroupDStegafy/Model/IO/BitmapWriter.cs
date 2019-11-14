using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GroupDStegafy.Model.IO
{
    /// <summary>
    ///     Responsible for the writing of an image (bitmap, .png)
    /// </summary>
    class BitmapWriter
    {

        /// <summary>
        ///     Saves the writable bitmap image that is passed in.
        ///     Precondition: 
        /// </summary>
        /// <param name="bitmap">The modified image.</param>
        public static async void SaveWritableBitmap(Bitmap bitmap)
        {
            //TODO refactor out to private helpers.

            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> { ".png" });
            var savefile = await fileSavePicker.PickSaveFileAsync();

            if (savefile != null)
            {
                var stream = await savefile.OpenAsync(FileAccessMode.ReadWrite);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                var pixelStream = bitmap.Image.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)bitmap.Image.PixelWidth,
                    (uint)bitmap.Image.PixelHeight, bitmap.DpiX, bitmap.DpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }

    }
}
