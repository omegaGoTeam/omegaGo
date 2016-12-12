using OmegaGo.Core;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Infrastructure;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Game;

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
        
        public TsumegoViewModel()
        {
            BoardState = new BoardState();
            BoardState.BoardHeight = 19;
            BoardState.BoardWidth = 19;

            BoardViewModel = new BoardViewModel() { BoardState = this.BoardState }; 
        }
        
    }
}
