﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Players.Igs;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class GameJoin : GameState
    {
        public User[] Users { get; set; }
        public GameSummary GameSummary { get; set; }
        public SgfEvent[] SgfEvents { get; set; }
        public override void Process(KgsConnection connection)
        {
            KgsGameInfo info = KgsGameInfo.FromGameJoin(this);
            var blackPlayer = new KgsPlayerBuilder(Game.StoneColor.Black, connection)
                .Name(info.Black.Name)
                .Rank(info.Black.Rank)
                .Build();
            var whitePlayer = new KgsPlayerBuilder(Game.StoneColor.White, connection)
                .Name(info.White.Name)
                .Rank(info.White.Rank)
                .Build();
            var ongame = new KgsGameBuilder(info)
                .BlackPlayer(blackPlayer)
                .WhitePlayer(whitePlayer)
                .Build();
            connection.Data.JoinGame(ongame);
            foreach (var ev in SgfEvents)
            {
                ev.ExecuteAsIncoming(connection, ongame);
            }
            connection.Events.RaiseGameJoined(ongame);
        }
    }
}
