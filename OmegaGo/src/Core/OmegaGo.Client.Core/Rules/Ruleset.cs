using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    //TODO comments!
    public abstract class Ruleset
    {
        public int Score;
        public int BoardWidth, BoardHeight;

        public void startGame(Player white, Player black, GameBoardSize gbSize)
        {
            BoardWidth = gbSize.Width;
            BoardHeight = gbSize.Height;
            return;
            // TODO Petr: I'll just comment this out for a while so I can get it at least somewhat working.
            throw new NotImplementedException();
        }

        public abstract void PutHandicapStone(Move moveToMake);

        public abstract MoveResult ControlMove(StoneColor[,] currentBoard, Move moveToMake, List<StoneColor[,]> history);

        //TODO test!
        public StoneColor[,] ControlCaptureAndRemoveStones(StoneColor[,] currentBoard)
        {
            bool[,] Liberty = FillLibertyTable(currentBoard);

            //control if group has liberty
            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)
                {
                    if (Liberty[i, j] == false)
                    {
                        List<Position> group = new List<Position>();
                        bool groupHasLiberty = false;
                        Position p = new Position();
                        p.X = i;
                        p.Y = j;
                        GetGroup(ref group, ref groupHasLiberty, p, currentBoard, Liberty);

                        //if group has liberty, setup true liberty for all; else remove the group from the board
                        for (int k = 0; k < group.Count; k++)
                        {
                            Position groupMember = group.ElementAt(k);
                            if (groupHasLiberty)
                            {
                                Liberty[groupMember.X, groupMember.Y] = true;
                            }
                            else
                            {
                                currentBoard[groupMember.X, groupMember.Y] = StoneColor.None;
                            }
                        }
                    }
                }
            }

            return currentBoard;
        }

        protected bool[,] FillLibertyTable(StoneColor[,] currentBoard)
        {
            bool[,] Liberty = new bool[BoardWidth, BoardHeight];

            //control if position has liberty
            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)
                {
                    bool emptyNeighbour = false;
                    //it has empty left neighbour
                    if (i > 0 && currentBoard[i - 1, j] == StoneColor.None)
                    {
                        emptyNeighbour = true;
                    }
                    else if (i < BoardWidth - 1 && currentBoard[i + 1, j] == StoneColor.None) //it has empty right neighbour
                    {
                        emptyNeighbour = true;
                    }
                    else if (j > 0 && currentBoard[i, j - 1] == StoneColor.None) //it has empty bottom neighbour
                    {
                        emptyNeighbour = true;
                    }
                    else if (j < BoardHeight - 1 && currentBoard[i, j + 1] == StoneColor.None) //it has empty upper neighbour
                    {
                        emptyNeighbour = true;
                    }
                    Liberty[i, j] = emptyNeighbour;
                }
            }

            return Liberty;
        }

        //TODO test!
        protected void GetGroup(ref List<Position> group, ref bool hasLiberty, Position pos, StoneColor[,] currentBoard, bool[,] Liberty)
        {
            StoneColor currentColor = currentBoard[pos.X, pos.Y];
            group.Add(pos);

            if (Liberty[pos.X, pos.Y])
                hasLiberty = true;

            if (pos.X < BoardWidth - 1 && currentBoard[pos.X + 1, pos.Y] == currentColor) //has same right neighbour
            {
                Position newp = new Position();
                newp.X = pos.X + 1;
                newp.Y = pos.Y;
                GetGroup(ref group, ref hasLiberty, newp, currentBoard, Liberty);
            }
            if (pos.Y < BoardHeight - 1 && currentBoard[pos.X, pos.Y + 1] == currentColor) //has same upper neighbour
            {
                Position newp = new Position();
                newp.X = pos.X;
                newp.Y = pos.Y + 1;
                GetGroup(ref group, ref hasLiberty, newp, currentBoard, Liberty);
            }

        }

        public bool AreBoardsEqual(StoneColor[,] b1, StoneColor[,] b2)
        {
            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)
                {
                    if (b1[i, j] != b2[i, j])
                        return false;
                }
            }

            return true;
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

            currentBoard[p.X, p.Y] = moveToMake.WhoMoves;
            bool[,] Liberty = FillLibertyTable(currentBoard);
            GetGroup(ref group, ref groupHasLiberty, p, currentBoard, Liberty);
            if (groupHasLiberty)
            {
                return MoveResult.Legal;
            }
            else
            {
                return MoveResult.SelfCapture;
            }

        }

        public abstract int CountScore(StoneColor[,] currentBoard);

        protected void CountArea(StoneColor[,] currentBoard)
        {
            throw new NotImplementedException();
        }

        protected void CountTerritory(StoneColor[,] currentBoard)
        {
            Territory[,] regions = new Territory[BoardHeight, BoardWidth];

            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    regions[i, j] = Territory.Unknown;
                }
            }

            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
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
            if (pos.X < BoardWidth - 1 ) //has right neighbour
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
                        //TODO Exception
                        break;
                }
                
            }
            if (pos.Y < BoardHeight - 1 ) //has upper neighbour
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
                        //TODO Exception
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
                        //TODO Exception
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
            for (int x = 0; x < BoardWidth; x++) 
                for (int y = 0; y < BoardHeight; y++)
                {
                    if (ControlMove(currentBoard, Move.Create(player, new Core.Position(x, y)), history) == MoveResult.Legal)
                    {
                        possiblePositions.Add(new Core.Position(x, y));
                    }
                }

            return possiblePositions;
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
