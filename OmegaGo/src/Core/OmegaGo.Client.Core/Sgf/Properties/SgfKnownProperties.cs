using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Parsing;
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
        private static readonly Dictionary<string, SgfKnownProperty> KnownProperties =
            new[]
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
                //Event
                new SgfKnownProperty( "EV", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Game comment
                new SgfKnownProperty( "GC", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfTextValue.Parse ),
                //Game name
                new SgfKnownProperty( "GN", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Handicap
                new SgfKnownProperty( "HA", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),                
                //Komi
                new SgfKnownProperty( "KM", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfRealValue.Parse ),
                //Opening
                new SgfKnownProperty( "ON", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Overtime
                new SgfKnownProperty( "OT", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Place
                new SgfKnownProperty( "PC", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Player Black
                new SgfKnownProperty( "PB", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Player White
                new SgfKnownProperty( "PW", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Result
                new SgfKnownProperty( "RE", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Round
                new SgfKnownProperty( "RO", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Rules
                new SgfKnownProperty( "RU", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Source
                new SgfKnownProperty( "SO", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),                
                //Timelimit
                new SgfKnownProperty( "TM", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfRealValue.Parse ),
                //User
                new SgfKnownProperty( "US", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //White rank
                new SgfKnownProperty( "WR", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //White team
                new SgfKnownProperty( "WT", SgfPropertyType.GameInfo, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ), 
                
                /* =====================
                    Move properties
                */ 
                //Black move
                new SgfKnownProperty( "B", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfPointValue.Parse ),
                //Black time left
                new SgfKnownProperty( "BL", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfRealValue.Parse ),
                //Bad move
                new SgfKnownProperty( "BM", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),                
                //Doubtful
                new SgfKnownProperty( "DO", SgfPropertyType.Move ),
                //Interesting
                new SgfKnownProperty( "IT", SgfPropertyType.Move ),
                //Ko
                new SgfKnownProperty( "KO", SgfPropertyType.Move ),
                //Set move number
                new SgfKnownProperty( "MN", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //OtStones Black
                new SgfKnownProperty( "OB", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //OtStones White
                new SgfKnownProperty( "OW", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //Tesuji
                new SgfKnownProperty( "TE", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //White move
                new SgfKnownProperty( "W", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfPointValue.Parse ),
                //White time left
                new SgfKnownProperty( "WL", SgfPropertyType.Move, SgfValueMultiplicity.Single, SgfRealValue.Parse ), 

                /* =====================
                    Setup properties
                */
                //Add black
                new SgfKnownProperty( "AB", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Add empty
                new SgfKnownProperty( "AE", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Add white
                new SgfKnownProperty( "AW", SgfPropertyType.Setup, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Player to play
                new SgfKnownProperty( "PL", SgfPropertyType.Setup, SgfValueMultiplicity.Single, SgfColorValue.Parse ), 

                /* =====================
                    Root properties
                */
                //Application
                new SgfKnownProperty( "AP", SgfPropertyType.Root, SgfValueMultiplicity.Single, Compose<string,string>( SgfSimpleTextValue.Parse, SgfSimpleTextValue.Parse) ),
                //Charset
                new SgfKnownProperty( "CA", SgfPropertyType.Root, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //File format
                new SgfKnownProperty( "FF", SgfPropertyType.Root, SgfValueMultiplicity.Single, SpecialSgfPropertyValueParsers.RangedNumberParser( 1, 4 ) ),
                //Game
                new SgfKnownProperty( "GM", SgfPropertyType.Root, SgfValueMultiplicity.Single, SpecialSgfPropertyValueParsers.GameParser ),
                //Style
                new SgfKnownProperty( "ST", SgfPropertyType.Root, SgfValueMultiplicity.Single, SpecialSgfPropertyValueParsers.RangedNumberParser( 0, 3 ) ),
                //Size
                new SgfKnownProperty( "SZ", SgfPropertyType.Root, SgfValueMultiplicity.Single, SpecialSgfPropertyValueParsers.SizeParser ), 

                /* =====================
                    No type properties
                */
                //Arrow
                new SgfKnownProperty( "AR", SgfPropertyType.NoType, SgfValueMultiplicity.List, Compose<SgfPoint,SgfPoint>( SgfPointValue.Parse, SgfPointValue.Parse) ),                
                //Comment
                new SgfKnownProperty( "C", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfTextValue.Parse ),
                //Circle
                new SgfKnownProperty( "CR", SgfPropertyType.NoType, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Dim points
                new SgfKnownProperty( "DD", SgfPropertyType.NoType, SgfValueMultiplicity.EList, SgfPointRectangleValue.Parse ),
                //Even position
                new SgfKnownProperty( "DM", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //Figure
                new SgfKnownProperty( "FG", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SpecialSgfPropertyValueParsers.FigureParser ),
                //Good for Black
                new SgfKnownProperty( "GB", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //Good for White
                new SgfKnownProperty( "GW", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //Hotspot
                new SgfKnownProperty( "HO", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //Label
                new SgfKnownProperty( "LB", SgfPropertyType.NoType, SgfValueMultiplicity.List, Compose<SgfPoint, string>( SgfPointValue.Parse, SgfSimpleTextValue.Parse )),
                //Line
                new SgfKnownProperty( "LN", SgfPropertyType.NoType, SgfValueMultiplicity.List, Compose<SgfPoint, SgfPoint>( SgfPointValue.Parse, SgfPointValue.Parse )),
                //Mark with X
                new SgfKnownProperty( "MA", SgfPropertyType.NoType, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Nodename
                new SgfKnownProperty( "N", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Print move node
                new SgfKnownProperty( "PM", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),                
                //Selected
                new SgfKnownProperty( "SL", SgfPropertyType.NoType, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Square
                new SgfKnownProperty( "SQ", SgfPropertyType.NoType, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Territory Black
                new SgfKnownProperty( "TB", SgfPropertyType.NoType, SgfValueMultiplicity.EList, SgfPointRectangleValue.Parse ),
                //Triangle
                new SgfKnownProperty( "TR", SgfPropertyType.NoType, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Territory White
                new SgfKnownProperty( "TW", SgfPropertyType.NoType, SgfValueMultiplicity.EList, SgfPointRectangleValue.Parse ),
                //Unclear position
                new SgfKnownProperty( "UC", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //Value (score)
                new SgfKnownProperty( "V", SgfPropertyType.NoType, SgfValueMultiplicity.Single, SgfRealValue.Parse ),
                //View
                new SgfKnownProperty( "VW", SgfPropertyType.NoType, SgfValueMultiplicity.EList, SgfPointRectangleValue.Parse ), 

                /* =====================
                    Deprecated properties
                */
                //Black species
                new SgfKnownProperty( "BS", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //Check mark
                new SgfKnownProperty( "CH", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //Evaluate comp move
                new SgfKnownProperty( "EL", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //Expected move
                new SgfKnownProperty( "EX", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfPointValue.Parse ),
                //Game identifier
                new SgfKnownProperty( "ID", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfSimpleTextValue.Parse ),
                //Letters
                new SgfKnownProperty( "L", SgfPropertyType.Deprecated, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Lose on time
                new SgfKnownProperty( "LT", SgfPropertyType.Deprecated ),
                //Simple markup
                new SgfKnownProperty( "M", SgfPropertyType.Deprecated, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Moves / overtime
                new SgfKnownProperty( "OM", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //Overtime length
                new SgfKnownProperty( "OP", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfRealValue.Parse ),
                //Operator overhead
                new SgfKnownProperty( "OV", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfRealValue.Parse ),
                //Region
                new SgfKnownProperty( "RG", SgfPropertyType.Deprecated, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Secure stones
                new SgfKnownProperty( "SC", SgfPropertyType.Deprecated, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Selftest moves
                new SgfKnownProperty( "SE", SgfPropertyType.Deprecated, SgfValueMultiplicity.List, SgfPointRectangleValue.Parse ),
                //Sigma
                new SgfKnownProperty( "SI", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfDoubleValue.Parse ),
                //Territory count
                new SgfKnownProperty( "TC", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
                //White species
                new SgfKnownProperty( "WS", SgfPropertyType.Deprecated, SgfValueMultiplicity.Single, SgfNumberValue.Parse ),
            }.ToDictionary(i => i.Identifier, i => i);

        /// <summary>
        /// Retrieves a known property by identifier
        /// Returns null if not known
        /// </summary>
        /// <param name="identifier">Identifier to search for</param>
        /// <returns>Known property or null</returns>
        public static SgfKnownProperty Get(string identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));
            SgfKnownProperty property = null;
            KnownProperties.TryGetValue(identifier, out property);
            return property;
        }

        /// <summary>
        /// Checks if a given property is known
        /// </summary>
        /// <param name="identifier">Identifier of the property</param>
        /// <returns>Is property known?</returns>
        public static bool Contains(string identifier) => KnownProperties.ContainsKey(identifier);

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

        /// <summary>
        /// Gets all known properties of a given type
        /// </summary>
        /// <param name="type">Type of the property</param>
        /// <returns>All known properties of requested type</returns>
        public static IEnumerable<SgfKnownProperty> GetPropertiesOfType(SgfPropertyType type)
        {
            foreach (var property in KnownProperties.Values)
            {
                if (property.Type == type)
                {
                    yield return property;
                }
            }
        }
    }
}
