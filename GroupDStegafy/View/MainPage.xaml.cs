using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using GroupDStegafy.ViewModel;

namespace GroupDStegafy.View
{
    /// <summary>
    ///     Main page of the Stegafy application.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void loadSourceButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

        private async void loadSecretButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail
            };
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".txt");

            var file = await openPicker.PickSingleFileAsync();
            ((MainPageViewModel)DataContext).HandleLoadSecret(file);
        }
    }
}
