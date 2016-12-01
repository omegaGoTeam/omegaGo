﻿using System;
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
        public Scenario Scenario { get; set; }


        public BoardViewModel BoardViewModel { get; set; }
        private BoardState BoardState;

        

        public TutorialViewModel()
        {
            BoardState = new BoardState();
            BoardState.BoardHeight = 9;
            BoardState.BoardWidth = 9;
            BoardState.PropertyChanged += BoardState_PropertyChanged;
            BoardViewModel = new BoardViewModel() { BoardState = BoardState }; // Mindfuck inception o.O
            Scenario = new BeginnerScenario();
            Scenario.GameTreeNodeChanged += Scenario_GameTreeNodeChanged;

        }

        private void Scenario_GameTreeNodeChanged(object sender, Core.GameTreeNode e)
        {
           BoardViewModel.GameTreeNode = e;
        }

        private void BoardState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BoardState.SelectedPosition))
            {
                Scenario.ClickPosition(BoardState.SelectedPosition);
            }
        }
    }
}
