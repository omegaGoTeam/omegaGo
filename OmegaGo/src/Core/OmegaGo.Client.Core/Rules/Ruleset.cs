using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
   
    public abstract class Ruleset
    {
        private int _boardWidth, _boardHeight;
        private bool[,] _checkedInters;
        private bool[,] _liberty;
        private List<Position> _captures;


        /// <summary>
        /// Initializes the ruleset. For each game, a new ruleset must be created.
        /// </summary>
        /// <param name="white"></param>
        /// <param name="black"></param>
        /// <param name="gbSize">Size of the game board.</param>
        public Ruleset(Player white, Player black, GameBoardSize gbSize)
        {
            _boardWidth = gbSize.Width;
            _boardHeight = gbSize.Height;
            _checkedInters = new bool[_boardWidth, _boardHeight];
            _liberty = new bool[_boardWidth, _boardHeight];
            _captures = new List<Position>();
        }

        public abstract void PlaceHandicapStone(Move moveToMake);
        public abstract int CountScore(StoneColor[,] currentBoard);

        /// <summary>
        /// Verifies the legality of a move. Places the stone on the board. Finds prisoners and remove them.
        /// </summary>
        /// <param name="previousBoard">The state of board before the move.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">List of previous game boards.</param>
        /// <returns>Object, which contains: the result of legality check, list of prisoners, the new state of game board.</returns>
        public MoveProcessingResult ProcessMove(StoneColor[,] previousBoard, Move moveToMake, List<StoneColor[,]> history)
        {
            StoneColor[,] currentBoard = (StoneColor[,])previousBoard.Clone();
            Position position = moveToMake.Coordinates;
            MoveProcessingResult processingResult = new MoveProcessingResult();
            processingResult.Captures = new List<Position>();
            processingResult.NewBoard = previousBoard;

            //1. step: check intersection
            if (moveToMake.Kind == MoveKind.Pass)
            {
                processingResult.Result = Pass();
                return processingResult;
            }
            else if (position.X < 0 || position.X >= _boardWidth || position.Y < 0 || position.Y >= _boardHeight)
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
        /// Checks whether 2 game boards equal.
        /// </summary>
        /// <param name="b1">First game board.</param>
        /// <param name="b2">Second game board.</param>
        /// <returns></returns>
        public bool AreBoardsEqual(StoneColor[,] b1, StoneColor[,] b2)
        {
            for (int i = 0; i < _boardWidth; i++)
            {
                for (int j = 0; j < _boardHeight; j++)
                {
                    if (b1[i, j] != b2[i, j])
                        return false;
                }
            }

            return true;
        }

       
        /// <summary>
        /// Gets all moves that can be legally made by the PLAYER on the CURRENT BOARD in a game with the specified HISTORY.
        /// </summary>
        /// <param name="player">The player who wants to make a move.</param>
        /// <param name="currentBoard">The current full board position.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns></returns>
        public List<Position> GetAllLegalMoves(StoneColor player, StoneColor[,] currentBoard, List<StoneColor[,]> history)
        {
            List<Position> possiblePositions = new List<Core.Position>();
            for (int x = 0; x < _boardWidth; x++)
                for (int y = 0; y < _boardHeight; y++)
                {
                    if (IsLegalMove(currentBoard, Move.Create(player, new Core.Position(x, y)), history) == MoveResult.Legal)
                    {
                        possiblePositions.Add(new Core.Position(x, y));
                    }
                }

            return possiblePositions;
        }

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="moveToMake"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public MoveResult IsLegalMove(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history)
        {
            MoveProcessingResult result = ProcessMove(currentBoard, moveToMake, history);
            return result.Result;
        }

        protected abstract MoveResult CheckSelfCaptureKoSuperko(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history);

        protected abstract MoveResult Pass();

        /// <summary>
        /// Checks the player's move.
        /// </summary>
        /// <param name="previousBoard">The state of game board before the player's move.</param>
        /// <param name="moveToMake">The player's move</param>
        /// <param name="history">List of game boards that represents the history of game.</param>
        /// <returns>Result of player's move, list of prisoners/captured stones and the new state of game board</returns>
        protected List<Position> CheckCapture(StoneColor[,] currentBoard, Move moveToMake)
        {
            _liberty = FillLibertyTable(currentBoard);
            _checkedInters = new bool[_boardWidth, _boardHeight];
            _captures = new List<Position>();
            int currentX = moveToMake.Coordinates.X;
            int currentY = moveToMake.Coordinates.Y;
            StoneColor opponentColor = (moveToMake.WhoMoves == StoneColor.Black) ? StoneColor.White : StoneColor.Black;


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

        protected void CheckNeighbourGroup(int x, int y,  StoneColor[,] currentBoard)
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

        protected bool[,] FillLibertyTable(StoneColor[,] currentBoard)
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

        protected void GetGroup(ref List<Position> group, ref bool hasLiberty, Position pos, StoneColor[,] currentBoard)
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

        protected MoveResult IsKo(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history)
        {
            int boardHistoryCount = history.Count;
            if (boardHistoryCount >= 2 && AreBoardsEqual(history.ElementAt(boardHistoryCount - 2), currentBoard))
                return MoveResult.Ko;

            return MoveResult.Legal;
        }

        protected MoveResult IsSuperKo(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history)
        {
            for (int i = 0; i < history.Count; i++)
            {
                if (AreBoardsEqual(history.ElementAt(i), currentBoard))
                    return MoveResult.SuperKo;
            }
            return MoveResult.Legal;
        }

        protected MoveResult IsPositionOccupied(StoneColor[,] currentBoard, Move moveToMake)
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

        protected MoveResult IsSelfCapture(StoneColor[,] currentBoard, Move moveToMake)
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

        protected void CountArea(StoneColor[,] currentBoard)
        {
            throw new NotImplementedException();
        }

        protected void CountTerritory(StoneColor[,] currentBoard)
        {
            Territory[,] regions = new Territory[_boardHeight, _boardWidth];

            for (int i = 0; i < _boardHeight; i++)
            {
                for (int j = 0; j < _boardWidth; j++)
                {
                    regions[i, j] = Territory.Unknown;
                }
            }

            for (int i = 0; i < _boardHeight; i++)
            {
                for (int j = 0; j < _boardWidth; j++)
                {
                    if (regions[i, j] == Territory.Unknown && currentBoard[i,j]== StoneColor.None)
                    {
                        List<Position> region = new List<Position>();
                        Territory regionBelongsTo = Territory.Unknown;
                        Position p = new Position();
                        p.X = i;
                        p.Y = j;
                        GetRegion(ref region,ref regionBelongsTo,p,currentBoard);

                        for (int k = 0; k < region.Count; k++)
                        {
                            Position regionMember = region.ElementAt(k);
                            regions[regionMember.X, regionMember.Y] = regionBelongsTo;    
                        }

                    }
                }
            }
        }

        protected void GetRegion(ref List<Position> region, ref Territory regionBelongsTo, Position pos, StoneColor[,] currentBoard)
        {
            region.Add(pos);
            if (pos.X < _boardWidth - 1 ) //has right neighbour
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
            if (pos.Y < _boardHeight - 1 ) //has upper neighbour
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
                switch (currentBoard[pos.X-1, pos.Y])
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
                        break;
                    default:
                        break;
                }
            }
            if (pos.Y > 0) //has bottom neighbour
            {
                switch (currentBoard[pos.X, pos.Y-1])
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
                        break;
                    default:
                        //TODO Exception
                        break;
                }
            }

        }

    }

    
    public enum Territory
    {
        White,
        Black,
        Neutral,
        Unknown
    }
}
