using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Kgs
{
    public class KgsGameController : RemoteGameController
    {
        public KgsGameController(
            KgsGameInfo kgsGameInfo,
            IRuleset ruleset,
            PlayerPair players, 
            IServerConnection serverConnection ) :
            base(kgsGameInfo, ruleset, players, serverConnection)
        {
            Info = kgsGameInfo;
        }

        /// <summary>
        /// KGS game info
        /// </summary>
        internal new KgsGameInfo Info { get; }

        /// <summary>
        /// KGS SGF Nodes
        /// </summary>
        internal Dictionary<int, KgsSgfNode> Nodes = new Dictionary<int, KgsSgfNode>();
    }
}
