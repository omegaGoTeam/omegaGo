using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmegaGo.Core.Online.Kgs.Datatypes
{
    /// <summary>
    /// The SGF in KGS is not quite like "standard" SGF. Multiple marks of the same type are considered different properties, so for example TR[aa][bb][cc] on KGS would be three different properties, one for each location. All rules-related properties (SZ[], TM[], etc) are grouped together in one "rules" property, etc. Furthere, some properties, such as DEAD, are not part of SGF but are used internally by KGS to track the state of the game board.
    /// </summary>
    /// <seealso cref="RulesDescription" />
    public class KgsSgfProperty : RulesDescription
    {
        /// <summary>
        /// Name of the SGF property, in upper case (such as 'MOVE' or 'TIMELEFT').
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// color for one of black, white, or empty.
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// loc for a board location; it will be either the string PASS or an object with x and y values.
        /// </summary>
        [JsonConverter(typeof(LocConverter))]
        public XY Loc { get; set; }
        /// <summary>
        /// loc2 for a second board location; it will be either the string PASS or an object with x and y values.
        /// </summary>
        [JsonConverter(typeof(LocConverter))]
        public XY Loc2 { get; set; }
        public string Text { get; set; }
        public float Float { get; set; }
        public int Int { get; set; }

        public override string ToString()
        {
            return Name + "(" + Loc + "/" + Color + "/" + Text + ")";
        }
    }

    /// <summary>
    /// Because <see cref="KgsSgfProperty.Loc"/> may contain either an object with properties X and Y (such as { x: 2, y: 4 }) or the string "PASS", we use this converter to deserialize it. 
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class LocConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new XY()
                {
                    IsPass = true
                };
            }
            JObject obj = JObject.Load(reader);
            return new XY()
            {
                IsPass = false,
                X = obj.Value<int>("x"),
                Y = obj.Value<int>("y")
            };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(XY);
        }
    }
}