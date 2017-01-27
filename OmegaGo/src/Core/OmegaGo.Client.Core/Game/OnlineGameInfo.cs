using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    /// <summary>
    /// Contains metadata about a game that is or was in progress on a server
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Game.GameInfo" />
    public class OnlineGameInfo : GameInfo
    {
        public OnlineGameInfo(
            PlayerInfo whitePlayerInfo,
            PlayerInfo blackPlayerInfo,
            GameBoardSize boardSize,
            RulesetType rulesetType,
            int numberOfHandicapStones,
            HandicapPlacementType handicapPlacementType,
            float komi,
            CountingType countingType,
            int numberOfObservers,
            ServerID server) :
            base(
                whitePlayerInfo,
                blackPlayerInfo,
                boardSize,
                rulesetType,
                numberOfHandicapStones,
                handicapPlacementType,
                komi,
                countingType)
        {
            NumberOfObservers = numberOfObservers;
            this.ServerID = server;
        }

        public ServerID ServerID { get; }
        public IgsConnection Server => Connections.GetConnection(ServerID);
        public int NumberOfObservers { get; set; }

        public override string ToString()
        {
            return $"{White.Name}({White.Rank}) v. {Black.Name}({Black.Rank}) (" + NumberOfObservers + " observers)";
        }
    }
}
