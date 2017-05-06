using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

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
                connection.Data.GetChannel<KgsGameContainer>(ChannelId)?.AddChannel(channel, connection);
            }
        }
    }
}