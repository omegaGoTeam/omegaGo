using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    class SoloBundle : LocalBundle
    {
        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            gameCreationViewModel.FormTitle = Localizer.Creation_NewLocalGame;
            gameCreationViewModel.WhitePlayer =
                gameCreationViewModel.PossiblePlayers.Last();
        }
    }
}
