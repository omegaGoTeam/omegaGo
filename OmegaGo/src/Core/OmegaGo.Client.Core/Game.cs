﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online;

namespace OmegaGo.Core
{
    /// <summary>
    /// A game instance represents a game opened in the ingame screen. It might be a game in progress, a watched game or a completed game.
    /// However, a game in Analyze Mode doesn't really need a Game instance, since it's basically just the player operating over a game tree.
    /// On the other hand, information about the game (such as the ruleset) are required in Analyze Mode.... and... well, are stored with SGF files.
    /// 
    /// TODO SGF files might possibly load into Games rather than GameTrees?
    /// 
    /// TODO It is yet to be decided whether a tsumego problem will also qualify as a Game instance. 
    /// </summary>
    public class Game
    {
        /// <summary>
        /// In ordinary games, this list will have exactly two players. If we ever add multiplayer games, this could include more players.
        /// </summary>
        public List<Player> Players;
        /// <summary>
        /// The game tree associated with the game. Each game has exactly one associated game tree.
        /// </summary>
        public GameTree GameTree;
        /// <summary>
        /// The server this game occurs on, if any.
        /// </summary>
        public ServerConnection Server;
        /// <summary>
        /// The game's identification number on the server.
        /// </summary>
        public int ServerId;
        public int NumberOfMovesPlayed;
        public int BoardSize;
        public int NumberOfHandicapStones;
        /// <summary>
        /// The komi value is the number of points added to White's score at the end of the game. We can afford to use float here, 
        /// because it can only be integers and half-integers, and in any case, the komi value is only ever added or subtracted,
        /// never multiplied or divided.
        /// </summary>
        public float KomiValue;
        public int NumberOfObservers;

        public Game()
        {
            Players = new List<Player>();
            GameTree = new GameTree();
        }

        public List<Move> PrimaryTimeline = new List<Move>();

        public void ForceMoveInHistory(int moveIndex, Move move)
        {
            while (PrimaryTimeline.Count <= moveIndex - 1)
            {
                PrimaryTimeline.Add(Move.CreateUnknownMove());
            }
            PrimaryTimeline[moveIndex - 1] = move;
            OnBoardNeedsRefreshing();
        }

        public event Action BoardNeedsRefreshing;
        private void OnBoardNeedsRefreshing()
        {
            BoardNeedsRefreshing?.Invoke();
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
    }
}