using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class GameList : KgsInterruptChannelMessage
    {
        public GameChannel[] Games { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.Data.GetChannel<KgsGameContainer>(ChannelId)?.UpdateGames(Games, connection);
        }
    }
}
