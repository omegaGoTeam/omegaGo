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
        public SgfArrowProperty(params SgfComposeValue<SgfPoint, SgfPoint>[] arrows) :
            base("AR",
                arrows.Select(ar => new SgfComposePropertyValue<SgfPoint, SgfPoint>(
                   new SgfPointValue(ar.Left),
                   new SgfPointValue(ar.Right))).ToArray<ISgfPropertyValue>())
        { }
    }

    public class SgfCircleProperty : SgfProperty
    {
        public SgfCircleProperty(params SgfPointRectangle[] pointRectangles) :
            base("CR",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        { }
    }

    public class SgfDimPointProperty : SgfProperty
    {
        public SgfDimPointProperty(params SgfPointRectangle[] pointRectangles) :
            base("DD",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        { }
    }

    public class SgfLabelProperty : SgfProperty
    {
        public SgfLabelProperty(params SgfComposeValue<SgfPoint, string>[] labels) :
            base("LB",
                labels.Select(pr => new SgfComposePropertyValue<SgfPoint, string>(
                    new SgfPointValue(pr.Left), new SgfSimpleTextValue(pr.Right))).ToArray<ISgfPropertyValue>())
        { }
    }

    public class SgfLineProperty : SgfProperty
    {
        public SgfLineProperty(params SgfComposeValue<SgfPoint, SgfPoint>[] arrows) :
            base("LN",
                arrows.Select(ar => new SgfComposePropertyValue<SgfPoint, SgfPoint>(
                    new SgfPointValue(ar.Left),
                    new SgfPointValue(ar.Right))).ToArray<ISgfPropertyValue>())
        { }
    }

    public class SgfCrossProperty : SgfProperty
    {
        public SgfCrossProperty(params SgfPointRectangle[] pointRectangles) :
            base("MA",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        { }
    }

    public class SgfSquareProperty : SgfProperty
    {
        public SgfSquareProperty(params SgfPointRectangle[] pointRectangles) :
            base("SQ",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        { }
    }

    public class SgfTriangleProperty : SgfProperty
    {
        public SgfTriangleProperty(params SgfPointRectangle[] pointRectangles) :
            base("TR",
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>())
        { }
    }
}
