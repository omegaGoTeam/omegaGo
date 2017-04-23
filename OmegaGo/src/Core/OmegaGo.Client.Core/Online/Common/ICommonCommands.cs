using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Common
{
    /// <summary>
    /// Exposes commands launched from our client to an online server. These commands are available
    /// at all online servers. Implementors of this interface might need to cast method parameters
    /// in order to get the information they need.
    /// </summary>
    public interface ICommonCommands
    {
        /// <summary>
        /// Tells the server that we wish to make a move in a game. If the placing of the move fails, 
        /// this will be reported by an event.
        /// </summary>
        /// <param name="remoteInfo">The game where we wish to make a move.</param>
        /// <param name="move">The move that we wish to make.</param>
        Task MakeMove(RemoteGameInfo remoteInfo, Move move);

        /// <summary>
        /// Tells the server to add additional time to our opponent's clock.
        /// </summary>
        /// <param name="remoteInfo">The game where we wish to add time.</param>
        /// <param name="additionalTime">The time to add to our opponent's clock.</param>
        /// <returns></returns>
        Task AddTime(RemoteGameInfo remoteInfo, TimeSpan additionalTime);

        Task UndoLifeDeath(RemoteGameInfo remoteInfo);

        Task LifeDeathDone(RemoteGameInfo remoteInfo);

        Task LifeDeathMarkDeath(Position position, RemoteGameInfo remoteInfo);

        Task Resign(RemoteGameInfo remoteInfo);
        
        Task AllowUndoAsync(RemoteGameInfo remoteInfo);

        Task RejectUndoAsync(RemoteGameInfo remoteInfo);        
    }
}
