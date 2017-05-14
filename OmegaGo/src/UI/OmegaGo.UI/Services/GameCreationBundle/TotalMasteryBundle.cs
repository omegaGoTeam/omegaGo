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
        public override void OnLoad(GameCreationViewModel vm)
        {
            // "Win a solo game against Fuego where Fuego is playing black and has a handicap of 3 stones or more."
            vm.FormTitle = Localizer.Creation_NewLocalGame;
            vm.BlackPlayer =
                vm.PossiblePlayers.Last();
            vm.Handicap = 3;
        }
    }
}
