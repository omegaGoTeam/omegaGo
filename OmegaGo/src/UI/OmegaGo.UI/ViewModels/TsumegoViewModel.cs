using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Tsumego;
using OmegaGo.UI.UserControls.ViewModels;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
namespace OmegaGo.UI.ViewModels
{
    public class TsumegoViewModel : ViewModelBase
    {
        /* 
         * Description of this screen:
         * 
         
In the Solve Tsumego screen, the player can select a problem from a list as on the rightmost screenshot in this section. Then, the player is presented with the problem. The player always plays as black. 

The player makes a move, then the problem either says "That move is wrong." or "That move is wrong, because your opponent would respond [this move]" or "That move is correct and you win the problem." or "Hm, alright, your opponent responds like this [move], what are you going to do now?"

Tsumego problems will be loaded from SGF files.

The program will tell the player whether they won or lost, it will keep track (and persist) information about which problems are solved and which were correctly solved the first time.

When a problem is over (either by victory or failure), the player can proceed to add stones to the problem, as though he were in analysis mode.

While solving a problem, the player can undo his moves. Undoing from the "analysis mode" back into the tsumego problem will resume the tsumego actions.

A tsumego problem will also display a problem statement (such as "Black to kill." or "Black to live." or "Black to score 10 points.")*/

        private readonly IQuestsManager _questsManager;
        private readonly IGameSettings _gameSettings;

        private BoardViewModel _boardViewModel;

        private TsumegoProblem _currentProblem;
        private GameTreeNode _currentProblemTree;
        private StoneColor _humansColor;
        private StoneColor _playerToMove;
        private string _currentProblemName = "A";
        private string _currentProblemInstructions = "B";
        private string _currentNodeStatus = "";
        private GameTreeNode _currentNode;

        /// <summary>
        /// Creates the tsumego view model
        /// </summary>
        public TsumegoViewModel(IQuestsManager questsManager, IGameSettings gameSettings )
        {
            _questsManager = questsManager;
            _gameSettings = gameSettings;

            var problem = Mvx.GetSingleton<TsumegoProblem>();

            BoardViewModel = new BoardViewModel(new GameBoardSize(19));
            BoardViewModel.BoardTapped += BoardViewModel_BoardTapped;

            LoadProblem(problem);
        }

        public BoardViewModel BoardViewModel
        {
            get { return _boardViewModel; }
            set { SetProperty(ref _boardViewModel, value); }
        }
        
        public GameTreeNode CurrentNode
        {
            get { return _currentNode; }
            set
            {
                SetProperty(ref _currentNode, value);
                BoardViewModel.GameTreeNode = value;
            }
        }

        public string CurrentNodeStatus
        {
            get { return _currentNodeStatus; }
            set { SetProperty(ref _currentNodeStatus, value); }
        }

        public string CurrentProblemName
        {
            get { return _currentProblemName; }
            set { SetProperty(ref _currentProblemName, value); }
        }
        public string CurrentProblemInstructions
        {
            get { return _currentProblemInstructions; }
            set { SetProperty(ref _currentProblemInstructions, value); }
        }

        public bool ShowPossibleMoves
        {
            get { return _gameSettings.Tsumego.ShowPossibleMoves; }
            set
            {
                _gameSettings.Tsumego.ShowPossibleMoves = value;
                BoardViewModel.Redraw();
                RaisePropertyChanged();
            }
        }

        public void UndoOneMove()
        {
            if (CurrentNode.Parent != null)
            {
                CurrentNode = CurrentNode.Parent;
                ReachNode(CurrentNode, true);
                if (CurrentNode.Tsumego.Expected &&
                    CurrentNode.Move.WhoMoves == _humansColor &&
                    CurrentNode.Parent != null)
                {
                    UndoOneMove();
                }
            }
            else
            {
                CurrentNodeStatus = "You are at the first move.";
            }
        }

        private void BoardViewModel_BoardTapped(object sender, Position e)
        {
            Move move = Move.PlaceStone(_playerToMove, e);
            GameTreeNode thatMove =
                CurrentNode.Branches.FirstOrDefault(gtn => gtn.Move.Coordinates == e);
            if (thatMove != null)
            {
                CurrentNode = thatMove;
            }
            else
            {
                // This is a new move.
                GameTreeNode newNode = new GameTreeNode(move);
                MoveProcessingResult mpr = TsumegoProblem.TsumegoRuleset.ProcessMove(
                    new GameBoard(CurrentNode.BoardState), move, new GameBoard[0]); // TODO Petr: ko???
                if (mpr.Result == MoveResult.Legal)
                {
                    newNode.BoardState = mpr.NewBoard;
                    CurrentNode.Branches.AddNode(newNode);
                    CurrentNode = newNode;
                }
            }
            ReachNode(CurrentNode, false);
            if (CurrentNode.Tsumego.Expected && CurrentNode.Move.WhoMoves == _humansColor)
            {
                if (CurrentNode.Branches.Count(br => br.Tsumego.Expected) >= 1)
                {
                    // The opponent responds...
                    CurrentNode = CurrentNode.Branches.First(br => br.Tsumego.Expected);
                    ReachNode(CurrentNode, false);
                }
            }

        }

        private void ReachNode(GameTreeNode node, bool afterUndo)
        {
            bool mayContinue = node.Branches.Any(br => br.Tsumego.Expected);
            string status;
            if (node.Tsumego.Correct)
            {
                status = "SOLVED!";
                 var hashset = new HashSet<string>(_gameSettings.Tsumego.SolvedProblems);
                if (!hashset.Contains(_currentProblem.Name))
                {
                    _questsManager.TsumegoSolved();
                }
                 hashset.Add(_currentProblem.Name);
                _gameSettings.Tsumego.SolvedProblems = hashset;
                 

            }
            else if (node.Tsumego.Wrong)
            {
                status = "WRONG!";
            }
            else
            {
                status = "...";
            }
            if (node.Tsumego.Expected)
            {
                if (mayContinue && (node.Tsumego.Correct || node.Tsumego.Wrong))
                {
                    status += " (more moves available)";
                }
            }
            else
            {
                status += " (unexpected)";
            }
            if (afterUndo)
            {
                status += " (move undone)";
            }

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (node.Parent == null)
            {
                _playerToMove = _humansColor;
            }
            else
            {
                _playerToMove = node.Move.WhoMoves.GetOpponentColor();
            }
            CurrentNodeStatus = status;
        }

        private void LoadProblem(TsumegoProblem problem)
        {
            _currentProblem = problem;
            _currentProblemTree = _currentProblem.SpawnThisProblem();
            CurrentProblemName = _currentProblem.Name;
            CurrentProblemInstructions = _currentProblemTree.Comment;
            CurrentNode = _currentProblemTree;
            _playerToMove = _currentProblem.ColorToPlay;
            _humansColor = _playerToMove;
            CurrentNodeStatus = _humansColor + " to play.";
        }



        // Navigation
        public IMvxCommand GoToPreviousProblem => new MvxCommand(() =>
        {
            int i = Problems.AllProblems.IndexOf(_currentProblem);
            int prev = i - 1;
            if (prev >= 0)
            {
                LoadProblem(Problems.AllProblems[prev]);
            }
        });
        public IMvxCommand GoToNextProblem => new MvxCommand(() =>
        {
            int i = Problems.AllProblems.IndexOf(_currentProblem);
            int next = i + 1;
            if (next < Problems.AllProblems.Count)
            {
                LoadProblem(Problems.AllProblems[next]);
            }
        });
        public IMvxCommand UndoOneMoveCommand => new MvxCommand(UndoOneMove);




    }
}
