using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    class TotalMasteryBundle : LocalBundle
    {
        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            // "Win a solo game against Fuego where Fuego is playing black and has a handicap of 3 stones or more."
            gameCreationViewModel.FormTitle = Localizer.Creation_NewLocalGame;
            gameCreationViewModel.BlackPlayer =
                gameCreationViewModel.PossiblePlayers.Last();
            gameCreationViewModel.Handicap = 3;
        }
    }
}
