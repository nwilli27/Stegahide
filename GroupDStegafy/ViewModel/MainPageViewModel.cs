using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupDStegafy.Annotations;
using GroupDStegafy.Model.Image;
using GroupDStegafy.Model.IO;

namespace GroupDStegafy.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Bitmap sourceBitmap;
        private Bitmap secretBitmap;

        public Bitmap SourceBitmap
        {
            get => this.sourceBitmap;
            set
            {
                this.sourceBitmap = value;
                this.OnPropertyChanged(nameof(this.SourceBitmap));
                this.OnPropertyChanged(nameof(this.SourceWriteableBitmap));
            }
        }

        public Bitmap SecretBitmap
        {
            get => this.secretBitmap;
            set
            {
                this.secretBitmap = value;
                this.OnPropertyChanged(nameof(this.SecretBitmap));
                this.OnPropertyChanged(nameof(this.SecretWriteableBitmap));
            }
        }

        public WriteableBitmap SourceWriteableBitmap => this.SourceBitmap?.AsWritableBitmapAsync().Result;
        public WriteableBitmap SecretWriteableBitmap => this.SecretBitmap?.AsWritableBitmapAsync().Result;

        public async void HandleLoadSource(StorageFile file)
        {
            if (file != null)
            {
                this.SourceBitmap = await BitmapReader.ReadAndReturnBitmap(file);
            }
        }

        public async void HandleLoadSecret(StorageFile file)
        {
            if (file != null)
            {
                // TODO make sure that the image is either monochrome or text, and handle text appropriately
                this.SecretBitmap = await BitmapReader.ReadAndReturnBitmap(file);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
