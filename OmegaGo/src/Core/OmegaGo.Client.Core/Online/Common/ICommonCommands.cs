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

        /// <summary>
        /// Asks the server to undo all death marks in the Life/Death Determination Phase.
        /// </summary>
        /// <param name="remoteInfo">The game this command concerns.</param>
        Task UndoLifeDeath(RemoteGameInfo remoteInfo);

        /// <summary>
        /// Tells the server that we are satisfied with what stones are declared dead in the Life/Death Determination Phase.
        /// </summary>
        /// <param name="remoteInfo">The game this command concerns.</param>
        Task LifeDeathDone(RemoteGameInfo remoteInfo);

        /// <summary>
        /// Tells the server that the stone at POSITION and all stones in the same group should be marked dead.
        /// </summary>
        /// <param name="position">The position that contains a dead stone, according to the user.</param>
        /// <param name="remoteInfo">The game this command concerns.</param>
        Task LifeDeathMarkDeath(Position position, RemoteGameInfo remoteInfo);

        /// <summary>
        /// Tells the server that we wish to resign.
        /// </summary>
        /// <param name="remoteInfo">The game this command concerns.</param>
        Task Resign(RemoteGameInfo remoteInfo);

        /// <summary>
        /// Tells the server that we accept the undo request made by our opponent.
        /// </summary>
        /// <param name="remoteInfo">The game this command concerns.</param>
        Task AllowUndoAsync(RemoteGameInfo remoteInfo);

        /// <summary>
        /// Tells the server that we reject the undo request made by our opponent.
        /// </summary>
        /// <param name="remoteInfo">The game this command concerns.</param>
        Task RejectUndoAsync(RemoteGameInfo remoteInfo);

        /// <summary>
        /// Tells the server that we no longer wish to receive information about a game.
        /// </summary>
        /// <param name="remoteInfo">The game this command concerns.</param>
        Task UnobserveAsync(RemoteGameInfo remoteInfo);
    }
}
