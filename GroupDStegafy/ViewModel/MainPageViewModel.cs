using System;
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
        private string secretText;

        public Bitmap SourceBitmap
        {
            get => this.sourceBitmap;
            set
            {
                this.sourceBitmap = value;
                this.OnPropertyChanged(nameof(this.SourceBitmap));
                this.OnPropertyChanged(nameof(this.SourceWriteableBitmap));
                this.EncodeCommand.OnCanExecuteChanged();
                this.DecodeCommand.OnCanExecuteChanged();
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
                this.EncodeCommand.OnCanExecuteChanged();
            }
        }

        public WriteableBitmap SourceWriteableBitmap => this.SourceBitmap?.AsWritableBitmapAsync().Result;
        public WriteableBitmap SecretWriteableBitmap => this.SecretBitmap?.AsWritableBitmapAsync().Result;

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

        private void encodeMessage(object obj)
        {
            throw new NotImplementedException();
        }

        private bool canEncodeMessage(object obj)
        {
            return this.sourceBitmap != null && (this.secretBitmap != null || this.secretText != null);
        }

        private void decodeMessage(object obj)
        {
            throw new NotImplementedException();
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
