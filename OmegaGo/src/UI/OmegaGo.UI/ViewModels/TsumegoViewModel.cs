using OmegaGo.Core;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.Services.Tsumego;

namespace OmegaGo.UI.ViewModels
{
    public class TsumegoViewModel : ViewModelBase
    {
        /*
         * In the Solve Tsumego screen, the player can select a problem from a list as on the rightmost screenshot in this section. Then, the player is presented with the problem. The player always plays as black. 

The player makes a move, then the problem either says "That move is wrong." or "That move is wrong, because your opponent would respond [this move]" or "That move is correct and you win the problem." or "Hm, alright, your opponent responds like this [move], what are you going to do now?"

Tsumego problems will be loaded from SGF files.

The program will tell the player whether they won or lost, it will keep track (and persist) information about which problems are solved and which were correctly solved the first time.

When a problem is over (either by victory or failure), the player can proceed to add stones to the problem, as though he were in analysis mode.

While solving a problem, the player can undo his moves. Undoing from the "analysis mode" back into the tsumego problem will resume the tsumego actions.

A tsumego problem will also display a problem statement (such as "Black to kill." or "Black to live." or "Black to score 10 points.")*/
        private BoardViewModel _boardViewModel;
        private BoardState _boardState;
        
        public BoardViewModel BoardViewModel
        {
            get { return _boardViewModel; }
            set { SetProperty(ref _boardViewModel, value); }
        }
        public BoardState BoardState
        {
            get { return _boardState; }
            set { SetProperty(ref _boardState, value); }
        }

        private TsumegoProblem CurrentProblem;
        private GameTreeNode CurrentProblemTree;

        private GameTreeNode _currentNode;
        public GameTreeNode CurrentNode
        {
            get { return this._currentNode; }
            set {
                SetProperty(ref _currentNode, value);
                BoardViewModel.GameTreeNode = value;
            }
        }

        private string _currentProblemName = "A";
        private string _currentProblemInstructions = "B";

        public string CurrentProblemName
        {
            get { return this._currentProblemName; }
            set { SetProperty(ref _currentProblemName, value); }
        }

        public string CurrentProblemInstructions
        {
            get { return this._currentProblemInstructions; }
            set { SetProperty(ref _currentProblemInstructions, value); }
        }

        public IMvxCommand GoToPreviousProblem => new MvxCommand(() => {
                int i = Problems.AllProblems.IndexOf(CurrentProblem);
                                                                           int prev = i - 1;
            if (prev >= 0)
            {
                LoadProblem(Problems.AllProblems[prev]);
            }
        });

        public IMvxCommand GoToNextProblem => new MvxCommand(() =>
        {
            int i = Problems.AllProblems.IndexOf(CurrentProblem);
            int next = i + 1;
            if (next < Problems.AllProblems.Count)
            {
                LoadProblem(Problems.AllProblems[next]);
            }
        });
        public TsumegoViewModel()
        {
            var problem = Mvx.GetSingleton<TsumegoProblem>();


            BoardState = new BoardState();
            BoardState.BoardHeight = 19;
            BoardState.BoardWidth = 19;


            BoardViewModel = new BoardViewModel() { BoardState = this.BoardState };
            BoardViewModel.BoardTapped += BoardViewModel_BoardTapped;


            LoadProblem(problem);
            //BoardViewModel
        }

        private void BoardViewModel_BoardTapped(object sender, Position e)
        {
            // Move place
        }

        private void LoadProblem(TsumegoProblem problem)
        {
            CurrentProblem = problem;
            this.CurrentProblemTree = CurrentProblem.SpawnThisProblem();
            this.CurrentProblemName = CurrentProblem.Name;
            this.CurrentProblemInstructions = CurrentProblemTree.Comment;
            this.CurrentNode = CurrentProblemTree;
        }
    }
}
