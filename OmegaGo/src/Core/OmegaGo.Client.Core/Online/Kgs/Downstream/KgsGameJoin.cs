﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players.Builders;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    public class KgsGameJoin : KgsGameState
    {
        public User[] Users { get; set; }
        public GameSummary GameSummary { get; set; }
        public SgfEvent[] SgfEvents { get; set; }

        public RulesDescription Rules
        {
            get
            {
                foreach (var ev in SgfEvents)
                {
                    if (ev.Props != null)
                    {
                        foreach (var prop in ev.Props)
                        {
                            if (prop.Name == "RULES")
                            {
                                return prop;
                            }
                        }
                    }
                }
                throw new Exception("Rules were not found in the GameJoin message.");
            }
        }
        public override void Process(KgsConnection connection)
        {
            // TODO Petr KGS OVERHAUL
            // TODO Petr : handle bad types
            KgsGameInfo info = KgsGameInfo.FromGameJoin(this, connection);
            if (info == null) return; // TODO Petr : warn the user that joining failed


            var blackPlayer = new KgsPlayerBuilder(Game.StoneColor.Black, connection)
                .Name(info.Black.Name)
                .Rank(info.Black.Rank)
                .Build();
            if (info.Black.Name == connection.Username)
            {
                blackPlayer =
                    new HumanPlayerBuilder(Game.StoneColor.Black).Name(info.Black.Name).Rank(info.Black.Rank).Build();
            }
            var whitePlayer = new KgsPlayerBuilder(Game.StoneColor.White, connection)
                .Name(info.White.Name)
                .Rank(info.White.Rank)
                .Build();
            if (info.White.Name == connection.Username)
            {
                whitePlayer =
                    new HumanPlayerBuilder(Game.StoneColor.White).Name(info.White.Name).Rank(info.White.Rank).Build();
            }
            var ongame = new KgsGameBuilder(info, connection)
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
