using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.GameCreation
{
    /// <summary>
    /// Determines what controls are relevant in the game setup form.
    /// </summary>
    public enum GameCreationFormStyle
    {
        /// <summary>
        /// The form creates a local game (hotseat or solo).
        /// </summary>
        LocalGame,
        /// <summary>
        /// The form creates an IGS match request.
        /// </summary>
        OutgoingIgs,
        /// <summary>
        /// The form lists information about an incoming IGS match request
        /// </summary>
        IncomingIgs,
        /// <summary>
        /// The form creates a public KGS challenge.
        /// </summary>
        KgsChallengeCreation,
        /// <summary>
        /// The form lists information about any KGS challenge (ours or another's).
        /// </summary>
        KgsChallengeNegotiation
    }
}
