using System.Collections.Generic;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Contains our current knowledge about the IGS server.
    /// </summary>
    public class IgsData
    {
        /// <summary>
        /// Gets or sets the list of games that were in progress on IGS at the time we last checked.
        /// </summary>
        public List<IgsGameInfo> GamesInProgress { get; internal set; } = new List<IgsGameInfo>();
        /// <summary>
        /// Gets or sets the list of users who were online on IGS at the time we last checked.
        /// </summary>
        public List<IgsUser> OnlineUsers { get; internal set; } = new List<IgsUser>();
    }
}