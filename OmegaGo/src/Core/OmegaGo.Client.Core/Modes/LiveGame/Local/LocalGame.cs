﻿using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    /// <summary>
    /// Represents a local game
    /// </summary>
    public class LocalGame : GameBase
    {        
        public LocalGame(GameInfo info, IRuleset ruleset, PlayerPair players) : base(info)
        {
            Controller = new GameController(info, ruleset, players);
        }

        public sealed override IGameController Controller { get; }
    }
}
