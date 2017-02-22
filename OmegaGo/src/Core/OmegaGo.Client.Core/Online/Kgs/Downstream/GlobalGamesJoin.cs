using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GlobalGamesJoin : KgsInterruptChannelMessage
    {
        public string ContainerType { get; set; }
        public GameChannel[] Games { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.Data.JoinGlobalChannel(ChannelId, ContainerType);
            foreach (var channel in Games)
            {
                connection.Data.Containers[ChannelId].AddGame(channel, connection);
            }
        }
    }
}