using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Properties.Known
{
    public class SgfAddBlackProperty : SgfProperty
    {
        public SgfAddBlackProperty(params SgfPointRectangle[] pointRectangles) :
            base("AB",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        {
        }
    }

    public class SgfAddEmptyProperty : SgfProperty
    {
        public SgfAddEmptyProperty(params SgfPointRectangle[] pointRectangles) :
            base("AE",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        {
        }
    }

    public class SgfAddWhiteProperty : SgfProperty { 
        public SgfAddWhiteProperty(params SgfPointRectangle[] pointRectangles):
            base("AW",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        {
        }
    }

    public class SgfPlayerToPlayProperty : SgfProperty
    {
        public SgfPlayerToPlayProperty(SgfColor color) : 
            base( "PL",
                new SgfColorValue(color)
            )
        { }
    }
}
