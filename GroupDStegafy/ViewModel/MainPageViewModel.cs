
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupDStegafy.Annotations;
using GroupDStegafy.FileIO;
using GroupDStegafy.Model.Image;

namespace GroupDStegafy.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private Bitmap sourceBitmap;
        private MonochromeBitmap secretBitmap;
        private string secretText;
        private bool canSaveSource;
        private bool canSaveSecret;

        /// <summary>
        ///     Gets the source bitmap.
        /// </summary>
        /// <value>
        ///     The source bitmap.
        /// </value>
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

        /// <summary>
        ///     Gets the secret bitmap.
        /// </summary>
        /// <value>
        ///     The secret bitmap.
        /// </value>
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

        /// <summary>
        ///     Gets the source bitmap as a WritableBitmap.
        /// </summary>
        /// <value>
        ///     The source bitmap as a WritableBitmap.
        /// </value>
        public WriteableBitmap SourceWriteableBitmap => this.SourceBitmap?.AsWritableBitmapAsync().Result;

        /// <summary>
        ///     Gets the secret bitmap as a WritableBitmap.
        /// </summary>
        /// <value>
        ///     The secret bitmap as a WritableBitmap.
        /// </value>
        public WriteableBitmap SecretWriteableBitmap => this.secretBitmap?.ToBitmap().AsWritableBitmapAsync().Result;

        /// <summary>
        ///     Gets a value indicating whether the source image can be saved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can save source; otherwise, <c>false</c>.
        /// </value>
        public bool CanSaveSource
        {
            get => this.canSaveSource;
            private set
            {
                this.canSaveSource = value;
                this.OnPropertyChanged(nameof(this.CanSaveSource));
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the decoded secret image or text can be saved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can save secret; otherwise, <c>false</c>.
        /// </value>
        public bool CanSaveSecret
        {
            get => this.canSaveSecret;
            private set
            {
                this.canSaveSecret = value;
                this.OnPropertyChanged(nameof(this.CanSaveSecret));
            }
        }

        /// <summary>
        ///     Gets the encode command.
        /// </summary>
        /// <value>
        ///     The encode command.
        /// </value>
        public RelayCommand EncodeCommand { get; }
        /// <summary>
        ///     Gets the decode command.
        /// </summary>
        /// <value>
        ///     The decode command.
        /// </value>
        public RelayCommand DecodeCommand { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.EncodeCommand = new RelayCommand(this.encodeMessage, this.canEncodeMessage);
            this.DecodeCommand = new RelayCommand(this.decodeMessage, this.canDecodeMessage);
        }

        /// <summary>
        ///     Handles loading the source image file.
        ///     Precondition: None
        ///     Postcondition: The source image file is loaded, this.CanSaveSource = false
        /// </summary>
        /// <param name="file">The file.</param>
        public async void HandleLoadSource(StorageFile file)
        {
            if (file != null)
            {
                this.SourceBitmap = await BitmapReader.ReadBitmap(file);
                this.CanSaveSource = false;
            }
        }

        /// <summary>
        ///     Handles loading the secret.
        ///     Precondition: None
        ///     Postcondition: The secret is loaded, this.CanSaveSecret = false
        /// </summary>
        /// <param name="file">The file.</param>
        public async void HandleLoadSecret(StorageFile file)
        {
            if (file != null)
            {
                // TODO handle text files differently
                var bitmap = await BitmapReader.ReadBitmap(file);
                this.SecretBitmap = new MonochromeBitmap(bitmap);
                this.CanSaveSecret = false;
            }
        }

        /// <summary>
        ///     Handles saving the source image, now with an embedded secret, to a file.
        ///     Precondition: None
        ///     Postcondition: The source image is saved to disk.
        /// </summary>
        /// <param name="file">The file.</param>
        public void HandleSaveSource(StorageFile file)
        {
            if (file != null)
            {
                BitmapWriter.WriteBitmap(file, this.SourceBitmap);
            }
        }

        /// <summary>
        ///     Handles saving the extracted secret to a file.
        ///     Precondition: None
        ///     Postcondition: The secret is saved to disk.
        /// </summary>
        /// <param name="file">The file.</param>
        public void HandleSaveSecret(StorageFile file)
        {
            if (file != null)
            {
                // TODO handle saving text
                BitmapWriter.WriteBitmap(file, this.SecretBitmap.ToBitmap());
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

        /// <summary>
        ///     Called when an observable property changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
