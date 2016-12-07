using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Properties
{
    /// <summary>
    /// Contains the definitions of known SGF properties
    /// </summary>
    internal static class SgfKnownProperties
    {
        /// <summary>
        /// This dictionary contains definitions for all known SGF properties
        /// </summary>
        private static Dictionary<string, SgfKnownProperty> _knownProperties =
            new SgfKnownProperty[]
            {
                //Game info properties
                new SgfKnownProperty( "AN", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ), 
                //Move properties
                
                //Setup properties
                new SgfKnownProperty( "AB", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointValue.Parse ),
                new SgfKnownProperty( "AE", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointValue.Parse ),
                
                //Root properties
                new SgfKnownProperty( "AP", SgfPropertyType.Root, SgfValueMultiplicity.Single, Compose<string,string>( SgfSimpleTextValue.Parse, SgfSimpleTextValue.Parse) ),

                //No type properties
                new SgfKnownProperty( "AR", SgfPropertyType.NoType, SgfValueMultiplicity.List, Compose<SgfPoint,SgfPoint>( SgfPointValue.Parse, SgfPointValue.Parse) ),
                new SgfKnownProperty( "AS", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ), 
                
                //Deprecated properties
            }.ToDictionary(i => i.Identifier, i => i);

        private static SgfPropertyValueParser Compose<TLeft, TRight>(SgfPropertyValueParser left, SgfPropertyValueParser right)
        {
            return (value) => SgfComposePropertyValue<TLeft, TRight>.Parse(value, left, right);
        }
    }
}
