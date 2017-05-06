using System;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.State
{
    /// <summary>
    /// Represents complete information about the end of a game
    /// </summary>
    public class GameEndInformation
    {
        /// <summary>
        /// Color of the winner
        /// </summary>
        private readonly StoneColor _winnerColor;

        /// <summary>
        /// Creates game end information
        /// </summary>
        /// <param name="reason">Reason why the game has ended</param>
        /// <param name="players">The player pair of the game controller.</param>
        /// <param name="winnerColor">The player who has won the game, or None if there is no winner.</param>
        private GameEndInformation(GameEndReason reason, PlayerPair players, StoneColor winnerColor = StoneColor.None)
        {
            Reason = reason;
            Players = players;
            _winnerColor = winnerColor;
        }

        /// <summary>
        /// Reason why the game has ended
        /// </summary>
        public GameEndReason Reason { get; }

        /// <summary>
        /// Gets the game's players
        /// </summary>
        private PlayerPair Players { get; }

        /// <summary>
        /// Gets the loser (or null if no loser)
        /// </summary>
        public GamePlayer Loser =>
            HasWinnerAndLoser ?
                Players.GetOpponentOf(Winner) :
                null;

        /// <summary>
        /// Gets the winner (or null if no winner)
        /// </summary>
        public GamePlayer Winner =>
            HasWinnerAndLoser ?
                Players[_winnerColor] :
                null;

        /// <summary>
        /// Indicates if there is a winner
        /// </summary>
        public bool HasWinnerAndLoser => _winnerColor != StoneColor.None;

        /// <summary>
        /// Player scores
        /// </summary>
        public Scores Scores { get; private set; }

        /// <summary>
        /// Creates a timeout
        /// </summary>
        /// <param name="whoTimedOut">Who timed out</param>
        /// <param name="players">Player pair</param>
        /// <returns></returns>
        public static GameEndInformation CreateTimeout(GamePlayer whoTimedOut, PlayerPair players) =>
            new GameEndInformation(GameEndReason.Timeout, players, whoTimedOut.Info.Color.GetOpponentColor());

        /// <summary>
        /// Creates a resignation
        /// </summary>
        /// <param name="whoResigned"></param>
        /// <param name="players"></param>
        /// <returns>Game end info</returns>
        public static GameEndInformation CreateResignation(GamePlayer whoResigned, PlayerPair players) =>
            new GameEndInformation(GameEndReason.Resignation, players, whoResigned.Info.Color.GetOpponentColor());

        /// <summary>
        /// Creates game cancellation
        /// </summary>
        /// <param name="players">Both players</param>
        /// <returns>Game end info</returns>
        public static GameEndInformation CreateCancellation(PlayerPair players) =>
            new GameEndInformation(GameEndReason.Cancellation, players);

        /// <summary>
        /// Creates disconnection
        /// </summary>
        /// <param name="whoDisconnected">Who has disconnected. In observed games, this may be null.</param>
        /// <param name="players">Both players</param>
        /// <returns>Game end info</returns>
        public static GameEndInformation CreateDisconnection(GamePlayer whoDisconnected, PlayerPair players) =>
            new GameEndInformation(GameEndReason.Disconnection, players, whoDisconnected?.Info.Color.GetOpponentColor() ?? StoneColor.None);

        /// <summary>
        /// Creates scored game end
        /// </summary>
        public static GameEndInformation CreateDraw(PlayerPair players, Scores scores) =>
             new GameEndInformation(GameEndReason.ScoringComplete, players )
             {
                 Scores = scores
             };

        /// <summary>
        /// Creates scored game end
        /// </summary>
        public static GameEndInformation CreateScoredGame(GamePlayer winner, GamePlayer loser, Scores scores) =>
            new GameEndInformation(GameEndReason.ScoringComplete, new PlayerPair(winner, loser), winner.Info.Color)
            {
                Scores = scores
            };

        /// <summary>
        /// Converts the game end info to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Reason)
            {
                case GameEndReason.Cancellation:
                    return "Game cancelled.";
                case GameEndReason.Disconnection:
                    return "Disconnected from server.";
                case GameEndReason.Resignation:
                    return Loser + " resigned.";
                case GameEndReason.Timeout:
                    return Loser + " timed out.";
                case GameEndReason.ScoringComplete:
                    if (HasWinnerAndLoser)
                    {
                        return (Winner.Info.Color == StoneColor.Black ? "B" : "W") + "+" +
                               Scores.AbsoluteScoreDifference;
                    }
                    else
                    {
                        return "Draw.";
                    }
                default:
                    return "Unknown reason";
            }
        }
    }
}
