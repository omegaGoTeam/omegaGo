using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.Online;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core
{
    /// <summary>
    /// A game instance represents a game opened in the ingame screen. It might be a game in progress, a watched game or a completed game.
    /// However, a game in Analyze Mode doesn't really need a Game instance, since it's basically just the player operating over a game tree.
    /// On the other hand, information about the game (such as the ruleset) are required in Analyze Mode.... and... well, are stored with SGF files.
    /// 
    /// TODO It is yet to be decided whether a tsumego problem and other things will also qualify as a Game instance. 
    /// </summary>
    public class GameInfo
    {
        /// <summary>
        /// In ordinary games, this list will have exactly two players. If we ever add multiplayer games, this could include more players.
        /// The first player is black. The second player is white.
        /// </summary>
        public List<Player> Players { get; set; }
        /// <summary>
        /// Gets the player playing black stones, i.e. "Players[0]".
        /// </summary>
        public Player Black => Players[0];
        /// <summary>
        /// Gets the player playing white stones, i.e. "Players[1]".
        /// </summary>
        public Player White => Players[1];
        /// <summary>
        /// The game tree associated with the game. Each game has exactly one associated game tree.
        /// </summary>
        public readonly GameTree GameTree;
        /// <summary>
        /// The server this game occurs on, if any.
        /// </summary>
        public ServerConnection Server { get; set; }
        /// <summary>
        /// The game's identification number on the server.
        /// </summary>
        public int ServerId { get; set; }
        /// <summary>
        /// The number of moves that have been played. At the beginning, this is zero. Handicap moves do not count.
        /// TODO maybe make handicap moves count
        /// </summary>
        public int NumberOfMovesPlayed { get; set; }
        /// <summary>
        /// The size of the game board. Cannot change during the game. Initialized during game creation.
        /// </summary>
        public GameBoardSize BoardSize { get; set; }
        /// <summary>
        /// Gets or sets the size of the board, if it is square.
        /// If the board is not square, calling this getter will throw an exception.
        /// </summary>
        public int SquareBoardSize
        {
            get
            {
                if (BoardSize.IsSquare)
                {
                    return BoardSize.Width;
                }
                throw new InvalidOperationException("The board of this game is not square.");
            }
            set
            {
                BoardSize = new GameBoardSize(value);
            }
        }
        /// <summary>
        /// Gets or sets the ruleset that governs this game. In the future, this should never be null. For now, we're prototyping.
        /// </summary>
        public IRuleset Ruleset { get; set; }
        public int NumberOfHandicapStones { get; set; }
        /// <summary>
        /// The komi value is the number of points added to White's score at the end of the game. We can afford to use float here, 
        /// because it can only be integers and half-integers, and in any case, the komi value is only ever added or subtracted,
        /// never multiplied or divided.
        /// </summary>
        public float KomiValue { get; set; }
        public int NumberOfObservers { get; set; }
        public GameController GameController { get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="GameInfo"/> class. A <see cref="OmegaGo.Core.GameController" /> is also created for this game and 
        /// initialized.
        /// </summary>
        public GameInfo()
        {
            Players = new List<Player>();
            GameTree = new GameTree();
            GameController = new Core.GameController(this);
        }

        /// <summary>
        /// Gets the moves made during this game, starting with the first, ending with the last move made. If this game's game tree
        /// is branching, then an exception triggers. If there is no move made yet, nothign is returned. "PASS" moves are also returned.
        /// </summary>
        public IEnumerable<Move> PrimaryTimeline
        {
            get
            {
                GameTreeNode node = this.GameTree.GameTreeRoot;
                while (node != null)
                {
                    yield return node.Move;
                    node = node.NextMove;
                }
            }
        }

        /// <summary>
        /// This is called when we receive a new move from an internet server. This method will remember the move and make sure it's played at the 
        /// appropriate time.
        /// </summary>
        /// <param name="moveIndex">0-based index of the move.</param>
        /// <param name="move">The move.</param>
        public void AcceptMoveFromInternet(int moveIndex, Move move)
        {
            Player player = GetPlayerByColor(move.WhoMoves);
            IOnlineAgent agent = player.Agent as IOnlineAgent;
            agent?.ForceHistoricMove(moveIndex, move);
        }

        private Player GetPlayerByColor(StoneColor color)
        {
            switch (color)
            {
                case StoneColor.Black: return Players[0];
                case StoneColor.White: return Players[1];
                default: throw new ArgumentException("Only Black and White may play.", nameof(color));
            }
        }
        
        /// <summary>
        /// Tells the online connection to give us information about the current game state and to push new moves as they happen.
        /// </summary>
        /// <exception cref="InvalidOperationException">This game is not an online game.</exception>
        public void StartObserving()
        {
            if (Server == null) throw new InvalidOperationException("This game is not an online game.");
            Server.StartObserving(this);
        }

        /// <summary>
        /// Returns the opponent of the specified player in this game.
        /// </summary>
        /// <param name="player">The player whose opponent we wish to learn about.</param>
        public Player OpponentOf(Player player)
        {
            if (player == Players[0])
                return Players[1];
            else if (player == Players[1])
                return Players[0];
            else
                throw new ArgumentException("The specified player does not belong to this game.", nameof(player));
        }

        /// <summary>
        /// Tells the online connection to stop pushing information about the game.
        /// </summary>
        /// <exception cref="InvalidOperationException">This game is not an online game.</exception>
        public void StopObserving()
        {
            if (Server == null) throw new InvalidOperationException("This game is not an online game.");
            Server.EndObserving(this);
        }

        public override string ToString()
        {
            return "[" + Server.ShortName + " " + ServerId + "] " + Players[0].Name + " vs. " + Players[1].Name + " (" + NumberOfObservers + " observers)";
        }

        public async Task AbsorbAdditionalInformation()
        {
            if (this.Server == null)
                throw new InvalidOperationException("Only online games can absorb additional information.");
            GameInfo moreInformation = await this.Server.GetGameById(this.ServerId);
            this.CopyInformationFrom(moreInformation);
        }

        private void CopyInformationFrom(GameInfo moreInformation)
        {
            this.BoardSize = moreInformation.BoardSize; // TODO add time limit later on
            this.Ruleset = new JapaneseRuleset(this.BoardSize);
        }
    }
}
