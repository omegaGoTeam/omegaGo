﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online
{
    public abstract class ServerConnection
    {

        /*
        /// <summary>
        /// Attempts to log in the user to the server associated with this class. If is succeeds, the class instance remembers the login data and establishes 
        /// the connection.
        /// </summary>
        /// <param name="username">User's login name</param>
        /// <param name="password">User's password.</param>
        /// <returns>True if the server permitted the login.</returns>
        public abstract bool Login(string username, string password);
        */
        public virtual Task<List<GameInfo>> ListGamesInProgress()
        {
            return Task.FromResult(new List<GameInfo>());
        }
        public virtual void StartObserving(GameInfo game)
        {
        }
        public virtual void EndObserving(GameInfo game)
        {
        }

        public event Action<GameInfo, Move> IncomingMove;
        protected void OnIncomingMove(GameInfo game, Move move)
        {
            IncomingMove?.Invoke(game, move);
        }
         


        /// <summary>
        /// Sends a log message that should be displayed to the user using the program.
        /// </summary>
        public event Action<string> LogEvent;

        protected void OnLogEvent(string message)
        {
            LogEvent?.Invoke(message);
        }

        /// <summary>
        /// Gets the abbreviation of the server name, i.e. "IGS" for the Internet Go Server and "OGS" for online-go.com.
        /// </summary>
        public abstract string ShortName { get; }

        public abstract Task MakeMove(GameInfo game, Move move);

        public virtual Task<GameInfo> GetGameById(int gameId)
        {
            throw new NotImplementedException();
        }

        public virtual void Resign(GameInfo game)
        {
            throw new NotImplementedException();
        }

        public virtual void LifeDeath_MarkDead(Position position, GameInfo game)
        {
            throw new NotImplementedException();
        }

        public virtual void LifeDeath_Done(GameInfo game)
        {
            throw new NotImplementedException();
        }

        public virtual void LifeDeath_Undo(GameInfo game)
        {
            throw new NotImplementedException();
        }
    }
}
