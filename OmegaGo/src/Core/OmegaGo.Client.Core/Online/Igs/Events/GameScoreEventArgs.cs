using System;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;

namespace OmegaGo.Core.Online.Igs.Events
{
    public class GameScoreEventArgs : EventArgs
    {
        public readonly float BlackScore;
        public readonly IgsGame GameInfo;
        public readonly float WhiteScore;

        public GameScoreEventArgs(IgsGame gameInfo, float blackScore, float whiteScore)
        {
            this.GameInfo = gameInfo;
            this.BlackScore = blackScore;
            this.WhiteScore = whiteScore;
        }
    }
}