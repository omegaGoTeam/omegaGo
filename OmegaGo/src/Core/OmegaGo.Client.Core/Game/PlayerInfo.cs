using OmegaGo.Core.Modes.LiveGame.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game
{
    public class PlayerInfo
    {
        public PlayerInfo(StoneColor color, string name, string rank)
        {
            Name = name ?? "";
            Rank = rank?.Trim() ?? "";
            Color = color;
        }
        
        /// <summary>
        /// Gets the name of the player. This could be a player's online nickname, an AI program's name and difficulty,
        /// or the text "Local Black" or "Local White".
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the rank of the player. There should be no whitespace. The rank may be arbitrary otherwise: NR, 17k, 6d+, 5p? etc.
        /// </summary>
        public string Rank { get; }

        /// <summary>
        /// Player's stone color
        /// </summary>
        public StoneColor Color { get; }

        public override string ToString()
        {
            return Name + "(" + Rank + ")";
        }
    }
}
