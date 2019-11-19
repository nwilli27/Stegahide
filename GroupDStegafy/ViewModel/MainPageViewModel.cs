
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
        private MonochromeBitmap secretBitmap;
        private string secretText;
        private bool canSaveSource;
        private bool canSaveSecret;

        public Bitmap SourceBitmap
        {
            get => this.sourceBitmap;
            private set
            {
                this.sourceBitmap = value;
                this.OnPropertyChanged(nameof(this.SourceBitmap));
                this.OnPropertyChanged(nameof(this.SourceWriteableBitmap));
                this.EncodeCommand.OnCanExecuteChanged();
                this.DecodeCommand.OnCanExecuteChanged();
            }
        }

        public MonochromeBitmap SecretBitmap
        {
            get => this.secretBitmap;
            private set
            {
                this.secretBitmap = value;
                this.OnPropertyChanged(nameof(this.SecretBitmap));
                this.OnPropertyChanged(nameof(this.SecretWriteableBitmap));
                this.EncodeCommand.OnCanExecuteChanged();
            }
        }

        public WriteableBitmap SourceWriteableBitmap => this.SourceBitmap?.AsWritableBitmapAsync().Result;

        public WriteableBitmap SecretWriteableBitmap => this.secretBitmap?.ToBitmap().AsWritableBitmapAsync().Result;

        public bool CanSaveSource
        {
            get => this.canSaveSource;
            private set
            {
                this.canSaveSource = value;
                this.OnPropertyChanged(nameof(this.CanSaveSource));
            }
        }

        public bool CanSaveSecret
        {
            get => this.canSaveSecret;
            private set
            {
                this.canSaveSecret = value;
                this.OnPropertyChanged(nameof(this.CanSaveSecret));
            }
        }

        public RelayCommand EncodeCommand { get; }
        public RelayCommand DecodeCommand { get; }

        public MainPageViewModel()
        {
            this.EncodeCommand = new RelayCommand(this.encodeMessage, this.canEncodeMessage);
            this.DecodeCommand = new RelayCommand(this.decodeMessage, this.canDecodeMessage);
        }

        public async void HandleLoadSource(StorageFile file)
        {
            if (file != null)
            {
                this.SourceBitmap = await BitmapReader.ReadAndReturnBitmap(file);
                this.CanSaveSource = false;
            }
        }

        public async void HandleLoadSecret(StorageFile file)
        {
            if (file != null)
            {
                // TODO handle text files differently
                var bitmap = await BitmapReader.ReadAndReturnBitmap(file);
                this.SecretBitmap = new MonochromeBitmap(bitmap);
                this.CanSaveSecret = false;
            }
        }

        public void HandleSaveSource(StorageFile file)
        {
            if (file != null)
            {
                BitmapWriter.SaveWritableBitmap(file, this.SourceBitmap);
            }
        }

        public void HandleSaveSecret(StorageFile file)
        {
            if (file != null)
            {
                // TODO handle saving text
                BitmapWriter.SaveWritableBitmap(file, this.SecretBitmap.ToBitmap());
            }
        }

        private void encodeMessage(object obj)
        {
            this.SourceBitmap.EmbedMonochromeImage(this.SecretBitmap);
            this.OnPropertyChanged(nameof(this.SourceBitmap));
            this.CanSaveSource = true;
        }

        private bool canEncodeMessage(object obj)
        {
            return this.sourceBitmap != null && (this.secretBitmap != null || this.secretText != null);
        }

        private void decodeMessage(object obj)
        {
            this.SecretBitmap = new MonochromeBitmap(this.SourceBitmap);
            this.CanSaveSecret = true;
        }

        private bool canDecodeMessage(object obj)
        {
            return this.sourceBitmap != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
