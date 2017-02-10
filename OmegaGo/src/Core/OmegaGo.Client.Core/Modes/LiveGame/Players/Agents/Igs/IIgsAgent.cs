using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs
{
    public interface IIgsAgent
    {
        void IncomingMoveFromServer(int moveIndex, Move move);
    }
}