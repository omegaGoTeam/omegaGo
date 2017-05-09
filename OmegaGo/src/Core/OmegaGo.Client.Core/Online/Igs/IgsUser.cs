using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Represents an IGS user
    /// </summary>
    public class IgsUser
    {
        /// <summary>
        /// User's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The first 7 characters of the country name or some form of abbreviation. If somebody sometime
        /// finds a list that would convert the country to an actual country name, then we can use this,
        /// until then, we can't.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// User's rank
        /// </summary>
        public string Rank { get; set; }

        /// <summary>
        /// The player rejects all incoming match requests automatically - the server doesn't even offer them to the player.
        /// </summary>
        public bool RejectsRequests { get; set; }
        
        /// <summary>
        /// The player is actively looking for a game to play. Note that not all clients support this flag.
        /// </summary>
        public bool LookingForAGame { get; set; }

        public override string ToString() => Name + " (" + Rank + ") " + (RejectsRequests ? " [not accepting requests]" : (LookingForAGame ? "[LFG]" : ""));
    }
}
