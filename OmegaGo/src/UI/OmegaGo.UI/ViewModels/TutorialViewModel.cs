using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.ViewModels.Tutorial;

namespace OmegaGo.UI.ViewModels
{
    public class TutorialViewModel : ViewModelBase, INotifyPropertyChanged
    {

        public Scenario Scenario { get; }


        public BoardViewModel BoardViewModel { get; }

        public TutorialViewModel()
        {
            BoardViewModel = new BoardViewModel(new GameBoardSize(9));
            Scenario = new BeginnerScenario();
            Scenario.GameTreeNodeChanged += Scenario_GameTreeNodeChanged;
            Scenario.ShiningPositionChanged += Scenario_ShiningPositionChanged;

        }

        private void Scenario_ShiningPositionChanged(object sender, Position e)
        {
            BoardViewModel.BoardControlState.ShiningPosition = e;
        }

        private void Scenario_GameTreeNodeChanged(object sender, GameTreeNode e)
        {
            BoardViewModel.GameTreeNode = e;
        }

        public void TapBoardControl()
        {
            Scenario.ClickPosition(BoardViewModel.BoardControlState.MouseOverPosition);
        }
    }
}
