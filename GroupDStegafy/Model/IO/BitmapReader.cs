using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupDStegafy.Model.IO
{
    /// <summary>
    ///     Responsible for the reading in of an image (bitmap, .png)
    /// </summary>
    class BitmapReader
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
        public async Task<Bitmap> ReadAndReturnBitmap()
        {
            var sourceImageFile = await this.selectSourceImageFile();
            //TODO move this to its own utility class/method that allows you to copy an image
            var copyBitmapImage = await this.MakeACopyOfTheFileToWorkOn(sourceImageFile);

            using (var fileStream = await sourceImageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(copyBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(copyBitmapImage.PixelHeight)
                };

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                    );

                var sourcePixels = pixelData.DetachPixelData();

                //TODO this was in his example, keeping here for now to reference later.
                //this.giveImageRedTint(sourcePixels, decoder.PixelWidth, decoder.PixelHeight);

                return new Bitmap()
                {
                    DpiX = decoder.DpiX,
                    DpiY = decoder.DpiY,
                    Image = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight)
                };
                
                //TODO was in his example, this sets the image on the display. We'd use this on the mainpage (reference for later)
                //using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
                //{
                //    await writeStream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                //    this.imageDisplay.Source = this.modifiedImage;
                //}
            }
        }

        #endregion

        #region Private Helpers

        private async Task<StorageFile> selectSourceImageFile()
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        //TODO make this a utility method maybe in utility class. Or an extension method
        private async Task<BitmapImage> MakeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputstream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputstream);
            return newImage;
        }

        #endregion
    }
}
