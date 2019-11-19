using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupDStegafy.Model.Image
{
    /// <summary>
    ///     Holds generic implementation for an image.
    /// </summary>
    public abstract class Image
    {

        #region Properties

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public uint Width { get; protected set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public uint Height { get; protected set; }

        #endregion
    }
}
