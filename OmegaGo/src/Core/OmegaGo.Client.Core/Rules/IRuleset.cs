using System.Collections.Generic;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    public interface IRuleset
    {
        /// <summary>
        /// Contains information about board state and group state.
        /// </summary>
        IRulesetInfo RulesetInfo { get; }

        /// <summary>
        /// There are two ways to score. One is based on territory, the other on area.
        /// This method uses the appropriate counting method according to the used ruleset and players' agreement.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="deadPositions">List of dead stones.</param>
        /// <param name="komi">Komi compensation.</param>
        /// <returns>The score of players.</returns>
        Scores CountScore(GameTreeNode currentNode, IEnumerable<Position> deadPositions, float komi);

        /// <summary>
        /// Places a handicap stone on the board. Verifies the legality of move (occupied position, outside the board).
        /// This method is called, if the ruleset allows free handicap placement.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of board.</param>
        /// <param name="position">Position to check.</param>
        /// <returns>The result of legality check.</returns>
        MoveResult PlaceFreeHandicapStone(ref GameBoard currentBoard, Position position);

        /// <summary>
        /// Sets the state of ruleset.
        /// </summary>
        /// <param name="boardState">State of board to use.</param>
        /// <param name="groupState">State of groups to use.</param>
        void SetRulesetInfo(GameBoard boardState, GroupState groupState);

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="moveToMake">The move of a player.</param>
        /// <returns>The result of legality check.</returns>
        MoveResult IsLegalMove(GameTreeNode currentNode, Move moveToMake);

        /// <summary>
        /// Verifies the legality of a move. Places the stone on the board. Finds prisoners and remove them.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <returns>Object, which contains: the result of legality check, list of prisoners, the new state of game board, the new state of groups.</returns>
        MoveProcessingResult ProcessMove(GameTreeNode currentNode, Move moveToMake);

        /// <summary>
        /// Gets the results of moves.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <returns>Map of move results.</returns>
        MoveResult[,] GetMoveResult(GameTreeNode currentNode);

        /// <summary>
        /// Gets the results of moves. Checks whether the intersection is occupied.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <returns>Map of move results.</returns>
        MoveResult[,] GetMoveResultLite(GameTreeNode boardState);

        /// <summary>
        /// Determines which points belong to which player as territory. This is a pure thread-safe method. 
        /// All stones on the board are considered alive for the purposes of determining territory using this method.
        /// </summary>
        /// <param name="board">The current game board.</param>
        /// <returns>Map of territories.</returns>
        Territory[,] DetermineTerritory(GameBoard board);

        
    }
}
