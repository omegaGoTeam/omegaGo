using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    class HotseatBundle : LocalBundle
    {
        public override void OnLoad(GameCreationViewModel vm)
        {
            vm.FormTitle = Localizer.Creation_NewLocalGame;
            vm.BlackPlayer =
                vm.PossiblePlayers.First();
            vm.WhitePlayer =
                vm.PossiblePlayers.First();
        }
    }
}
