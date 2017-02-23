using System.Collections.Generic;
using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Downstream.Abstract
{
    internal interface IGameStateMessage : IGameFlags
    {
        /// <summary>
        /// A list of the actions available to each player in the game. See below for the format for each action.
        /// </summary>
        KgsGameAction[] Actions { get; set; }
        /// <summary>
        /// An object mapping role to the current state of that role's clock. See below for the format of each clock. May not be present when there are no clocks involved in the game.
        /// </summary>
        Dictionary<string, Clock> Clocks { get; set; }
        /// <summary>
        /// The final score from the game. Only present if the game has been scored.
        /// </summary>
        object Score { get; set; }
        /// <summary>
        /// Boolean. Indicates whether or not white has OKed the current score. Only present when scoring.
        /// </summary>
        bool WhiteDoneSent { get; set; }
        /// <summary>
        /// Boolean. Indicates whether or not black has OKed the current score. Only present when scoring.
        /// </summary>
        bool BlackDoneSent { get; set; }
        /// <summary>
        /// The white score. Only present during scoring.
        /// </summary>
        object WhiteScore { get; set; }
        /// <summary>
        /// The black score. Only present during scoring.
        /// </summary>
        object BlackScore { get; set; }
        /// <summary>
        /// The current "done ID." Each time a player changes the life and death of stones in the game, the done ID is incremented, and the "doneSent" flag for each player is cleared. When sending a GAME_SCORING_DONE message, you must include the done ID, and if it is not up to date the message will be ignored by the server.
        /// </summary>
        int DoneId { get; set; }
    }
}