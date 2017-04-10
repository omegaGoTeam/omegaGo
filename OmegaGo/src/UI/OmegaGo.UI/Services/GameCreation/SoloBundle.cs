using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreationBundle
{
    class SoloBundle : GameCreationBundle
    {
        public override GameCreationFormStyle Style => GameCreationFormStyle.LocalGame;
        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            gameCreationViewModel.WhitePlayer =
                gameCreationViewModel.PossiblePlayers.Last();
        }
    }
}
