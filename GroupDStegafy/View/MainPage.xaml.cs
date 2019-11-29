using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using GroupDStegafy.ViewModel;
using Windows.UI.Xaml;

namespace GroupDStegafy.View
{
    /// <summary>
    ///     Main page of the Stegafy application.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            ((MainPageViewModel) DataContext).TextTooLarge += showTextTooLargePopup;
            ((MainPageViewModel)DataContext).ImageTooLarge += showImageTooLargePopup;
        }

        #endregion

        #region Events

        private async void loadSourceButton_Click(object sender, RoutedEventArgs e)
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
            ((MainPageViewModel)DataContext).HandleLoadSource(file);
        }

        private async void loadSecretButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail
            };
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".txt");

            var file = await openPicker.PickSingleFileAsync();
            ((MainPageViewModel)DataContext).HandleLoadSecret(file);
        }

        private async void saveSourceButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await promptSaveImage();
            ((MainPageViewModel) DataContext).HandleSaveSource(file);
        }

        private async void saveSecretButton_Click(object sender, RoutedEventArgs e)
        {
            var file = ((MainPageViewModel)DataContext).SecretBitmap == null ? await promptSaveText() : await promptSaveImage();
            ((MainPageViewModel)DataContext).HandleSaveSecret(file);
        }

        #endregion

        #region Private Helpers

        private static async Task<StorageFile> promptSaveImage()
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> { ".png" });
            var file = await fileSavePicker.PickSaveFileAsync();
            return file;
        }

        private static async Task<StorageFile> promptSaveText()
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "text"
            };
            fileSavePicker.FileTypeChoices.Add("Plain text files", new List<string> { ".txt" });
            var file = await fileSavePicker.PickSaveFileAsync();
            return file;
        }

        private static async void showTextTooLargePopup(object sender, object e)
        {
            var messageToLargeDialog = new ContentDialog
            {
                Title = "Message To Large",
                Content = "The message exceeds the number of pixels available to encode." + Environment.NewLine +
                          "Please increase the (Bits Per Color Channel) or decrease total # of words.",
                CloseButtonText = "Ok"
            };

            await messageToLargeDialog.ShowAsync();
        }

        private static async void showImageTooLargePopup(object sender, object e)
        {
            var messageToLargeDialog = new ContentDialog
            {
                Title = "Message To Large",
                Content = "The secret image is larger than the source image." + Environment.NewLine +
                          "Please use a larger source image or a smaller secret image.",
                CloseButtonText = "Ok"
            };

            await messageToLargeDialog.ShowAsync();
        }

        #endregion
    }
}
