using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;

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
            RulesetInfo = new RulesetInfo(gbSize);
            GameBoard newBoard = new GameBoard(gbSize);
            GroupState groupState = new GroupState(RulesetInfo);
        }

        public IRulesetInfo RulesetInfo { get; }

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
        /// Calculates the default compensation (komi).
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
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="deadPositions">List of dead stones.</param>
        /// <param name="komi">Komi compensation.</param>
        /// <returns>The score of players.</returns>
        public abstract Scores CountScore(GameTreeNode currentNode, IEnumerable<Position> deadPositions, float komi);

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
            RulesetInfo.GroupState.AddStoneToBoard(position,StoneColor.Black);
            return MoveResult.Legal;
        }

        /// <summary>
        /// Sets the state of ruleset.
        /// </summary>
        /// <param name="boardState">State of board to use.</param>
        /// <param name="groupState">State of groups to use.</param>
        public void SetRulesetInfo(GameBoard boardState, GroupState groupState)
        {
            RulesetInfo.SetState(boardState, groupState);
        }

        /// <summary>
        /// Determines whether a move is legal. Information about any captures and the new board state are discarded.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="moveToMake">The move of a player.</param>
        /// <returns>The result of legality check.</returns>
        public MoveResult IsLegalMove(GameTreeNode currentNode, Move moveToMake)
        {
            MoveProcessingResult result = ProcessMove(currentNode, moveToMake);
            return result.Result;
        }

        /// <summary>
        /// Verifies the legality of a move. Places the stone on the board. Finds prisoners and remove them.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="moveToMake">Move to check.</param>
        /// <returns>Object, which contains: the result of legality check, list of prisoners, the new state of game board, the new state of groups.</returns>
        public MoveProcessingResult ProcessMove(GameTreeNode currentNode, Move moveToMake)
        {
            lock (RulesetInfo)
            {
                StoneColor player = moveToMake.WhoMoves;
                Position position = moveToMake.Coordinates;
                GameBoard[] history = new GameBoard[0];
                GroupState previousGroupState, currentGroupState;
                GameBoard previousBoard, currentBoard;
                if (currentNode == null)
                {
                    previousGroupState = new GroupState(RulesetInfo);
                    currentGroupState = new GroupState(RulesetInfo);
                    previousBoard = new GameBoard(RulesetInfo.BoardSize);
                    currentBoard = new GameBoard(RulesetInfo.BoardSize);
                }
                else
                {
                    history = currentNode.GetGameBoardHistory().ToArray();
                    //set Ruleset state
                    previousGroupState = new GroupState(currentNode.GroupState, RulesetInfo);
                    currentGroupState = new GroupState(currentNode.GroupState, RulesetInfo);
                    previousBoard = new GameBoard(currentNode.BoardState);
                    currentBoard = new GameBoard(currentNode.BoardState);
                }

                SetRulesetInfo(currentBoard, currentGroupState);

                MoveProcessingResult processingResult = new MoveProcessingResult
                {
                    Captures = new List<Position>(),
                    NewBoard = previousBoard,
                    NewGroupState = previousGroupState
                };

                //1. step: check intersection
                if (moveToMake.Kind == MoveKind.Pass)
                {
                    processingResult.Result = Pass(currentNode);
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
                    RulesetInfo.GroupState.AddStoneToBoard(moveToMake.Coordinates, moveToMake.WhoMoves);

                    //3. step: find captures and remove prisoners
                    List<int> capturedGroups = CheckCapturedGroups(moveToMake);
                    foreach (int groupID in capturedGroups)
                    {
                        processingResult.Captures.AddRange(RulesetInfo.GroupState.Groups[groupID].Members);
                        RulesetInfo.GroupState.Groups[groupID].DeleteGroup();
                    }

                    //4. step: check selfcapture, ko
                    MoveResult r = CheckSelfCaptureKoSuperko(moveToMake, history);

                    if (r == MoveResult.Legal)
                    {
                        RulesetInfo.GroupState.CountLiberties();
                        processingResult.NewBoard = currentBoard;
                        processingResult.NewGroupState = currentGroupState;
                    }
                    else
                    {
                        SetRulesetInfo(previousBoard, previousGroupState);
                    }

                    processingResult.Result = r;
                    return processingResult;
                }
            }
        }
        
        /// <summary>
        /// Gets the results of moves.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <returns>Map of move results.</returns>
        public MoveResult[,] GetMoveResult(GameTreeNode currentNode)
        {
            if (currentNode == null)
            {
                MoveResult[,] moveResults = new MoveResult[RulesetInfo.BoardSize.Width, RulesetInfo.BoardSize.Height];
                for (int x = 0; x < RulesetInfo.BoardSize.Width; x++)
                    for (int y = 0; y < RulesetInfo.BoardSize.Height; y++)
                        moveResults[x,y] = MoveResult.Legal;
                return moveResults;
            }

            lock (RulesetInfo)
            {
                MoveResult[,] moveResults = new MoveResult[RulesetInfo.BoardSize.Width, RulesetInfo.BoardSize.Height];
                GameBoard[] history = currentNode.GetGameBoardHistory().ToArray();
                GameBoard boardState = new GameBoard(currentNode.BoardState);
                GroupState groupState = new GroupState(currentNode.GroupState, RulesetInfo);
                StoneColor player;

                if (currentNode.Move.WhoMoves == StoneColor.None)
                    player = StoneColor.White; // TODO (future work)  Petr: ensure this is actually appropriate in all such situations (probably isn't)
                else
                    player = currentNode.Move.WhoMoves.GetOpponentColor();

                for (int x = 0; x < RulesetInfo.BoardSize.Width; x++)
                    for (int y = 0; y < RulesetInfo.BoardSize.Height; y++)
                    {
                        //set Ruleset state
                        SetRulesetInfo(boardState, groupState);
                        Position position = new Position(x, y);
                        Move move = Move.PlaceStone(player, position);

                        if (IsPositionOccupied(position) == MoveResult.OccupiedPosition)
                        {
                            moveResults[x, y] = MoveResult.OccupiedPosition;
                        }
                        else
                        {
                            //Find captures and remove prisoners
                            List<int> capturedGroups = CheckPossiblyCapturedGroups(move);

                            if (capturedGroups.Count != 0)
                                SetRulesetInfo(new GameBoard(currentNode.BoardState), new GroupState(currentNode.GroupState, RulesetInfo));

                            //remove prisoners
                            foreach (int groupID in capturedGroups)
                            {
                                RulesetInfo.GroupState.Groups[groupID].DeleteGroup();
                            }

                            // add temporarily a stone to board
                            RulesetInfo.GroupState.AddTempStoneToBoard(position, player);
                            //check selfcapture, ko
                            moveResults[x, y] = CheckSelfCaptureKo(move, history);
                            // remove added stone
                            RulesetInfo.GroupState.RemoveTempStoneFromPosition(position);
                        }
                    }

                return moveResults;
            }
        }

        /// <summary>
        /// Gets the results of moves. Checks whether the intersection is occupied.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <returns>Map of move results.</returns>
        public MoveResult[,] GetMoveResultLite(GameTreeNode currentNode)
        {
            int width = currentNode.BoardState.Size.Width;
            int height = currentNode.BoardState.Size.Height;
            MoveResult[,] moveResults = new MoveResult[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (currentNode.BoardState[x,y] == StoneColor.None)
                        moveResults[x,y]= MoveResult.Legal;
                    else
                        moveResults[x, y] = MoveResult.OccupiedPosition;
                }
            return moveResults;
        }

        /// <summary>
        /// Determines which points belong to which player as territory. This is a pure thread-safe method. 
        /// All stones on the board are considered alive for the purposes of determining territory using this method.
        /// </summary>
        /// <param name="board">The current game board.</param>
        /// <returns>Map of territories.</returns>
        public Territory[,] DetermineTerritory(GameBoard board)
        {
            RulesetInfo.SetBoard(board);

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

        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Checks 3 illegal move types: self capture, ko, superko (Japanese ruleset permits superko).
        /// </summary>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>The result of legality check.</returns>
        protected abstract MoveResult CheckSelfCaptureKoSuperko(Move moveToMake, GameBoard[] history);

        /// <summary>
        /// Handles the pass of a player. Two consecutive passes signal the end of game. (In AGA, black player starts the passing)
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <returns>The legality of move or new game phase notification.</returns>
        protected abstract MoveResult Pass(GameTreeNode currentNode);

        /// <summary>
        /// Checks 2 illegal move types: self capture, ko.
        /// </summary>
        /// <param name="moveToMake">Move to check.</param>
        /// <param name="history">All previous full board positions.</param>
        /// <returns>The result of legality check.</returns>
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
        /// Checks whether the player places a stone on the intersection where the opponent has just captured a stone.
        /// </summary>
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
        /// <param name="p">Position to check.</param>
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
        /// <param name="moveToMake">Move to check.</param>
        /// <returns>The result of legality check.</returns>
        protected MoveResult IsSelfCapture(Move moveToMake)
        {
            Position p = moveToMake.Coordinates;
            if (CheckGroupLiberty(GetGroupID(p)))
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
        /// The area of a player are all live stones of player left on the board together with any points of his territory. In this case, prisoners are ignored.
        /// This method adds up total area of players.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="deadPositions">Positions marked as dead.</param>
        /// <returns>The score of players.</returns>
        protected Scores CountArea(GameTreeNode currentNode, IEnumerable<Position> deadPositions)
        {
            Scores scores = GetRegionScores(currentNode, deadPositions);
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
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="deadPositions">Positions marked as dead.</param>
        /// <returns>The score of players, which does not include number of prisoners and dead stones.</returns>
        protected Scores CountTerritory(GameTreeNode currentNode, IEnumerable<Position> deadPositions)
        {
            Scores scores = new Scores(0, 0);

            //prisoners
            IEnumerable<GameTreeNode> history = currentNode.GetNodeHistory();
            foreach (GameTreeNode node in history)
            {
                if (node.Move.Captures.Count != 0)
                {
                    if (node.Move.WhoMoves == StoneColor.Black)
                        scores.WhiteScore -= node.Move.Captures.Count;
                    else if (node.Move.WhoMoves == StoneColor.White)
                        scores.BlackScore -= node.Move.Captures.Count;
                }
            }

            //dead stones
            foreach (Position position in deadPositions)
            {
                if (RulesetInfo.BoardState[position.X, position.Y] == StoneColor.Black)
                    scores.BlackScore--;
                else if (RulesetInfo.BoardState[position.X, position.Y] == StoneColor.White)
                    scores.WhiteScore--;
            }

            //regions
            Scores regionScores= GetRegionScores(currentNode, deadPositions);
            scores.WhiteScore += regionScores.WhiteScore;
            scores.BlackScore += regionScores.BlackScore;

            return scores;
        }

        /// <summary>
        /// Finds the prisoners of opponent player.
        /// </summary>
        /// <param name="moveToMake">The player's move</param>
        /// <returns>List of prisoners/captured stones.</returns>
        private List<int> CheckCapturedGroups(Move moveToMake)
        {
            int currentX = moveToMake.Coordinates.X;
            int currentY = moveToMake.Coordinates.Y;
            StoneColor opponentColor = moveToMake.WhoMoves.GetOpponentColor();
            List<int> capturedGroups = new List<int>();
            int groupID;

            //check whether neighbour groups have liberty
            //right neighbour
            if (currentX < RulesetInfo.BoardSize.Width - 1 && RulesetInfo.BoardState[currentX + 1, currentY] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX + 1, currentY));
                if (!CheckGroupLiberty(groupID))
                    capturedGroups.Add(groupID);
            }

            //left neighbour
            if (currentX > 0 && RulesetInfo.BoardState[currentX - 1, currentY] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX - 1, currentY));
                if (!CheckGroupLiberty(groupID))
                    capturedGroups.Add(groupID);
            }

            //upper neighbour
            if (currentY < RulesetInfo.BoardSize.Height - 1 && RulesetInfo.BoardState[currentX, currentY + 1] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX, currentY + 1));
                if (!CheckGroupLiberty(groupID))
                    capturedGroups.Add(groupID);
            }
            //bottom neighbour
            if (currentY > 0 && RulesetInfo.BoardState[currentX, currentY - 1] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX, currentY - 1));
                if (!CheckGroupLiberty(groupID))
                    capturedGroups.Add(groupID);
            }

            return capturedGroups.Distinct().ToList();
        }

        /// <summary>
        /// Finds the possible prisoners of opponent player.
        /// </summary>
        /// <param name="moveToMake">The player's move</param>
        /// <returns>List of prisoners/captured stones.</returns>
        private List<int> CheckPossiblyCapturedGroups(Move moveToMake)
        {
            int currentX = moveToMake.Coordinates.X;
            int currentY = moveToMake.Coordinates.Y;
            StoneColor opponentColor = moveToMake.WhoMoves.GetOpponentColor();
            List<int> capturedGroups = new List<int>();
            int groupID;

            //check whether neighbour groups have liberty
            //right neighbour
            if (currentX < RulesetInfo.BoardSize.Width - 1 && RulesetInfo.BoardState[currentX + 1, currentY] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX + 1, currentY));
                if (RulesetInfo.GroupState.Groups[groupID].LibertyCount == 1)
                    capturedGroups.Add(groupID);
            }

            //left neighbour
            if (currentX > 0 && RulesetInfo.BoardState[currentX - 1, currentY] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX - 1, currentY));
                if (RulesetInfo.GroupState.Groups[groupID].LibertyCount == 1)
                    capturedGroups.Add(groupID);
            }

            //upper neighbour
            if (currentY < RulesetInfo.BoardSize.Height - 1 && RulesetInfo.BoardState[currentX, currentY + 1] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX, currentY + 1));
                if (RulesetInfo.GroupState.Groups[groupID].LibertyCount == 1)
                    capturedGroups.Add(groupID);
            }
            //bottom neighbour
            if (currentY > 0 && RulesetInfo.BoardState[currentX, currentY - 1] == opponentColor)
            {
                groupID = GetGroupID(new Position(currentX, currentY - 1));
                if (RulesetInfo.GroupState.Groups[groupID].LibertyCount == 1)
                    capturedGroups.Add(groupID);
            }

            return capturedGroups.Distinct().ToList();
        }

        /// <summary>
        /// Returns the ID of group.
        /// </summary>
        /// <param name="position">Position of group member.</param>
        /// <returns>The ID of group.</returns>
        private int GetGroupID(Position position)
        {
            return RulesetInfo.GroupState.GroupMap[position.X, position.Y];
        }

        /// <summary>
        /// Checks the liberty of surrounding groups.
        /// </summary>
        /// <param name="groupID">ID of group.</param>
        private bool CheckGroupLiberty(int groupID)
        {
            if (RulesetInfo.GroupState.Groups[groupID].LibertyCount != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Discovers the area from the given position and determines to which player it belongs.
        /// </summary>
        /// <param name="region">List of position which belongs to the area.</param>
        /// <param name="regionBelongsTo">Determines which player controls the discovered region.</param>
        /// <param name="pos">Starting position.</param>
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

        /// <summary>
        /// Returns the scores after adding up total territory of players.
        /// </summary>
        /// <param name="currentNode">Node of tree representing the previous move.</param>
        /// <param name="deadPositions">Positions marked as dead.</param>
        /// <returns>The score of players, which includes number of prisoners and dead stones yet.</returns>
        private Scores GetRegionScores(GameTreeNode currentNode, IEnumerable<Position> deadPositions)
        {
            RulesetInfo.SetBoard(currentNode.BoardState.BoardWithoutTheseStones(deadPositions));

            Scores scores = new Scores(0, 0);
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
    }
}
