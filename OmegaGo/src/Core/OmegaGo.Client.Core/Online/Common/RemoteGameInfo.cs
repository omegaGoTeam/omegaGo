using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Common
{
    /// <summary>
    /// A common base class for metadata of a game that runs on an online server.
    /// </summary>
    public abstract class RemoteGameInfo : GameInfo
    {
        protected RemoteGameInfo(PlayerInfo whitePlayerInfo, PlayerInfo blackPlayerInfo, GameBoardSize boardSize, RulesetType rulesetType, int numberOfHandicapStones, HandicapPlacementType handicapPlacementType, float komi, CountingType countingType) : base(whitePlayerInfo, blackPlayerInfo, boardSize, rulesetType, numberOfHandicapStones, handicapPlacementType, komi, countingType)
        {
        }

        /// <summary>
        /// Gets or sets the number of moves that have been made prior to the opening of this game by this client.
        /// Sound effects will be suppressed for moves up to this number, otherwise we would have a cacophony of sounds
        /// as the game is opened. 
        /// </summary>
        public int PreplayedMoveCount { get; set; } = 0;
    }
}
