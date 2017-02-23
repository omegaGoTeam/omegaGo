using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs
{
    public interface IIgsAgent
    {
        void MoveFromServer(int moveIndex, Move move);
    }
}