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
                /* =====================
                    Game info properties
                */
                //Annotation
                new SgfKnownProperty( "AN", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ), 
                //Black rank
                new SgfKnownProperty( "BR", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ), 
                //Black team
                new SgfKnownProperty( "BT", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Copyright
                new SgfKnownProperty( "CP", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Date
                new SgfKnownProperty( "DT", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),     
                
                /* =====================
                    Move properties
                */ 
                //Black move
                new SgfKnownProperty( "B", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfPointValue.Parse ),
                //Black time left
                new SgfKnownProperty( "BL", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfRealValue.Parse ),
                //Bad move
                new SgfKnownProperty( "BM", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),                
                //White move
                new SgfKnownProperty( "W", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfPointValue.Parse ),
                //Doubtful
                new SgfKnownProperty( "DO", SgfPropertyType.Move ), 

                /* =====================
                    Setup properties
                */
                //Add black
                new SgfKnownProperty( "AB", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointValue.Parse ),
                //Add empty
                new SgfKnownProperty( "AE", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointValue.Parse ),
                //Add white
                new SgfKnownProperty( "AW", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointValue.Parse ),

                /* =====================
                    Root properties
                */
                //Application
                new SgfKnownProperty( "AP", SgfPropertyType.Root, SgfValueMultiplicity.Single, Compose<string,string>( SgfSimpleTextValue.Parse, SgfSimpleTextValue.Parse) ),
                //Charset
                new SgfKnownProperty( "CA", SgfPropertyType.Root, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ), 
                /* =====================
                    No type properties
                */
                //Arrow
                new SgfKnownProperty( "AR", SgfPropertyType.NoType, SgfValueMultiplicity.List, Compose<SgfPoint,SgfPoint>( SgfPointValue.Parse, SgfPointValue.Parse) ),
                //Who adds stones
                new SgfKnownProperty( "AS", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Comment
                new SgfKnownProperty( "C", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfTextValue.Parse ),
                //Circle
                new SgfKnownProperty( "CR", SgfPropertyType.NoType, SgfValueMultiplicity.List, SgfPointValue.Parse ),
                //Dim points
                new SgfKnownProperty( "DD", SgfPropertyType.NoType, SgfValueMultiplicity.EList, SgfPointValue.Parse ),
                //Even position
                new SgfKnownProperty( "DM", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ), 
                
                /* =====================
                    Deprecated properties
                */
                //Black species
                new SgfKnownProperty( "BS", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //Check mark
                new SgfKnownProperty( "CH", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
            }.ToDictionary(i => i.Identifier, i => i);

        /// <summary>
        /// Generates SGF Property Value parser for compose values
        /// </summary>
        /// <typeparam name="TLeft">Type of the left value</typeparam>
        /// <typeparam name="TRight">Type of the right value</typeparam>
        /// <param name="left">Left value parser</param>
        /// <param name="right">Right value parser</param>
        /// <returns></returns>
        private static SgfPropertyValueParser Compose<TLeft, TRight>(SgfPropertyValueParser left, SgfPropertyValueParser right)
        {
            //create a parser function using closure
            return (value) => SgfComposePropertyValue<TLeft, TRight>.Parse(value, left, right);
        }
    }
}
