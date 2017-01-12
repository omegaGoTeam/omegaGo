using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    internal class GameController : IGameController
    {
        public GamePlayer TurnPlayer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler BoardMustBeRefreshed;

        public void BeginGame()
        {
            throw new NotImplementedException();
        }

        public void RespondRequest()
        {
            throw new NotImplementedException();
        }
    }
}
