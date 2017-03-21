using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;

namespace OmegaGo.Core.Rules
{
    public interface IRuleset
    {

        /// <summary>
        /// Places a handicap stone on the board. Verifies the legality of move (occupied position, outside the board).
        /// This method is called, if the ruleset allows free handicap placement.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of board.</param>
        /// <param name="position">Position to check.</param>
        /// <returns>The result of legality check.</returns>
        MoveResult PlaceFreeHandicapStone(ref GameBoard currentBoard, Position position);

        void SetRulesetInfo(GameBoard boardState, GroupState groupState);

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="moveToMake">The move of a player.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>The result of legality check.</returns>
        MoveResult IsLegalMove(GameTreeNode currentNode, Move moveToMake);

        /// <summary>
        /// Gets all moves that can be legally made by the PLAYER on the board in a game with the specified HISTORY.
        /// </summary>
        /// <param name="player">The player who wants to make a move.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>Map of move result.</returns>
        MoveResult[,] GetAllLegalMoves(GameTreeNode currentNode);

        /// <summary>
        /// Verifies the legality of a move. Places the stone on the board. Finds prisoners and remove them.
        /// </summary>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of previous game boards.</param>
        /// <returns>Object, which contains: the result of legality check, list of prisoners, the new state of game board.</returns>
        MoveProcessingResult ProcessMove(GameTreeNode currentNode, Move moveToMake);
        
        /// <summary>
        /// There are two ways to score. One is based on territory, the other on area.
        /// This method uses the appropriate counting method according to the used ruleset and players' agreement.
        /// </summary>
        /// <param name="currentBoard">The state of board after removing dead stones.</param>
        /// <returns>The score of players.</returns>
        Scores CountScore(GameBoard currentBoard);
        
        /// <summary>
        /// Determines which points belong to which player as territory. This is a pure thread-safe method. 
        /// All stones on the board are considered alive for the purposes of determining territory using this method.
        /// </summary>
        /// <param name="board">The current game board.</param>
        Territory[,] DetermineTerritory(GameBoard board);
    }
}
