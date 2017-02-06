using System;
using OmegaGo.Core.Modes.LiveGame.Online.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Online.Igs.Events
{
    public class GamePlayerEventArgs : EventArgs
    {
        public IgsGame Game { get; }
        public GamePlayer Player { get; }

        public GamePlayerEventArgs(IgsGame game, GamePlayer player)
        {
            this.Game = game;
            this.Player = player;
        }
    }
}