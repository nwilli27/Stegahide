using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupDStegafy.Model
{
    /// <summary>
    ///     Class holds the implementation for a Bitmap image
    /// </summary>
    class Bitmap
    {

        #region Properties

        //TODO base implementation, accessibility may change based on what we need.
        public double DpiX { get; set; }

        public double DpiY { get; set; }

        public WriteableBitmap Image { get; set; }

        #endregion
    }
}
