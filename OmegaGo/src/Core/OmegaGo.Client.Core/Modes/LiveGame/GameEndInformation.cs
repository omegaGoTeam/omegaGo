using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    public class GameEndInformation
    {
        private GameEndInformation(GameEndReason reason, bool hasWinnerAndLoser, GamePlayer winner, GamePlayer loser)
        {
            this.Reason = reason;
            this.Winner = winner;
            this.Loser = loser;
            this.HasWinnerAndLoser = hasWinnerAndLoser;
        }

        GameEndReason Reason { get; }
        public GamePlayer Loser { get; }
        public GamePlayer Winner { get; }
        public bool HasWinnerAndLoser { get; }
        public float WinnerScoreDifference { private set; get; }

        public static GameEndInformation Timeout(GamePlayer whoTimedOut, IGameController controller)
        {
            return new LiveGame.GameEndInformation(GameEndReason.Timeout,
                true,
                controller.Players.GetOpponentOf(whoTimedOut),
                whoTimedOut);
        }
        public static GameEndInformation Resignation(GamePlayer whoResigned, IGameController controller)
        {
            return new LiveGame.GameEndInformation(GameEndReason.Resignation,
                true,
                controller.Players.GetOpponentOf(whoResigned),
                whoResigned);
        }
        public static GameEndInformation Cancellation()
        {
            return new LiveGame.GameEndInformation(GameEndReason.Cancellation,
                false,
                null,
                null);
        }
        public static GameEndInformation Disconnection(GamePlayer whoDisconnected, IGameController controller)
        {
            return new LiveGame.GameEndInformation(GameEndReason.Disconnection, true,
                controller.Players.GetOpponentOf(whoDisconnected),
                whoDisconnected);
        }
        public static GameEndInformation ScoringComplete(bool isDraw, GamePlayer winner, GamePlayer loser, float winnerScoreDifference)
        {
            return new LiveGame.GameEndInformation(GameEndReason.ScoringComplete, !isDraw, winner, loser)
            {
                WinnerScoreDifference = winnerScoreDifference
            };
        }
    }
}
