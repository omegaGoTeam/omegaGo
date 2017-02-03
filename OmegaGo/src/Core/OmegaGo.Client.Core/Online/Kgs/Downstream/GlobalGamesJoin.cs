using System.Collections.Generic;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
{
    class GlobalGamesJoin : KgsInterruptChannelMessage
    {
        public string ContainerType { get; set; }
        public GameChannel[] Games { get; set; }
        public override void Process(KgsConnection connection)
        {
            // TODO
        }
    }
}