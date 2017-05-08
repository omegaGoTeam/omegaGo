using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GameContainerRemoveGame : KgsInterruptChannelMessage
    {
        public int GameId { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.Data.GetChannel<KgsGameContainer>(ChannelId)?.RemoveGame(GameId);
        }
    }
}
