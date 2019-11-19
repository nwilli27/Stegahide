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
    public sealed partial class MainPage : Page
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
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
            var file = await this.promptSaveImage();
            ((MainPageViewModel) DataContext).HandleSaveSource(file);
        }

        private async void saveSecretButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO handle saving text
            var file = await this.promptSaveImage();
            ((MainPageViewModel)DataContext).HandleSaveSecret(file);
        }

        #endregion

        #region Private Helpers

        private async Task<StorageFile> promptSaveImage()
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

        #endregion
    }
}
