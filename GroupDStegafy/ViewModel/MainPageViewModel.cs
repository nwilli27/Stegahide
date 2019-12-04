
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GroupDStegafy.Annotations;
using GroupDStegafy.FileIO;
using GroupDStegafy.Model.Extensions;
using GroupDStegafy.Model.Image;
using GroupDStegafy.Model.Text;

namespace GroupDStegafy.ViewModel
{
    /// <summary>
    ///     Class responsible for the view to model interaction.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Data Members 

        private Bitmap sourceBitmap;
        private MonochromeBitmap secretBitmap;
        private string secretText;
        private string encryptionKey;
        private int bitsPerColorChannel;

        private bool canSaveSource;
        private bool canSaveSecret;
        private bool encryptImageSelected;
        private bool showEncryptedSelected;

        #endregion

        #region Properties

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
                this.OnPropertyChanged(nameof(this.ImageEncryptionVisibility));
                this.OnPropertyChanged(nameof(this.ShowEncryptedVisibility));
                this.OnPropertyChanged(nameof(this.TextEncodingVisibility));
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
                this.OnPropertyChanged(nameof(this.SecretBitmapVisibility));
                this.OnPropertyChanged(nameof(this.ImageEncryptionVisibility));
                this.EncodeCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the secret text.
        /// </summary>
        /// <value>
        /// The secret text.
        /// </value>
        public string SecretText
        {
            get => this.secretText;
            set
            {
                this.secretText = string.IsNullOrEmpty(value) ? value : value.ToAlphabeticalOnly();
                this.OnPropertyChanged(nameof(this.SecretText));
                this.OnPropertyChanged(nameof(this.SecretTextVisibility));
                this.OnPropertyChanged(nameof(this.EncryptionVisibility));
                this.OnPropertyChanged(nameof(this.TextEncodingVisibility));
                this.EncodeCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the encryption key.
        /// </summary>
        /// <value>
        /// The encryption key.
        /// </value>
        public string EncryptionKey
        {
            get => this.encryptionKey;
            set
            {
                this.encryptionKey = value;
                this.OnPropertyChanged(nameof(this.EncryptionKey));
            }
        }

        /// <summary>
        /// Gets or sets the bits per color channel.
        /// </summary>
        /// <value>
        /// The bits per color channel.
        /// </value>
        public int BitsPerColorChannel
        {
            get => this.bitsPerColorChannel;
            set
            {
                this.bitsPerColorChannel = value;
                this.OnPropertyChanged(nameof(this.BitsPerColorChannel));
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

        #endregion

        #region Visibility Properties

        /// <summary>
        /// Gets the secret bitmap visibility.
        /// </summary>
        /// <value>
        /// The secret bitmap visibility.
        /// </value>
        public Visibility SecretBitmapVisibility => this.SecretBitmap != null ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the secret text visibility.
        /// </summary>
        /// <value>
        /// The secret text visibility.
        /// </value>
        public Visibility SecretTextVisibility => this.SecretText != null ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the text encoding visibility.
        /// </summary>
        /// <value>
        /// The text encoding visibility.
        /// </value>
        public Visibility TextEncodingVisibility => this.SourceBitmap != null && this.SecretText != null ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the image encryption visibility.
        /// </summary>
        /// <value>
        /// The image encryption visibility.
        /// </value>
        public Visibility ImageEncryptionVisibility => this.sourceBitmap != null && this.secretBitmap != null ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the encryption visibility.
        /// </summary>
        /// <value>
        /// The encryption visibility.
        /// </value>
        public Visibility EncryptionVisibility => (this.secretBitmap != null || this.secretText != null) && this.SourceBitmap != null && this.SourceBitmap.HasEncryption
            ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the show encrypted checkbox visibility.
        /// </summary>
        /// <value>
        /// The show encrypted visibility.
        /// </value>
        public Visibility ShowEncryptedVisibility => this.SourceBitmap.HasSecretMessage && this.SourceBitmap.HasEncryption
            ? Visibility.Visible : Visibility.Collapsed;

        #endregion

        #region Boolean Properties

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
        /// Gets or sets a value indicating whether [encrypt image selected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [encrypt image selected]; otherwise, <c>false</c>.
        /// </value>
        public bool EncryptImageSelected
        {
            get => this.encryptImageSelected;
            set
            {
                this.encryptImageSelected = value;
                this.OnPropertyChanged(nameof(this.EncryptImageSelected));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show encrypted selected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show encrypted selected]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowEncryptedSelected
        {
            get => this.showEncryptedSelected;
            set
            {
                this.showEncryptedSelected = value;
                this.OnPropertyChanged(nameof(this.ShowEncryptedSelected));
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when the user tries to embed a text message in an image where it will not fit.
        /// </summary>
        public event EventHandler TextTooLarge;

        /// <summary>
        ///     Occurs when the user tries to embed a monochrome image in an image where it will not fit.
        /// </summary>
        public event EventHandler ImageTooLarge;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.EncodeCommand = new RelayCommand(this.encodeMessage, this.canEncodeMessage);
            this.DecodeCommand = new RelayCommand(this.decodeMessage, this.canDecodeMessage);
            this.bitsPerColorChannel = 1;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Handles loading the source image file.
        ///     Precondition: None
        ///     Post-condition: The source image file is loaded, this.CanSaveSource = false
        /// </summary>
        /// <param name="file">The file.</param>
        public async void HandleLoadSource(StorageFile file)
        {
            if (file != null)
            {
                this.SourceBitmap = await BitmapReader.ReadBitmap(file);
                this.CanSaveSource = false;
                if (this.SourceBitmap.HasSecretMessage)
                {
                    this.decodeMessage(null);
                }
            }
        }

        /// <summary>
        ///     Handles loading the secret.
        ///     Precondition: None
        ///     Post-condition: The secret is loaded, this.CanSaveSecret = false
        /// </summary>
        /// <param name="file">The file.</param>
        public async void HandleLoadSecret(StorageFile file)
        {
            if (file != null)
            {
                if (file.Name.EndsWith(".txt"))
                {
                    this.SecretText = await Windows.Storage.FileIO.ReadTextAsync(file);
                    this.BitsPerColorChannel = 1;
                    this.EncryptionKey = null;
                    this.SecretBitmap = null;
                }
                else
                {
                    var bitmap = await BitmapReader.ReadBitmap(file);
                    this.SecretBitmap = MonochromeBitmap.FromBitmap(bitmap);
                    this.SecretText = null;
                }
                this.CanSaveSecret = false;
            }
        }

        /// <summary>
        ///     Handles saving the source image, now with an embedded secret, to a file.
        ///     Precondition: None
        ///     Post-condition: The source image is saved to disk.
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
        ///     Post-condition: The secret is saved to disk.
        /// </summary>
        /// <param name="file">The file.</param>
        public async void HandleSaveSecret(StorageFile file)
        {
            if (file != null)
            {
                if (this.SecretBitmap == null)
                {
                    await Windows.Storage.FileIO.WriteTextAsync(file, this.SecretText);
                }
                else
                {
                    BitmapWriter.WriteBitmap(file, this.SecretBitmap.ToBitmap());
                }
            }
        }

        #endregion

        #region Private Helpers

        private void encodeMessage(object obj)
        {
            if (this.secretBitmap != null)
            {
                try
                {
                    this.SourceBitmap.EmbedMonochromeImage(this.SecretBitmap, this.EncryptImageSelected);
                }
                catch (SecretTooLargeException)
                {
                    this.ImageTooLarge?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                try
                {
                    this.SourceBitmap.EmbedTextMessage(this.SecretText, this.EncryptionKey, this.BitsPerColorChannel);
                }
                catch (SecretTooLargeException)
                {
                    this.TextTooLarge?.Invoke(this, EventArgs.Empty);
                }
            }

            this.OnPropertyChanged(nameof(this.SourceWriteableBitmap));
            this.EncodeCommand.OnCanExecuteChanged();
            this.DecodeCommand.OnCanExecuteChanged();
            this.CanSaveSource = true;
        }

        private void decodeMessage(object obj)
        {
            if (this.SourceBitmap.IsSecretText)
            {
                this.SecretText = this.SourceBitmap.DecodeTextMessage();
                this.BitsPerColorChannel = this.SourceBitmap.BitsPerColorChannel;

                if (this.SourceBitmap.HasEncryption)
                {
                    this.EncryptionKey = TextCipher.EncryptionKey;
                }
                if (this.ShowEncryptedSelected && this.SourceBitmap.HasEncryption)
                {
                    this.SecretText = TextDecoder.RemoveDecodeIndicator(TextCipher.EncryptedMessage);
                }

                this.SecretBitmap = null;
            }
            else
            {
                this.SecretBitmap = MonochromeBitmap.FromEmbeddedSecret(this.SourceBitmap);
                if (this.SourceBitmap.HasEncryption && !this.ShowEncryptedSelected)
                {
                    this.SecretBitmap = this.SecretBitmap.GetFlipped();
                }
                this.SecretText = null;
            }
            
            this.CanSaveSecret = true;
        }

        private bool canDecodeMessage(object obj)
        {
            return this.sourceBitmap != null &&
                   this.sourceBitmap.HasSecretMessage;
        }

        private bool canEncodeMessage(object obj)
        {
            return this.sourceBitmap != null && (this.secretBitmap != null || this.secretText != null);
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
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

        #endregion
    }
}
