using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.ViewModels.Tutorial;

namespace OmegaGo.UI.ViewModels
{
    public class TutorialViewModel : ViewModelBase, INotifyPropertyChanged
    {

        public Scenario Scenario { get; }


        public BoardViewModel BoardViewModel { get; }
        private BoardState _boardState;

        

        public TutorialViewModel()
        {
            this._boardState = new BoardState();
            this._boardState.BoardHeight = 9;
            this._boardState.BoardWidth = 9;
            BoardViewModel = new BoardViewModel() { BoardState = this._boardState }; // Mindfuck inception o.O
            Scenario = new BeginnerScenario();
            Scenario.GameTreeNodeChanged += Scenario_GameTreeNodeChanged;
            Scenario.ShiningPositionChanged += Scenario_ShiningPositionChanged;

        }

        private void Scenario_ShiningPositionChanged(object sender, Core.Position e)
        {
            this._boardState.ShiningPosition = e;
        }

        private void Scenario_GameTreeNodeChanged(object sender, Core.GameTreeNode e)
        {
           BoardViewModel.GameTreeNode = e;
        }

        public void TapBoardControl()
        {
            Scenario.ClickPosition(this._boardState.SelectedPosition);
        }
    }
}
