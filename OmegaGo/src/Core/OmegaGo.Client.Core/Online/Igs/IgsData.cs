using System.Collections.Generic;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsData
    {
        private readonly IgsConnection _igsConnection;

        public List<IgsGameInfo> GamesInProgress { get; set; } = new List<IgsGameInfo>();
        public List<IgsUser> OnlineUsers { get; set; } = new List<IgsUser>();

        public IgsData(IgsConnection igsConnection)
        {
            this._igsConnection = igsConnection;
        }
    }
}