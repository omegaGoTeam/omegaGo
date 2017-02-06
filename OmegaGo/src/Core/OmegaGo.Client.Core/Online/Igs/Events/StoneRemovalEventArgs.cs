using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;

namespace OmegaGo.Core.Online.Igs
{
    public class StoneRemovalEventArgs : EventArgs
    {
        public IgsGame Game { get;  }
        public Position DeadPosition { get;  }

        public StoneRemovalEventArgs(IgsGame game, Position deadPosition)
        {
            this.Game = game;
            this.DeadPosition = deadPosition;
        }
    }
}