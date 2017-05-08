using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// Updates with more information about the game.
    /// </summary>
    public class KgsGameState : KgsInterruptChannelMessage, IGameStateMessage
    {

        public override void Process(KgsConnection connection)
        {
            KgsGame game = connection.Data.GetGame(ChannelId);
            if (game != null)
            {
                if (this.Clocks != null)
                {
                    if (this.Clocks[Role.White] != null)
                    {
                        game.Controller.Players.White.Clock.UpdateFromClock(this.Clocks[Role.White]);
                    }
                    if (this.Clocks[Role.Black] != null)
                    {
                        game.Controller.Players.Black.Clock.UpdateFromClock(this.Clocks[Role.Black]);
                    }
                }
                game.Controller.DoneId = DoneId;
                if (this.Actions != null)
                {
                    if (this.Actions.Any(action => action.Action == "SCORE"))
                    {
                        if (game.Controller.Phase.Type != Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination)
                        {
                            game.Controller.SetPhase(Modes.LiveGame.Phases.GamePhaseType.LifeDeathDetermination);
                        }
                    }
                }
                if (this.BlackDoneSent && !game.Controller.BlackDoneReceived)
                {
                    game.Controller.BlackDoneReceived = true;
                    game.Controller.KgsConnector.RaiseDoneReceived(game.Controller.Players.Black);
                }
                if (this.WhiteDoneSent && !game.Controller.WhiteDoneReceived)
                {
                    game.Controller.WhiteDoneReceived = true;
                    game.Controller.KgsConnector.RaiseDoneReceived(game.Controller.Players.White);
                }
            }
        }
        #region Flags
        // This region may be copied to other messages that make use of flags.
        /// <summary>
        /// If set, it means that the game has been scored.
        /// </summary>
        public bool Over { get; set; }
        /// <summary>
        /// The game cannot continue because the player whose turn it is has left.
        /// </summary>
        public bool Adjourned { get; set; }
        /// <summary>
        /// Only users specified by the owner are allowed in.
        /// </summary>
        public bool Private { get; set; }
        /// <summary>
        /// Only KGS Plus subscribers are allowed in.
        /// </summary>
        public bool Subscribers { get; set; }
        /// <summary>
        /// This game is a server event, and should appear at the top of game lists.
        /// </summary>
        public bool Event { get; set; }
        /// <summary>
        /// This game was created by uploading an SGF file.
        /// </summary>
        public bool Uploaded { get; set; }
        /// <summary>
        /// This game includes a live audio track.
        /// </summary>
        public bool Audio { get; set; }
        /// <summary>
        /// The game is paused. Tournament games are paused when they are first created, to give players time to join before the clocks start.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// This game has a name (most games are named after the players involved). In some cases, instead of seeing this flag when it is set, a text field name will appear instead.
        /// </summary>
        public bool Named { get; set; }
        /// <summary>
        /// This game has been saved to the KGS archives. Most games are saved automatically, but demonstration and review games must be saved by setting this flag.
        /// </summary>
        public bool Saved { get; set; }
        /// <summary>
        /// This game may appear on the open or active game lists.
        /// </summary>
        public bool Global { get; set; }
        #endregion
        #region Game state fields
        /// <summary>
        /// A list of the actions available to each player in the game. See below for the format for each action.
        /// </summary>
        public KgsGameAction[] Actions { get; set; }
        /// <summary>
        /// An object mapping role to the current state of that role's clock. See below for the format of each clock. May not be present when there are no clocks involved in the game.
        /// </summary>
        public Dictionary<string, Clock> Clocks { get; set; }
        /// <summary>
        /// The final score from the game. Only present if the game has been scored.
        /// </summary>
        public object Score { get; set; }
        /// <summary>
        /// Boolean. Indicates whether or not white has OKed the current score. Only present when scoring.
        /// </summary>
        public bool WhiteDoneSent { get; set; }
        /// <summary>
        /// Boolean. Indicates whether or not black has OKed the current score. Only present when scoring.
        /// </summary>
        public bool BlackDoneSent { get; set; }
        /// <summary>
        /// The white score. Only present during scoring.
        /// </summary>
        public object WhiteScore { get; set; }
        /// <summary>
        /// The black score. Only present during scoring.
        /// </summary>
        public object BlackScore { get; set; }
        /// <summary>
        /// The current "done ID." Each time a player changes the life and death of stones in the game, the done ID is incremented, and the "doneSent" flag for each player is cleared. When sending a GAME_SCORING_DONE message, you must include the done ID, and if it is not up to date the message will be ignored by the server.
        /// </summary>
        public int DoneId { get; set; }
        #endregion
    }

    /// <summary>
    /// An action available to a player in a KGS game.
    /// </summary>
    public class KgsGameAction
    {
        /// <summary>
        /// One of: MOVE, EDIT, SCORE, CHALLENGE_CREATE, CHALLENGE_SETUP, CHALLENGE_WAIT, CHALLENGE_ACCEPT, CHALLENGE_SUBMITTED, EDIT_DELAY.
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// The user who can perform this action. Multiple users may have the same action available.
        /// </summary>
        public User User { get; set; }
    }

    /// <summary>
    /// Information about time remaining for a single player in a KGS game.
    /// </summary>
    public class Clock
    {
        /// <summary>
        /// Boolean. If present, the clock has been paused, e.g. because the player has left the game.
        /// </summary>
        public bool Paused { get; set; }
        /// <summary>
        /// Boolean. If present, the clock is running. A clock is only running when it is the turn of the player who owns this clock.
        /// </summary>
        public bool Running { get; set; }
        /// <summary>
        /// Double. The seconds left in the current period of the clock.
        /// </summary>
        public double Time { get; set; }
        /// <summary>
        /// Only present for byo-yomi clocks. The number of periods left on the clock.
        /// </summary>
        public int PeriodsLeft { get; set; }
        /// <summary>
        /// Only present for Canadian clocks. The number of stones left in the current period.
        /// </summary>
        public int StonesLeft { get; set; }
    }
}
