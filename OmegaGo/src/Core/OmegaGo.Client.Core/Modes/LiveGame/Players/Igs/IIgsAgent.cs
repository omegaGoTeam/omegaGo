using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
{
    public interface IIgsAgent
    {
        void IncomingMoveFromServer(int moveIndex, Move move);
    }
}