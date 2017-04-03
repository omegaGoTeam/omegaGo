using System.Linq;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    /// <summary>
    /// TODO: Replace this logic with MvvmCross bundles
    /// </summary>
    class HotseatBundle : GameCreationBundle.GameCreationBundle
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
