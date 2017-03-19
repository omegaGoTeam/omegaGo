using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// The ruleset contains the basics of Go rules. 
    /// </summary>
    public abstract class Ruleset : IRuleset
    {
        /// <summary>
        /// Initializes the ruleset. For each game, a new ruleset must be created.
        /// </summary>
        /// <param name="gbSize">Size of the game board.</param>
        protected Ruleset(GameBoardSize gbSize)
        {
            GameBoard newBoard = new GameBoard(gbSize);
            GroupState groupState = new GroupState(gbSize);
            RulesetInfo rulesetInfo = new RulesetInfo(gbSize,newBoard,groupState);
        }

        /// <summary>
        /// Factory method that creates a ruleset of given type and gameboard size
        /// </summary>
        /// <param name="ruleset">Ruleset</param>
        /// <param name="gameBoardSize">Gameboard size</param>
        /// <param name="countingType">Counting type (AGA)</param>
        /// <returns>Ruleset</returns>
        public static IRuleset Create(RulesetType ruleset, GameBoardSize gameBoardSize, CountingType countingType = CountingType.Area)
        {
            switch (ruleset)
            {
                case RulesetType.Chinese:
                    {
                        return new ChineseRuleset(gameBoardSize);
                    }
                case RulesetType.Japanese:
                    {
                        return new JapaneseRuleset(gameBoardSize);
                    }
                case RulesetType.AGA:
                    {
                        return new AGARuleset(gameBoardSize, countingType);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(ruleset));
            }
        }

        /// <summary>
        /// Calculates the default compensation (komi)
        /// </summary>
        /// <param name="rsType">Type of the ruleset</param>
        /// <param name="gbSize">Game board size</param>
        /// <param name="handicapStoneCount">Handicap stone count</param>
        /// <param name="cType">Counting type</param>
        /// <returns></returns>
        public static float GetDefaultCompensation(RulesetType rsType, GameBoardSize gbSize, int handicapStoneCount, CountingType cType)
        {
            if (rsType == RulesetType.AGA)
                return AGARuleset.GetAGACompensation(gbSize, handicapStoneCount, cType);
            if (rsType == RulesetType.Chinese)
                return ChineseRuleset.GetChineseCompensation(gbSize, handicapStoneCount);
            if (rsType == RulesetType.Japanese)
                return JapaneseRuleset.GetJapaneseCompensation(gbSize, handicapStoneCount);

            return 0;
        }

        /// <summary>
        /// There are two ways to score. One is based on territory, the other on area.
        /// This method uses the appropriate counting method according to the used ruleset and players' agreement.
        /// </summary>
        /// <param name="currentBoard">The state of board after removing dead stones.</param>
        /// <returns>The score of players.</returns>
        public abstract Scores CountScore(GameBoard currentBoard);

        /// <summary>
        /// Places a handicap stone on the board. Verifies the legality of move (occupied position, outside the board).
        /// This method is called, if the ruleset allows free handicap placement.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of board.</param>
        /// <param name="position">Position to check.</param>
        /// <returns>The result of legality check.</returns>
        public MoveResult PlaceFreeHandicapStone(ref GameBoard currentBoard, Position position)
        {
            if (IsOutsideTheBoard(position) == MoveResult.OutsideTheBoard)
                return MoveResult.OutsideTheBoard;
            if (IsPositionOccupied(position) == MoveResult.OccupiedPosition)
                return MoveResult.OccupiedPosition;

            currentBoard[position.X, position.Y] = StoneColor.Black;
            return MoveResult.Legal;
        }

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="moveToMake">The move of a player.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>The result of legality check.</returns>
        public MoveResult IsLegalMove(Move moveToMake, GameBoard[] history)
        {
            MoveProcessingResult result = ProcessMove(moveToMake, history);
            return result.Result;
        }

        /// <summary>
        /// Gets all moves that can be legally made by the PLAYER on the board in a game with the specified HISTORY.
        /// </summary>
        /// <param name="player">The player who wants to make a move.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>List of legal moves.</returns>
        public MoveResult[,] GetAllLegalMoves(StoneColor player, GameBoard[] history)
        {
            MoveResult[,] moveResults = new MoveResult[RulesetInfo.BoardSize.Width, RulesetInfo.BoardSize.Height];
            //TODO Aniko: set RulesetInfo.BoardState
            for (int x = 0; x < RulesetInfo.BoardSize.Width; x++)
                for (int y = 0; y < RulesetInfo.BoardSize.Height; y++)
                {
                    Move move = Move.PlaceStone(player, new Position(x, y));
                    if (IsPositionOccupied(new Position(x,y)) == MoveResult.OccupiedPosition)
                    {
                        moveResults[x, y] = MoveResult.OccupiedPosition;
                    }
                    else
                    {
                        //2. step: add stone
                        RulesetInfo.BoardState[x, y] = player;
                        //3. step: find captures and remove prisoners
                        //TODO Aniko: set RulesetInfo
                        List<Position> captures = CheckCapture(move);
                        foreach(Position p in captures)
                        {
                            RulesetInfo.BoardState[p.X, p.Y] = StoneColor.None;
                        }
                        //4. step: check selfcapture, ko
                        moveResults[x, y] = CheckSelfCaptureKo(move, history);
                    }
                }

            return moveResults;
        }

        protected MoveResult CheckSelfCaptureKo(Move moveToMake, GameBoard[] history)
        {
            if (IsSelfCapture(moveToMake) == MoveResult.SelfCapture)
            {
                return MoveResult.SelfCapture;
            }
            else if (IsKo(moveToMake, history) == MoveResult.Ko)
            {
                return MoveResult.Ko;
            }
            else
            {
                return MoveResult.Legal;
            }
        }

        /// <summary>
        /// Verifies the legality of a move. Places the stone on the board. Finds prisoners and remove them.
        /// </summary>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of previous game boards.</param>
        /// <returns>Object, which contains: the result of legality check, list of prisoners, the new state of game board.</returns>
        public MoveProcessingResult ProcessMove(Move moveToMake, GameBoard[] history)
        {
            GameBoard previousBoard = new GameBoard(history.Last());
            GameBoard currentBoard = new GameBoard(previousBoard);
            Position position = moveToMake.Coordinates;
            StoneColor player = moveToMake.WhoMoves;

            MoveProcessingResult processingResult = new MoveProcessingResult
            {
                Captures = new List<Position>(),
                NewBoard = previousBoard
            };

            //1. step: check intersection
            if (moveToMake.Kind == MoveKind.Pass)
            {
                processingResult.Result = Pass(player);
                return processingResult;
            }
            else if (IsOutsideTheBoard(position) == MoveResult.OutsideTheBoard)
            {
                processingResult.Result = MoveResult.OutsideTheBoard;
                return processingResult;
            }
            else if (IsPositionOccupied(position) == MoveResult.OccupiedPosition)
            {
                processingResult.Result = MoveResult.OccupiedPosition;
                return processingResult;
            }
            else
            {
                //2. step: add stone
                currentBoard[moveToMake.Coordinates.X, moveToMake.Coordinates.Y] = player;
                //3. step: find captures and remove prisoners
                //TODO Aniko: set RulesetInfo
                processingResult.Captures = CheckCapture(moveToMake);
                foreach (Position p in processingResult.Captures)
                {
                    currentBoard[p.X, p.Y] = StoneColor.None;
                }
                //4. step: check selfcapture, ko, superko
                MoveResult r = CheckSelfCaptureKoSuperko(moveToMake, history);
                if (r == MoveResult.Legal)
                    processingResult.NewBoard = currentBoard;
                else
                    processingResult.NewBoard = previousBoard;

                processingResult.Result = r;
                return processingResult;
            }
        }

        /// <summary>
        /// Determines which points belong to which player as territory. This is a pure thread-safe method. 
        /// All stones on the board are considered alive for the purposes of determining territory using this method.
        /// </summary>
        /// <param name="board">The current game board.</param>
        public Territory[,] DetermineTerritory(GameBoard board)
        {
            Territory[,] regions = new Territory[RulesetInfo.BoardSize.Width, RulesetInfo.BoardSize.Height];
            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
            {
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    regions[i, j] = Territory.Unknown;
                }
            }

            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
            {
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    if (regions[i, j] == Territory.Unknown && board[i, j] == StoneColor.None)
                    {
                        HashSet<Position> region = new HashSet<Position>();
                        Territory regionBelongsTo = Territory.Unknown;
                        Position p = new Position();
                        p.X = i;
                        p.Y = j;
                        GetRegion(ref region, ref regionBelongsTo, p);

                        foreach (Position regionMember in region)
                        {
                            regions[regionMember.X, regionMember.Y] = regionBelongsTo;
                        }

                    }
                }
            }
            return regions;
        }

        protected abstract MoveResult CheckSelfCaptureKoSuperko(Move moveToMake, GameBoard[] history);

        /// <summary>
        /// Handles the pass of a player. Two consecutive passes signal the end of game.
        /// </summary>
        /// <param name="playerColor">Color of player, who passes.</param>
        /// <returns>The legality of move or new game phase notification.</returns>
        protected abstract MoveResult Pass(StoneColor playerColor);

        /// <summary>
        /// Checks whether the player places a stone on the intersection where the opponent has just captured a stone.
        /// </summary>
        /// <param name="currentBoard">The state of game board.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of game boards that represents the history of game.</param>
        /// <returns>The result of legality check.</returns>
        protected MoveResult IsKo(Move moveToMake, GameBoard[] history)
        {    
            int boardHistoryCount = history.Length;
            if (boardHistoryCount >= 2 && history.ElementAt(boardHistoryCount - 2).Equals(RulesetInfo.BoardState))
                return MoveResult.Ko;

            return MoveResult.Legal;
        }

        /// <summary>
        /// Determines whether the move causes a full board position to repeat again in the same game.
        /// </summary>
        /// <param name="currentBoard">The state of game board.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of game boards that represents the history of game.</param>
        /// <returns>The result of legality check.</returns>
        protected MoveResult IsSuperKo(Move moveToMake, GameBoard[] history)
        {
            for (int i = 0; i < history.Length; i++)
            {
                if (history.ElementAt(i).Equals(RulesetInfo.BoardState))
                    return MoveResult.SuperKo;
            }
            return MoveResult.Legal;
        }

        /// <summary>
        /// Determines whether the intersection is already occupied by another stone.
        /// </summary>
        /// <param name="currentBoard">The state of game board.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <returns>The result of legality check.</returns>
        protected MoveResult IsPositionOccupied(Position p)
        {
            if (RulesetInfo.BoardState[p.X, p.Y] != StoneColor.None)
            {
                return MoveResult.OccupiedPosition;
            }
            else
            {
                return MoveResult.Legal;
            }

        }

        /// <summary>
        /// Determines whether the move is suicidal.
        /// </summary>
        /// <param name="currentBoard">The state of game board.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <returns>The result of legality check.</returns>
        protected MoveResult IsSelfCapture(Move moveToMake)
        {
            Position p = moveToMake.Coordinates;
            List<Position> group = new List<Position>();
            bool groupHasLiberty = false;
            //TODO Aniko: implement
            /*_checkedInters = new bool[_boardWidth, _boardHeight];

            currentBoard[p.X, p.Y] = moveToMake.WhoMoves;
            _liberty = FillLibertyTable(currentBoard);
            GetGroup(ref group, ref groupHasLiberty, p, currentBoard);*/
            if (groupHasLiberty)
            {
                return MoveResult.Legal;
            }
            else
            {
                return MoveResult.SelfCapture;
            }
        }

        /// <summary>
        /// Determines whether the player places a stone outside the game board.
        /// </summary>
        /// <param name="p">Position to place stone.</param>
        /// <returns>The result of legality check.</returns>
        protected MoveResult IsOutsideTheBoard(Position p)
        {
            if (p.X < 0 || p.X >= RulesetInfo.BoardSize.Width || p.Y < 0 || p.Y >= RulesetInfo.BoardSize.Height)
                return MoveResult.OutsideTheBoard;
            else
                return MoveResult.Legal;
        }

        /// <summary>
        /// Finds the prisoners of opponent player.
        /// </summary>
        /// <param name="currentBoard">The state of game board after the player's move.</param>
        /// <param name="moveToMake">The player's move</param>
        /// <returns>List of prisoners/captured stones.</returns>
        protected List<Position> CheckCapture(Move moveToMake)
        {
            //TODO Aniko: implement
           
            /*_liberty = FillLibertyTable(currentBoard);
            _checkedInters = new bool[_boardWidth, _boardHeight];
            int currentX = moveToMake.Coordinates.X;
            int currentY = moveToMake.Coordinates.Y;
            StoneColor opponentColor = moveToMake.WhoMoves.GetOpponentColor();

            //check whether neighbour groups have liberty
            //right neighbour
            if (currentX < _boardWidth - 1 && currentBoard[currentX + 1, currentY] == opponentColor && !_liberty[currentX + 1, currentY])
                CheckNeighbourGroup(currentX + 1, currentY, currentBoard);

            //left neighbour
            if (currentX > 0 && currentBoard[currentX - 1, currentY] == opponentColor && !_liberty[currentX - 1, currentY])
                CheckNeighbourGroup(currentX - 1, currentY, currentBoard);

            //upper neighbour
            if (currentY < _boardHeight - 1 && currentBoard[currentX, currentY + 1] == opponentColor && !_liberty[currentX, currentY + 1])
                CheckNeighbourGroup(currentX, currentY + 1, currentBoard);

            //bottom neighbour
            if (currentY > 0 && currentBoard[currentX, currentY - 1] == opponentColor && !_liberty[currentX, currentY - 1])
                CheckNeighbourGroup(currentX, currentY - 1, currentBoard);
                */
            return new List<Position>();
        }
        
        /// <summary>
        /// Checks the liberty of surrounding groups.
        /// </summary>
        /// <param name="x">Letter-based coordinate of position.</param>
        /// <param name="y">Number-based coordinate of position.</param>
        /// <param name="currentBoard">The state of game board after the player's move.</param>
        protected void CheckNeighbourGroup(int x, int y)
        {
            List<Position> group = new List<Position>();
            bool groupHasLiberty = false;
            Position p = new Position();

            //TODO Aniko: implement
            /*p.X = x;
            p.Y = y;
            GetGroup(ref group, ref groupHasLiberty, p, currentBoard);

            //if group has liberty, setup true liberty for all; else remove the group from the board
            for (int k = 0; k < group.Count; k++)
            {
                Position groupMember = group.ElementAt(k);
                if (groupHasLiberty)
                {
                    _liberty[groupMember.X, groupMember.Y] = true;
                }
                else
                {
                    _captures.Add(groupMember);
                }
            }*/

        }

        /// <summary>
        /// The area of a player are all live stones of player left on the board together with any points of his territory. In this case, prisoners are ignored.
        /// This method adds up total area of players.
        /// </summary>
        /// <param name="currentBoard">The state of board after removing dead stones.</param>
        /// <returns>The score of players.</returns>
        protected Scores CountArea()
        {
            Scores scores = CountTerritory();
            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
            {
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    if (RulesetInfo.BoardState[i, j] == StoneColor.Black)
                        scores.BlackScore++;
                    else if (RulesetInfo.BoardState[i, j] == StoneColor.White)
                        scores.WhiteScore++;
                }
            }

            return scores;

        }

        /// <summary>
        /// The territory of a player are those empty points on the board which are entirely surrounded by his live stones. 
        /// This method adds up total territory of players. The scores include prisoners and dead stones. 
        /// </summary>
        /// <param name="currentBoard">The state of board after removing dead stones.</param>
        /// <returns>The score of players, which includes number of prisoners and dead stones yet.</returns>
        protected Scores CountTerritory()
        {
            Scores scores = new Scores();
            scores.WhiteScore = 0;
            scores.BlackScore = 0;

            Territory[,] regions = DetermineTerritory(RulesetInfo.BoardState);

            for (int i = 0; i < RulesetInfo.BoardSize.Width; i++)
            {
                for (int j = 0; j < RulesetInfo.BoardSize.Height; j++)
                {
                    if (regions[i, j] == Territory.Black)
                        scores.BlackScore++;
                    else if (regions[i, j] == Territory.White)
                        scores.WhiteScore++;
                }
            }

            return scores;
        }

        private void GetRegion(
            ref HashSet<Position> region,
            ref Territory regionBelongsTo, 
            Position pos)
        {
            if (region.Contains(pos)) return;

            region.Add(pos);

            Position newp = new Position();

            if (pos.X < RulesetInfo.BoardSize.Width - 1) //has right neighbour
            {
                newp.X = pos.X + 1;
                newp.Y = pos.Y;

                switch (RulesetInfo.BoardState[pos.X + 1, pos.Y])
                {
                    case StoneColor.None:
                        GetRegion(ref region, ref regionBelongsTo, newp);
                        break;
                    case StoneColor.Black:
                        if (regionBelongsTo == Territory.White)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.Black;
                        break;
                    case StoneColor.White:
                        if (regionBelongsTo == Territory.Black)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.White;
                        break;
                    default:
                        break;
                }

            }
            if (pos.Y < RulesetInfo.BoardSize.Height - 1) //has upper neighbour
            {
                newp.X = pos.X;
                newp.Y = pos.Y + 1;
                switch (RulesetInfo.BoardState[pos.X, pos.Y + 1])
                {
                    case StoneColor.None:
                        GetRegion(ref region, ref regionBelongsTo, newp);
                        break;
                    case StoneColor.Black:
                        if (regionBelongsTo == Territory.White)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.Black;
                        break;
                    case StoneColor.White:
                        if (regionBelongsTo == Territory.Black)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.White;
                        break;
                    default:
                        break;
                }
            }
            if (pos.X > 0) //has left neighbour
            {
                newp.X = pos.X - 1;
                newp.Y = pos.Y;
                switch (RulesetInfo.BoardState[pos.X - 1, pos.Y])
                {
                    case StoneColor.Black:
                        if (regionBelongsTo == Territory.White)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.Black;
                        break;
                    case StoneColor.White:
                        if (regionBelongsTo == Territory.Black)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.White;
                        break;
                    case StoneColor.None:
                        GetRegion(ref region, ref regionBelongsTo, newp);
                        break;
                    default:
                        break;
                }
            }
            if (pos.Y > 0) //has bottom neighbour
            {
                newp.X = pos.X;
                newp.Y = pos.Y - 1;
                switch (RulesetInfo.BoardState[pos.X, pos.Y - 1])
                {
                    case StoneColor.Black:
                        if (regionBelongsTo == Territory.White)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.Black;
                        break;
                    case StoneColor.White:
                        if (regionBelongsTo == Territory.Black)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.White;
                        break;
                    case StoneColor.None:
                        GetRegion(ref region, ref regionBelongsTo, newp);
                        break;
                    default:
                        break;
                }
            }

        }

    }
}
