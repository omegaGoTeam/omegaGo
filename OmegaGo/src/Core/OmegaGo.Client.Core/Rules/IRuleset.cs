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
        /// Initializes the ruleset. For each game, a new ruleset must be created.
        /// </summary>
        /// <param name="white"></param>
        /// <param name="black"></param>
        /// <param name="gbSize">Size of the game board.</param>
        Scores CountScore(List<Position> deadStones, GameBoard currentBoard);

        /// <summary>
        /// Sets the value of Komi. 
        /// If the type of handicap placement is fixed, places handicap stones on the board.
        /// Otherwise, PlaceFreeHandicapStone should be called handicapStoneNumber times.
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="handicapStoneNumber"></param>
        /// <param name="placementType"></param>
        void ModifyScoresAfterLDConfirmationPhase(int deadWhiteStoneCount, int deadBlackStoneCount);

        /// <summary>
        /// Places a handicap stone on the board. Verifies the legality of move (occupied position, outside the board).
        /// This method is called, if the ruleset allows free handicap placement.
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="moveToMake"></param>
        /// <returns></returns>
        void StartHandicapPhase(ref GameBoard currentBoard, int handicapStoneNumber, HandicapPositions.Type placementType);

        /// <summary>
        /// Places a handicap stone on the board. Verifies the legality of move (occupied position, outside the board).
        /// This method is called, if the ruleset allows free handicap placement.
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="moveToMake"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        List<Position> GetAllLegalMoves(StoneColor player, GameBoard currentBoard, List<GameBoard> history);

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="moveToMake"></param>
        /// <param name="history"></param>
        MoveResult IsLegalMove(GameBoard currentBoard, Move moveToMake, List<GameBoard> history);
    }
}
