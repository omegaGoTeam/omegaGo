using System;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Online.Igs
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