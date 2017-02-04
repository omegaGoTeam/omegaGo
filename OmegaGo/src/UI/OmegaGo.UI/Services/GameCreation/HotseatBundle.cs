using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreationBundle
{
    class HotseatBundle : GameCreationBundle
    {
        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            gameCreationViewModel.BlackPlayer =
                gameCreationViewModel.PossiblePlayers.First();
            gameCreationViewModel.WhitePlayer =
                gameCreationViewModel.PossiblePlayers.First();
        }
    }
}
