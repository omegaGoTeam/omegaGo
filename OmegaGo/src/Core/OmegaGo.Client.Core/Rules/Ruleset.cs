﻿using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// The ruleset contains the basics of Go rules. 
    /// </summary>
    public abstract class Ruleset : IRuleset
    {
        private int _boardWidth, _boardHeight;
        private bool[,] _checkedInters;
        private bool[,] _liberty;
        private List<Position> _captures;

        /// <summary>
        /// Initializes the ruleset. For each game, a new ruleset must be created.
        /// </summary>
        /// <param name="gbSize">Size of the game board.</param>
        protected Ruleset(GameBoardSize gbSize)
        {
            _boardWidth = gbSize.Width;
            _boardHeight = gbSize.Height;
            _checkedInters = new bool[_boardWidth, _boardHeight];
            _liberty = new bool[_boardWidth, _boardHeight];
            _captures = new List<Position>();
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

        public abstract void ModifyScoresAfterLDDeterminationPhase(int deadWhiteStoneCount, int deadBlackStoneCount);

        /// <summary>
        /// Sets the value of Komi. 
        /// If the type of handicap placement is fixed, places handicap stones on the board.
        /// Otherwise, PlaceFreeHandicapStone should be called "handicapStoneCount" times.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of board.</param>
        /// <param name="handicapStoneCount">Number of handicap stones.</param>
        /// <param name="placementType"></param>
        public void StartHandicapPlacementPhase(ref GameBoard currentBoard, int handicapStoneCount, HandicapPlacementType placementType)
        {
            if (handicapStoneCount == 0)
            {
                SetKomi(handicapStoneCount);
                return;
            }

            if (placementType == HandicapPlacementType.Fixed)
                PlaceFixedHandicapStones(ref currentBoard, handicapStoneCount);

            SetKomi(handicapStoneCount);
        }

        /// <summary>
        /// Places a handicap stone on the board. Verifies the legality of move (occupied position, outside the board).
        /// This method is called, if the ruleset allows free handicap placement.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of board.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <returns>The result of legality check.</returns>
        public MoveResult PlaceFreeHandicapStone(ref GameBoard currentBoard, Move moveToMake)
        {
            Position position = moveToMake.Coordinates;
            if (IsOutsideTheBoard(position) == MoveResult.OutsideTheBoard)
                return MoveResult.OutsideTheBoard;
            if (IsPositionOccupied(currentBoard, moveToMake) == MoveResult.OccupiedPosition)
                return MoveResult.OccupiedPosition;

            currentBoard[position.X, position.Y] = StoneColor.Black;
            return MoveResult.Legal;

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
        /// Verifies the legality of a move. Places the stone on the board. Finds prisoners and remove them.
        /// </summary>
        /// <param name="previousBoard">The state of board before the move.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of previous game boards.</param>
        /// <returns>Object, which contains: the result of legality check, list of prisoners, the new state of game board.</returns>
        public MoveProcessingResult ProcessMove(GameBoard previousBoard, Move moveToMake, List<GameBoard> history)
        {
            GameBoard currentBoard = new GameBoard(previousBoard);
            Position position = moveToMake.Coordinates;
            MoveProcessingResult processingResult = new MoveProcessingResult();
            processingResult.Captures = new List<Position>();
            processingResult.NewBoard = previousBoard;

            //1. step: check intersection
            if (moveToMake.Kind == MoveKind.Pass)
            {
                processingResult.Result = Pass(moveToMake.WhoMoves);
                return processingResult;
            }
            else if (IsOutsideTheBoard(position) == MoveResult.OutsideTheBoard)
            {
                processingResult.Result = MoveResult.OutsideTheBoard;
                return processingResult;
            }
            else if (IsPositionOccupied(previousBoard, moveToMake) == MoveResult.OccupiedPosition)
            {
                processingResult.Result = MoveResult.OccupiedPosition;
                return processingResult;
            }
            else
            {
                //2. step: add stone
                currentBoard[moveToMake.Coordinates.X, moveToMake.Coordinates.Y] = moveToMake.WhoMoves;
                //3. step: find captures and remove prisoners
                processingResult.Captures = CheckCapture(currentBoard, moveToMake);
                for (int i = 0; i < processingResult.Captures.Count; i++)
                {
                    Position p = processingResult.Captures.ElementAt(i);
                    currentBoard[p.X, p.Y] = StoneColor.None;
                }
                //4. step: check selfcapture, ko, superko
                MoveResult r = CheckSelfCaptureKoSuperko(currentBoard, moveToMake, history);
                if (r == MoveResult.Legal)
                {
                    ModifyScoresAfterCapture(processingResult.Captures.Count, moveToMake.WhoMoves);
                    processingResult.Result = r;
                    processingResult.NewBoard = currentBoard;
                    return processingResult;
                }
                else
                {
                    processingResult.Result = r;
                    processingResult.Captures = new List<Position>();
                    processingResult.NewBoard = previousBoard;
                    return processingResult;
                }
            }
        }

        /// <summary>
        /// Gets all moves that can be legally made by the PLAYER on the CURRENT BOARD in a game with the specified HISTORY.
        /// </summary>
        /// <param name="player">The player who wants to make a move.</param>
        /// <param name="currentBoard">The current full board position.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>List of legal moves.</returns>
        public List<Position> GetAllLegalMoves(StoneColor player, GameBoard currentBoard, List<GameBoard> history)
        {
            List<Position> possiblePositions = new List<Position>();
            for (int x = 0; x < _boardWidth; x++)
                for (int y = 0; y < _boardHeight; y++)
                {
                    if (IsLegalMove(currentBoard, Move.PlaceStone(player, new Position(x, y)), history) == MoveResult.Legal)
                    {
                        possiblePositions.Add(new Position(x, y));
                    }
                }

            return possiblePositions;
        }

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="currentBoard">The current full board position.</param>
        /// <param name="moveToMake">The move of a player.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>The result of legality check.</returns>
        public MoveResult IsLegalMove(GameBoard currentBoard, Move moveToMake, List<GameBoard> history)
        {
            // TODO this should not alter the ruleset or players, the "IsLegalMove" method should be pure
            MoveProcessingResult result = ProcessMove(currentBoard, moveToMake, history);
            return result.Result;
        }

        /// <summary>
        /// Determines all positions that share the color of the specified position. "None" is also a color for the purposes of this method. This method
        /// is not thread-safe (it depends on <see cref="_checkedInters"/>).
        /// </summary>
        /// <param name="pos">The position whose group we want to identify.</param>
        /// <param name="board">The current full board position.</param>
        /// <returns></returns>
        public IEnumerable<Position> DiscoverGroup(Position pos, GameBoard board)
        {
            _checkedInters = new bool[_boardWidth, _boardHeight];
            List<Position> group = new List<Position>();
            bool iDontCare = false;
            GetGroup(ref group, ref iDontCare, pos, board);
            return group;
        }

        /// <summary>
        /// Determines which points belong to which player as territory. This is a pure thread-safe method. 
        /// All stones on the board are considered alive for the purposes of determining territory using this method.
        /// </summary>
        /// <param name="board">The current game board.</param>
        public Territory[,] DetermineTerritory(GameBoard board)
        {
            Territory[,] regions = new Territory[_boardWidth, _boardHeight];
            for (int i = 0; i < _boardWidth; i++)
            {
                for (int j = 0; j < _boardHeight; j++)
                {
                    regions[i, j] = Territory.Unknown;
                }
            }

            for (int i = 0; i < _boardWidth; i++)
            {
                for (int j = 0; j < _boardHeight; j++)
                {
                    if (regions[i, j] == Territory.Unknown && board[i, j] == StoneColor.None)
                    {
                        HashSet<Position> region = new HashSet<Position>();
                        Territory regionBelongsTo = Territory.Unknown;
                        Position p = new Position();
                        p.X = i;
                        p.Y = j;
                        GetRegion(ref region, ref regionBelongsTo, p, board);

                        foreach (Position regionMember in region)
                        {
                            regions[regionMember.X, regionMember.Y] = regionBelongsTo;
                        }

                    }
                }
            }
            return regions;
        }

        /// <summary>
        /// Sets the value of Komi.
        /// </summary>
        /// <param name="handicapStoneCount">Number of handicap stones.</param>
        protected abstract void SetKomi(int handicapStoneCount);

        protected abstract MoveResult CheckSelfCaptureKoSuperko(GameBoard currentBoard, Move moveToMake, List<GameBoard> history);

        /// <summary>
        /// Handles the pass of a player. Two consecutive passes signal the end of game.
        /// </summary>
        /// <param name="playerColor">Color of player, who passes.</param>
        /// <returns>The legality of move or new game phase notification.</returns>
        protected abstract MoveResult Pass(StoneColor playerColor);

        protected abstract void ModifyScoresAfterCapture(int capturedStoneCount, StoneColor removedStonesColor);

        /// <summary>
        /// Places handicape stones on fixed positions.
        /// </summary>
        /// <param name="currentBoard">Reference to the state of game board.</param>
        /// <param name="stoneCount">Number of handicap stones.</param>
        protected void PlaceFixedHandicapStones(ref GameBoard currentBoard, int stoneCount)
        {
            switch (_boardWidth)
            {
                case 9:
                    {
                        if (stoneCount <= HandicapPositions.MaxFixedHandicap9)
                            for (int i = 0; i < stoneCount; i++)
                            {
                                Position handicapPosition = HandicapPositions.FixedHandicapPositions9[i];
                                currentBoard[handicapPosition.X, handicapPosition.Y] = StoneColor.Black;
                            }
                        break;
                    }
                case 13:
                    {
                        if (stoneCount <= HandicapPositions.MaxFixedHandicap13)
                            for (int i = 0; i < stoneCount; i++)
                            {
                                Position handicapPosition = HandicapPositions.FixedHandicapPositions13[i];
                                currentBoard[handicapPosition.X, handicapPosition.Y] = StoneColor.Black;
                            }
                        break;
                    }
                case 19:
                    {
                        if (stoneCount <= HandicapPositions.MaxFixedHandicap19)
                            for (int i = 0; i < stoneCount; i++)
                            {
                                Position handicapPosition = HandicapPositions.FixedHandicapPositions19[i];
                                currentBoard[handicapPosition.X, handicapPosition.Y] = StoneColor.Black;
                            }
                        break;
                    }
                default:
                    break;
            }
        }


        /// <summary>
        /// Finds the prisoners of opponent player.
        /// </summary>
        /// <param name="currentBoard">The state of game board after the player's move.</param>
        /// <param name="moveToMake">The player's move</param>
        /// <returns>List of prisoners/captured stones.</returns>
        protected List<Position> CheckCapture(GameBoard currentBoard, Move moveToMake)
        {
            _liberty = FillLibertyTable(currentBoard);
            _checkedInters = new bool[_boardWidth, _boardHeight];
            _captures = new List<Position>();
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

            return _captures;
        }

        public List<Position> GetAllLegalMoves(GameBoard board)
        {
            // TODO make this work according to rules
            List<Position> legalMoves = new List<Position>();
            for (int i = 0; i < board.Size.Width; i++)
                for (int j = 0; j < board.Size.Height; j++)
                {
                    if (board[i, j] == StoneColor.None)
                    {
                        legalMoves.Add(new Position() { X = i, Y = j });
                    }
                }
            return legalMoves;
        }

        /// <summary>
        /// Checks the liberty of surrounding groups.
        /// </summary>
        /// <param name="x">Letter-based coordinate of position.</param>
        /// <param name="y">Number-based coordinate of position.</param>
        /// <param name="currentBoard">The state of game board after the player's move.</param>
        protected void CheckNeighbourGroup(int x, int y, GameBoard currentBoard)
        {
            List<Position> group = new List<Position>();
            bool groupHasLiberty = false;
            Position p = new Position();

            p.X = x;
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
            }

        }

        protected bool[,] FillLibertyTable(GameBoard currentBoard)
        {
            _liberty = new bool[_boardWidth, _boardHeight];

            //check whether position has liberty
            for (int i = 0; i < _boardWidth; i++)
            {
                for (int j = 0; j < _boardHeight; j++)
                {
                    bool emptyNeighbour = false;
                    //it has empty left neighbour
                    if (i > 0 && currentBoard[i - 1, j] == StoneColor.None)
                    {
                        emptyNeighbour = true;
                    }
                    else if (i < _boardWidth - 1 && currentBoard[i + 1, j] == StoneColor.None) //it has empty right neighbour
                    {
                        emptyNeighbour = true;
                    }
                    else if (j > 0 && currentBoard[i, j - 1] == StoneColor.None) //it has empty bottom neighbour
                    {
                        emptyNeighbour = true;
                    }
                    else if (j < _boardHeight - 1 && currentBoard[i, j + 1] == StoneColor.None) //it has empty upper neighbour
                    {
                        emptyNeighbour = true;
                    }
                    _liberty[i, j] = emptyNeighbour;
                }
            }

            return _liberty;
        }

        protected void GetGroup(ref List<Position> group, ref bool hasLiberty, Position pos, GameBoard currentBoard)
        {
            StoneColor currentColor = currentBoard[pos.X, pos.Y];
            if (!_checkedInters[pos.X, pos.Y])
            {
                group.Add(pos);
                _checkedInters[pos.X, pos.Y] = true;
            }
            Position newp = new Position();

            if (_liberty[pos.X, pos.Y])
                hasLiberty = true;
            //has same unchecked right neighbour
            if (pos.X < _boardWidth - 1 && currentBoard[pos.X + 1, pos.Y] == currentColor && !_checkedInters[pos.X + 1, pos.Y])
            {
                newp.X = pos.X + 1;
                newp.Y = pos.Y;
                GetGroup(ref group, ref hasLiberty, newp, currentBoard);
            }
            //has same unchecked upper neighbour
            if (pos.Y < _boardHeight - 1 && currentBoard[pos.X, pos.Y + 1] == currentColor && !_checkedInters[pos.X, pos.Y + 1])
            {
                newp.X = pos.X;
                newp.Y = pos.Y + 1;
                GetGroup(ref group, ref hasLiberty, newp, currentBoard);
            }
            //has same unchecked left neighbour
            if (pos.X > 0 && currentBoard[pos.X - 1, pos.Y] == currentColor && !_checkedInters[pos.X - 1, pos.Y])
            {
                newp.X = pos.X - 1;
                newp.Y = pos.Y;
                GetGroup(ref group, ref hasLiberty, newp, currentBoard);
            }
            //has same unchecked bottom neighbour
            if (pos.Y > 0 && currentBoard[pos.X, pos.Y - 1] == currentColor && !_checkedInters[pos.X, pos.Y - 1])
            {
                newp.X = pos.X;
                newp.Y = pos.Y - 1;
                GetGroup(ref group, ref hasLiberty, newp, currentBoard);
            }
        }

        /// <summary>
        /// Checks whether the player places a stone on the intersection where the opponent has just captured a stone.
        /// </summary>
        /// <param name="currentBoard">The state of game board.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of game boards that represents the history of game.</param>
        /// <returns>The result of legality check.</returns>
        protected MoveResult IsKo(GameBoard currentBoard, Move moveToMake, List<GameBoard> history)
        {
            int boardHistoryCount = history.Count;
            if (boardHistoryCount >= 2 && history.ElementAt(boardHistoryCount - 2).Equals(currentBoard))
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
        protected MoveResult IsSuperKo(GameBoard currentBoard, Move moveToMake, List<GameBoard> history)
        {
            for (int i = 0; i < history.Count; i++)
            {
                if (history.ElementAt(i).Equals(currentBoard))
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
        protected MoveResult IsPositionOccupied(GameBoard currentBoard, Move moveToMake)
        {
            Position p = moveToMake.Coordinates;
            if (currentBoard[p.X, p.Y] != StoneColor.None)
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
        protected MoveResult IsSelfCapture(GameBoard currentBoard, Move moveToMake)
        {
            Position p = moveToMake.Coordinates;
            List<Position> group = new List<Position>();
            bool groupHasLiberty = false;
            _checkedInters = new bool[_boardWidth, _boardHeight];

            currentBoard[p.X, p.Y] = moveToMake.WhoMoves;
            _liberty = FillLibertyTable(currentBoard);
            GetGroup(ref group, ref groupHasLiberty, p, currentBoard);
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
            if (p.X < 0 || p.X >= _boardWidth || p.Y < 0 || p.Y >= _boardHeight)
                return MoveResult.OutsideTheBoard;
            else
                return MoveResult.Legal;
        }

        /// <summary>
        /// The area of a player are all live stones of player left on the board together with any points of his territory. In this case, prisoners are ignored.
        /// This method adds up total area of players.
        /// </summary>
        /// <param name="currentBoard">The state of board after removing dead stones.</param>
        /// <returns>The score of players.</returns>
        protected Scores CountArea(GameBoard currentBoard)
        {
            Scores scores = CountTerritory(currentBoard);
            for (int i = 0; i < _boardWidth; i++)
            {
                for (int j = 0; j < _boardHeight; j++)
                {
                    if (currentBoard[i, j] == StoneColor.Black)
                        scores.BlackScore++;
                    else if (currentBoard[i, j] == StoneColor.White)
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
        protected Scores CountTerritory(GameBoard currentBoard)
        {
            Scores scores = new Scores();
            scores.WhiteScore = 0;
            scores.BlackScore = 0;

            Territory[,] regions = DetermineTerritory(currentBoard);

            for (int i = 0; i < _boardWidth; i++)
            {
                for (int j = 0; j < _boardHeight; j++)
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
            Position pos,
            GameBoard currentBoard)
        {
            if (region.Contains(pos)) return;

            region.Add(pos);
            if (pos.X < _boardWidth - 1) //has right neighbour
            {
                Position newp = new Position();
                newp.X = pos.X + 1;
                newp.Y = pos.Y;

                switch (currentBoard[pos.X + 1, pos.Y])
                {
                    case StoneColor.None:
                        GetRegion(ref region, ref regionBelongsTo, newp, currentBoard);
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
            if (pos.Y < _boardHeight - 1) //has upper neighbour
            {
                Position newp = new Position();
                newp.X = pos.X;
                newp.Y = pos.Y + 1;
                switch (currentBoard[pos.X, pos.Y + 1])
                {
                    case StoneColor.None:
                        GetRegion(ref region, ref regionBelongsTo, newp, currentBoard);
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
                Position newp = new Position();
                newp.X = pos.X - 1;
                newp.Y = pos.Y;
                switch (currentBoard[pos.X - 1, pos.Y])
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
                        GetRegion(ref region, ref regionBelongsTo, newp, currentBoard);
                        break;
                    default:
                        break;
                }
            }
            if (pos.Y > 0) //has bottom neighbour
            {
                Position newp = new Position();
                newp.X = pos.X;
                newp.Y = pos.Y - 1;
                switch (currentBoard[pos.X, pos.Y - 1])
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
                        GetRegion(ref region, ref regionBelongsTo, newp, currentBoard);
                        break;
                    default:
                        //TODO Exception
                        break;
                }
            }

        }

    }
}
