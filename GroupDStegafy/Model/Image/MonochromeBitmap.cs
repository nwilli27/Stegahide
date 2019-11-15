using Windows.UI;

namespace GroupDStegafy.Model.Image
{
    public class MonochromeBitmap
    {
        public bool[] Pixels { get; }
        public uint Width { get; }
        private uint Height => (uint)(this.Pixels.Length / this.Width);

        public MonochromeBitmap(Bitmap source)
        {
            this.Width = source.Width;
            this.Pixels = new bool[source.Width * source.Height];
            for (var i = 0; i < this.Pixels.Length; i++)
            {
                var lsb = (source.GetPixelBgra8((int) (i % this.Width), (int) (i / this.Width)).B & 1) == 1;
                this.Pixels[i] = lsb;
            }
        }

        public Bitmap ToBitmap()
        {
            var bytes = new byte[this.Pixels.Length * 4];
            var bitmap = new Bitmap(bytes, this.Width, 1, 1);
            for (var i = 0; i < this.Pixels.Length; i++)
            {
                bitmap.SetPixelBgra8((int) (i % this.Width), (int)(i / this.Width), this.Pixels[i] ? Colors.White : Colors.Black);
            }

            return bitmap;
        }
    }
}
