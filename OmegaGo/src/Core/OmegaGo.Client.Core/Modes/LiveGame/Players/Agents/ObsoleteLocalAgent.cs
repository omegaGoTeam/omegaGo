using System;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// Represents the agent of a human player playing on this device. This agent works for hotseat, playing against AI and for internet play.
    /// </summary>
    /// <seealso cref="ObsoleteAgentBase" />
    public class ObsoleteLocalAgent : ObsoleteAgentBase, IReceiverOfLocalActions
    {
        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;

        public override void PleaseMakeAMove()
        {
            OnPleaseMakeAMove?.Invoke(this, Player);
        }

        public void Click(StoneColor color, Position selectedPosition)
        {
            Game.GameController.MakeMove(Player, Move.PlaceStone(color, selectedPosition));

        }
        public void ForcePass(StoneColor color)
        {
            Game.GameController.MakeMove(Player, Move.Pass(color));
        }

        /// <summary>
        /// Occurs when the game controller requests this agent's PLAYER to make a move.
        /// </summary>
        public event EventHandler<GamePlayer> OnPleaseMakeAMove ;
    }
}
