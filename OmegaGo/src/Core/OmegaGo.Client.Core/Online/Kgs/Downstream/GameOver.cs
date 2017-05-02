using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GameOver : KgsInterruptChannelMessage
    {
        [JsonConverter(typeof(FloatOrStringConverter))]
        public FloatOrString Score { get; set; }
        public override void Process(KgsConnection connection)
        {
            var game = connection.Data.GetGame(ChannelId);
            var controller = game.Controller;
            var players = game.Controller.Players;
            var black = game.Controller.Players.Black;
            var white = game.Controller.Players.White;
            GameEndInformation gameEndInfo = null; 
            if (Score.IsFloat)
            {
                if (Score.Float == 0)
                {
                    gameEndInfo = GameEndInformation.CreateDraw(controller.Players, new Rules.Scores(0, 0));
                }
                else
                {
                   if (Score.Float > 0)
                   {
                       gameEndInfo = GameEndInformation.CreateScoredGame(controller.Players.Black,
                           controller.Players.White, new Rules.Scores(Score.Float, 0));
                   }
                   else
                    {
                        gameEndInfo = GameEndInformation.CreateScoredGame(controller.Players.White,
                            controller.Players.Black, new Rules.Scores(0, -Score.Float));

                    }
                }
            }
            else
            {
                switch (Score.String)
                {
                    case "B+RESIGN":
                    case "B+FORFEIT":
                        gameEndInfo = GameEndInformation.CreateResignation(white, players);
                        break;
                    case "W+RESIGN":
                    case "W+FORFEIT":
                        gameEndInfo = GameEndInformation.CreateResignation(black, players);
                        break;
                    case "B+TIME":
                        gameEndInfo = GameEndInformation.CreateTimeout(white, players);
                        break;
                    case "W+TIME":
                        gameEndInfo = GameEndInformation.CreateTimeout(black, players);
                        break;
                    case "NO_RESULT":
                        gameEndInfo = GameEndInformation.CreateDraw(players, new Rules.Scores(0, 0));
                        break;
                    case "UNKNOWN":
                    case "UNFINISHED":
                    default:
                        gameEndInfo = GameEndInformation.CreateCancellation(players);
                        break;
                }
            }
            controller.KgsConnector.EndTheGame(gameEndInfo);
        }
    }  

    public class FloatOrString
    {
        public float Float { get;  }
        public bool IsFloat { get; }
        public string String { get; }
        public FloatOrString(float f)
        {
            Float = f;
            IsFloat = true;
        }
        public FloatOrString(string s)
        {
            String = s;
            IsFloat = false;
        }
    }
    public class FloatOrStringConverter : JsonConverter
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
                return new FloatOrString((string)reader.Value);
            }
            else if (reader.TokenType == JsonToken.Float)
            {
                return new FloatOrString((float) (double) reader.Value);
            }
            throw new Exception("Unexpected type.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FloatOrString);
        }
    }
}
