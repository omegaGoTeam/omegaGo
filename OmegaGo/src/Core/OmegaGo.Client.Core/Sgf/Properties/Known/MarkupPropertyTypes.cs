using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Properties.Known
{
    public class SgfArrowProperty : SgfProperty
    {
        public SgfArrowProperty(SgfPoint from, SgfPoint to) :
            base("AR",
                new SgfComposePropertyValue<SgfPoint, SgfPoint>(
                    new SgfPointValue(from),
                    new SgfPointValue(to)))
        { }
    }

    public class SgfCircleProperty : SgfProperty
    {
        public SgfCircleProperty(params SgfPointRectangle[] pointRectangles) :
            base("CR",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        {
        }
    }

    public class SgfDimPointProperty : SgfProperty
    {
        public SgfDimPointProperty(params SgfPointRectangle[] pointRectangles) :
            base("DD",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        {
        }
    }

    public class SgfLabelProperty : SgfProperty
    {
        public SgfLabelProperty(params SgfComposeValue<SgfPoint, string>[] labels) :
            base("LB",
                labels.Select(pr => new SgfComposePropertyValue<SgfPoint, string>(
                    new SgfPointValue(pr.Left), new SgfSimpleTextValue(pr.Right))).ToArray<ISgfPropertyValue>())
        {
        }
    }
}
