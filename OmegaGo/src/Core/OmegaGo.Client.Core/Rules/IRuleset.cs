using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public interface IRuleset
    {
        /// <summary>
        /// There are two ways to score. One is based on territory, the other on area.
        /// This method uses the appropriate counting method according to the used ruleset and players' agreement.
        /// </summary>
        /// <param name="currentBoard">The state of board after removing dead stones.</param>
        /// <returns>The score of players.</returns>
        Scores CountScore(GameBoard currentBoard);

        void ModifyScoresAfterLDDeterminationPhase(int deadWhiteStoneCount, int deadBlackStoneCount);

        /// <summary>
        /// Sets the value of Komi. 
        /// If the type of handicap placement is fixed, places handicap stones on the board.
        /// Otherwise, PlaceFreeHandicapStone should be called "handicapStoneCount" times.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of board.</param>
        /// <param name="handicapStoneCount">Number of handicap stones.</param>
        /// <param name="placementType"></param>
        void StartHandicapPlacementPhase(ref GameBoard currentBoard, int handicapStoneCount, HandicapPositions.Type placementType);

        /// <summary>
        /// Places a handicap stone on the board. Verifies the legality of move (occupied position, outside the board).
        /// This method is called, if the ruleset allows free handicap placement.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of board.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <returns>The result of legality check.</returns>
        MoveResult PlaceFreeHandicapStone(ref GameBoard currentBoard, Move moveToMake);

        /// <summary>
        /// Verifies the legality of a move. Places the stone on the board. Finds prisoners and remove them.
        /// </summary>
        /// <param name="previousBoard">The state of board before the move.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of previous game boards.</param>
        /// <returns>Object, which contains: the result of legality check, list of prisoners, the new state of game board.</returns>
        MoveProcessingResult ProcessMove(GameBoard previousBoard, Move moveToMake, List<GameBoard> history);

        /// <summary>
        /// Gets all moves that can be legally made by the PLAYER on the CURRENT BOARD in a game with the specified HISTORY.
        /// </summary>
        /// <param name="player">The player who wants to make a move.</param>
        /// <param name="currentBoard">The current full board position.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>List of legal moves.</returns>
        List<Position> GetAllLegalMoves(StoneColor player, GameBoard currentBoard, List<GameBoard> history);

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="currentBoard">The current full board position.</param>
        /// <param name="moveToMake">The move of a player.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>The result of legality check.</returns>
        MoveResult IsLegalMove(GameBoard currentBoard, Move moveToMake, List<GameBoard> history);

        /// <summary>
        /// Determines all positions that share the color of the specified position. "None" is also a color for the purposes of this method. This method is not thread-safe.
        /// </summary>
        /// <param name="pos">The position whose group we want to identify.</param>
        /// <param name="board">The current full board position.</param>
        /// <returns></returns>
        IEnumerable<Position> DiscoverGroup(Position pos, GameBoard board);

        /// <summary>
        /// Determines which points belong to which player as territory. This is a pure thread-safe method. 
        /// All stones on the board are considered alive for the purposes of determining territory using this method.
        /// </summary>
        /// <param name="board">The current game board.</param>
        Territory[,] DetermineTerritory(GameBoard board);
    }
}
