using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online
{
    /// <summary>
    /// Base class for classes that connect to online servers. The application maintains only one <see cref="ServerConnection"/> instance for each online server, and the instance includes an active TCP connection (if required for the server) and login credentials. 
    /// </summary>
    public abstract class ServerConnection
    {
        /// <summary>
        /// Gets a list of all games that are in progress at this server. The returned list is not kept by this class
        /// and may be used by the caller as the caller sees fit.
        /// </summary>
        public virtual Task<List<GameInfo>> ListGamesInProgressAsync()
        {
            return Task.FromResult(new List<GameInfo>());
        }
        public virtual void StartObserving(GameInfo game)
        {
        }
        public virtual void EndObserving(GameInfo game)
        {
        }

        /// <summary>
        /// Occurs when the connection class wants to present a log message to the user using the program.
        /// </summary>
        public event EventHandler<string> LogEvent;
        protected void OnLogEvent(string message)
        {
            LogEvent?.Invoke(this, message);
        }

        /// <summary>
        /// Gets the abbreviation of the server name, i.e. "IGS" for the Internet Go Server and "OGS" for online-go.com.
        /// </summary>
        public abstract string ShortName { get; }

        public abstract void MakeMove(GameInfo game, Move move);

        /// <summary>
        /// Gets an online game by its server identifier (the number of the game on the server).
        /// </summary>
        /// <param name="gameId">The number of the game (1 to 1000 on IGS, 1 to 1000000 on OGS, I think).</param>
        /// <returns></returns>
        public virtual Task<GameInfo> GetGameByIdAsync(int gameId)
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
