using System;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Online.Igs;

namespace OmegaGo.Core.Online.Igs
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