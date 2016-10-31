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

        public abstract MoveResult ControlMove(Color[,] currentBoard, Move moveToMake, List<Color[,]> history);

        //TODO test!
        public Color[,] ControlCaptureAndRemoveStones(Color[,] currentBoard)
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
                                currentBoard[groupMember.X, groupMember.Y] = Color.None;
                            }
                        }
                    }
                }
            }

            return currentBoard;
        }

        protected bool[,] FillLibertyTable(Color[,] currentBoard)
        {
            bool[,] Liberty = new bool[BoardWidth, BoardHeight];

            //control if position has liberty
            for (int i = 0; i < BoardWidth; i++)
            {
                for (int j = 0; j < BoardHeight; j++)
                {
                    bool emptyNeighbour = false;
                    //it has empty left neighbour
                    if (i > 0 && currentBoard[i - 1, j] == Color.None)
                    {
                        emptyNeighbour = true;
                    }
                    else if (i < BoardWidth - 1 && currentBoard[i + 1, j] == Color.None) //it has empty right neighbour
                    {
                        emptyNeighbour = true;
                    }
                    else if (j > 0 && currentBoard[i, j - 1] == Color.None) //it has empty bottom neighbour
                    {
                        emptyNeighbour = true;
                    }
                    else if (j < BoardHeight - 1 && currentBoard[i, j + 1] == Color.None) //it has empty upper neighbour
                    {
                        emptyNeighbour = true;
                    }
                    Liberty[i, j] = emptyNeighbour;
                }
            }

            return Liberty;
        }

        //TODO test!
        protected void GetGroup(ref List<Position> group, ref bool hasLiberty, Position pos, Color[,] currentBoard, bool[,] Liberty)
        {
            Color currentColor = currentBoard[pos.X, pos.Y];
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

        public bool AreBoardsEqual(Color[,] b1, Color[,] b2)
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

        protected MoveResult IsKo(Color[,] currentBoard, Move moveToMake, List<Color[,]> history)
        {
            int boardHistoryCount = history.Count;
            if (boardHistoryCount >= 2 && AreBoardsEqual(history.ElementAt(boardHistoryCount - 2), currentBoard))
                return MoveResult.Ko;

            return MoveResult.Legal;
        }

        protected MoveResult IsSuperKo(Color[,] currentBoard, Move moveToMake, List<Color[,]> history)
        {
            for (int i = 0; i < history.Count; i++)
            {
                if (AreBoardsEqual(history.ElementAt(i), currentBoard))
                    return MoveResult.SuperKo;
            }
            return MoveResult.Legal;
        }

        protected MoveResult IsPositionOccupied(Color[,] currentBoard, Move moveToMake)
        {
            Position p = moveToMake.Coordinates;
            if (currentBoard[p.X, p.Y] != Color.None)
            {
                return MoveResult.OccupiedPosition;
            }
            else
            {
                return MoveResult.Legal;
            }

        }

        protected MoveResult IsSelfCapture(Color[,] currentBoard, Move moveToMake)
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

        public abstract int CountScore(Color[,] currentBoard);

        protected void CountArea(Color[,] currentBoard)
        {
            throw new NotImplementedException();
        }

        protected void CountTerritory(Color[,] currentBoard)
        {
            Territory[,] regions = new Territory[BoardHeight, BoardWidth];

            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    regions[i, j] = Territory.Unknown;
                }
            }

            //TODO: call GetRegion
            throw new NotImplementedException();

        }

        protected void GetRegion(ref List<Position> region, ref Territory regionBelongsTo, Position pos, Color[,] currentBoard)
        {
            region.Add(pos);
            if (pos.X < BoardWidth - 1) //has same right neighbour
            {
                Position newp = new Position();
                newp.X = pos.X + 1;
                newp.Y = pos.Y;

                switch (currentBoard[pos.X + 1, pos.Y])
                {
                    case Color.None:
                        GetRegion(ref region, ref regionBelongsTo, newp, currentBoard);
                        break;
                    case Color.Black:
                        if (regionBelongsTo == Territory.White)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.Black;
                        break;
                    case Color.White:
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
            if (pos.Y < BoardHeight - 1) //has same upper neighbour
            {
                Position newp = new Position();
                newp.X = pos.X;
                newp.Y = pos.Y + 1;
                switch (currentBoard[pos.X, pos.Y + 1])
                {
                    case Color.None:
                        GetRegion(ref region, ref regionBelongsTo, newp, currentBoard);
                        break;
                    case Color.Black:
                        if (regionBelongsTo == Territory.White)
                            regionBelongsTo = Territory.Neutral;
                        else if (regionBelongsTo == Territory.Unknown)
                            regionBelongsTo = Territory.Black;
                        break;
                    case Color.White:
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
            //TODO control left, upper neighbour
            throw new NotImplementedException();

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
