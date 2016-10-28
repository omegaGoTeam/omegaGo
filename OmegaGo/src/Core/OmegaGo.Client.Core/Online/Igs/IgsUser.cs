using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsUser
    {
        public string Name;
        public string Country;
        public string Rank;

        /// <summary>
        /// The player rejects all incoming match requests automatically - the server doesn't even offer them to the player.
        /// </summary>
        public bool RejectsRequests;
        
        /// <summary>
        /// The player is actively looking for a game to play. Note that not all clients support this flag.
        /// </summary>
        public bool LookingForAGame;

        public override string ToString() => Name + " (" + Rank + ")";
    }
}
