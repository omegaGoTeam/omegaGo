using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Properties.Known
{
    /// <summary>
    /// AP property
    /// </summary>
    public class SgfApplicationProperty : SgfProperty
    {
        public SgfApplicationProperty(string name, string version) :
            base("AP",
                new SgfComposePropertyValue<string, string>(
                    new SgfSimpleTextValue(name),
                    new SgfSimpleTextValue(version)))
        {
        }
    }

    /// <summary>
    /// CA property
    /// </summary>
    public class SgfCharsetProperty : SgfProperty
    {
        /// <summary>
        /// Creates default UTF-8 charset
        /// </summary>
        public SgfCharsetProperty() :
            base("CA", new SgfSimpleTextValue("UTF-8"))
        { }

        public SgfCharsetProperty(string charset) :
                base("CA",
                    new SgfSimpleTextValue(charset))
        { }
    }

    /// <summary>
    /// FF property
    /// </summary>
    public class SgfFileFormatProperty : SgfProperty
    {
        public SgfFileFormatProperty(int fileFormat) :
            base("FF",
                new SgfNumberValue(fileFormat))
        { }
    }

    /// <summary>
    /// GM property
    /// </summary>
    public class SgfGameProperty : SgfProperty
    {
        /// <summary>
        /// Only GM value 1 (Go) is supported
        /// </summary>
        public SgfGameProperty() :
            base("GM",
                new SgfNumberValue(1))
        { }
    }

    /// <summary>
    /// ST property
    /// </summary>
    public class SgfStyleProperty : SgfProperty
    {
        /// <summary>
        /// Style property
        /// </summary>
        /// <param name="style">Style - range 0 - 3</param>
        public SgfStyleProperty(int style) :
            base("ST",
                new SgfNumberValue(style))
        {
            if (style < 0 || style > 3) throw new ArgumentOutOfRangeException(nameof(style));
        }
    }

    /// <summary>
    /// SZ property
    /// </summary>
    public class SgfSizeProperty : SgfProperty
    {
        /// <summary>
        /// Creates size property for rectangular boards
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public SgfSizeProperty(int width, int height) :
            base("SZ", (width != height ? (ISgfPropertyValue)new SgfComposePropertyValue<int, int>(
                new SgfNumberValue(width),
                new SgfNumberValue(height)) : new SgfNumberValue(width)))
        {
        }

        /// <summary>
        /// Creates size property for square board
        /// </summary>
        /// <param name="size">Size</param>
        public SgfSizeProperty(int size) :
            base("SZ", new SgfNumberValue(size))
        { }
    }
}
