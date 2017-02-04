using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class KgsSgfProperty : RulesDescription
    {
        public string Name { get; set; }
        public string Color { get; set; }
        [JsonConverter(typeof(LocConverter))]
        public XY Loc { get; set; }
        [JsonConverter(typeof(LocConverter))]
        public XY Loc2 { get; set; }
        public string Text { get; set; }
        public float Float { get; set; }
        public int Int { get; set; }
    }

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