using System;
using System.Collections.Generic;
using OmegaGo.Core.Modes.LiveGame.Connectors.Kgs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Kgs
{
    public class KgsGame : RemoteGame<KgsGameInfo, KgsGameController>
    {
        private readonly KgsConnection _connection;

        public KgsGame(KgsGameInfo info, IRuleset ruleset, PlayerPair players, KgsConnection connection) : base(info)
        {
            this._connection = connection;
            Controller = new KgsGameController(info, ruleset, players, connection);
            Controller.Nodes.Add(0, new KgsSgfNode(0));
        }

        public override KgsGameController Controller { get; }

        public void GetChatMessage(Tuple<string, string> tuple)
        {
            var chatMessage = new ChatMessage(tuple.Item1, tuple.Item2,
                DateTimeOffset.Now, ChatMessageKind.Incoming);
            Controller.OnChatMessageReceived(chatMessage);
            Controller.MessageLog.Add(chatMessage);
        }
    }
}
