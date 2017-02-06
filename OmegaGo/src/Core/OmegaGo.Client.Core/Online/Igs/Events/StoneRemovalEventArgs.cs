using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;

namespace OmegaGo.Core.Online.Igs.Events
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