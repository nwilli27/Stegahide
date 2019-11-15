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

        public WriteableBitmap SourceWriteableBitmap => this.SourceBitmap?.AsWritableBitmapAsync().Result ?? null;

        public async void HandleLoadBitmap(StorageFile file)
        {
            if (file != null)
            {
                this.SourceBitmap = await BitmapReader.ReadAndReturnBitmap(file);
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
