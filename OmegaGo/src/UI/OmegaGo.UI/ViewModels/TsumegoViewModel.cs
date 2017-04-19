using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Tsumego;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.Utility;

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
In the Solve Tsumego screen, the player can select a problem from a list as on the rightmost screenshot in this section. 
Then, the player is presented with the problem. The player always plays as black. 

The player makes a move, then the problem either says "That move is wrong." or 
"That move is wrong, because your opponent would respond [this move]" or
"That move is correct and you win the problem." or 
"Hm, alright, your opponent responds like this [move], what are you going to do now?"

Tsumego problems will be loaded from SGF files.

The program will tell the player whether they won or lost, 
it will keep track (and persist) information about which problems are solved and which were correctly solved the first time.

When a problem is over (either by victory or failure), 
the player can proceed to add stones to the problem, as though he were in analysis mode.

While solving a problem, the player can undo his moves. 
Undoing from the "analysis mode" back into the tsumego problem will resume the tsumego actions.

A tsumego problem will also display a problem statement
(such as "Black to kill." or "Black to live." or "Black to score 10 points.")*/

        private readonly IQuestsManager _questsManager;
        private readonly IGameSettings _gameSettings;
        private readonly ITsumegoProblemsLoader _problemsLoader;

        private BoardViewModel _boardViewModel;

        private TsumegoProblem _currentProblem;
        private GameTreeNode _currentProblemTree;
        private StoneColor _humansColor;
        private StoneColor _playerToMove;
        private string _currentProblemName = "A";
        private string _currentProblemInstructions = "B";
        private string _currentNodeStatus = "";
        private bool _correctVisible;
        private bool _wrongVisible;
        private GameTreeNode _currentNode;

        private List<TsumegoProblemInfo> _allProblems = null;

        //commands
        private IMvxCommand _goToNextProblemCommand;
        private IMvxCommand _goToPreviousProblemCommand;
        private IMvxCommand _undoOneMoveCommand;

        /// <summary>
        /// Creates the tsumego view model
        /// </summary>
        public TsumegoViewModel(IQuestsManager questsManager, IGameSettings gameSettings, ITsumegoProblemsLoader problemsLoader)
        {
            _questsManager = questsManager;
            _gameSettings = gameSettings;
            _problemsLoader = problemsLoader;
            var problem = Mvx.Resolve<TsumegoProblem>();
            Rectangle rectangle = problem.GetBoundingBoard();
            BoardViewModel = new BoardViewModel(rectangle);
            BoardViewModel.BoardTapped += BoardViewModel_BoardTapped;

            LoadProblem(problem);
        }

        /// <summary>
        /// Goes to the previous problem in list
        /// </summary>
        public IMvxCommand GoToPreviousProblemCommand => _goToPreviousProblemCommand ??
            (_goToPreviousProblemCommand = new MvxAsyncCommand(GoToPreviousProblemAsync, CanGoToPreviousProblem));


        /// <summary>
        /// Goes to the next problem in list
        /// </summary>
        public IMvxCommand GoToNextProblemCommand => _goToNextProblemCommand ?? (_goToNextProblemCommand = new MvxAsyncCommand(GoToNextProblemAsync, CanGoToNextProblem));

        /// <summary>
        /// Undoes one move        
        /// </summary>
        public IMvxCommand UndoOneMoveCommand => _undoOneMoveCommand ??
                                                 (_undoOneMoveCommand = new MvxCommand(UndoOneMove, CanUndoOneMove));


        public string CurrentProblemPermanentlySolved =>
            this._gameSettings.Tsumego.SolvedProblems.Contains(this.CurrentProblemName)
                ? (_currentNode.Tsumego.Correct
                    ? Localizer.Tsumego_YouHaveSolvedThisProblem
                    : Localizer.Tsumego_YouHavePreviouslySolvedThisProblem)
                : Localizer.Tsumego_NotYetSolved;

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

        public bool CorrectVisible
        {
            get { return _correctVisible; }
            set { SetProperty(ref _correctVisible, value); }
        }

        public bool WrongVisible
        {
            get { return _wrongVisible; }
            set { SetProperty(ref _wrongVisible, value); }
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

        public async void Init()
        {
            _allProblems = new List<TsumegoProblemInfo>(await _problemsLoader.GetProblemListAsync());
            GoToPreviousProblemCommand.RaiseCanExecuteChanged();
            GoToNextProblemCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Shows the problem name in the tab title
        /// </summary>
        public override void Appearing()
        {
            TabTitle = CurrentProblemName;
        }

        /// <summary>
        /// Goes backward in the problem list
        /// </summary>
        private async Task GoToPreviousProblemAsync()
        {
            int i = _allProblems.IndexOf(_allProblems.FirstOrDefault(p => p.Name == _currentProblem.Name));
            int prev = i - 1;
            if (prev >= 0)
            {
                LoadProblem(await _problemsLoader.GetProblemAsync(_allProblems[prev]));
            }
        }

        /// <summary>
        /// Checks if we can go backward in the problem list
        /// </summary>
        /// <returns>Previous problem available?</returns>
        private bool CanGoToPreviousProblem()
        {
            if (_allProblems != null)
            {
                int i = _allProblems.IndexOf(_allProblems.FirstOrDefault(p => p.Name == _currentProblem.Name));
                int prev = i - 1;
                return prev >= 0;
            }
            return false;
        }

        /// <summary>
        /// Goes forward in the problem list
        /// </summary>
        private async Task GoToNextProblemAsync()
        {
            int i = _allProblems.IndexOf(_allProblems.FirstOrDefault(p => p.Name == _currentProblem.Name));
            int next = i + 1;
            if (next < _allProblems.Count)
            {
                LoadProblem(await _problemsLoader.GetProblemAsync(_allProblems[next]));
            }
        }

        /// <summary>
        /// Checks if we can go forward in the problem list
        /// </summary>
        /// <returns>Next problem available?</returns>
        private bool CanGoToNextProblem()
        {
            if (_allProblems != null)
            {
                int index = _allProblems.IndexOf(_allProblems.FirstOrDefault(p => p.Name == _currentProblem.Name));
                int next = index + 1;
                return (next < _allProblems.Count);
            }
            return false;
        }

        /// <summary>
        /// Undoes one move
        /// </summary>
        private void UndoOneMove()
        {
            if (CurrentNode.Parent != null)
            {
                CurrentNode = CurrentNode.Parent;
                ReachNode(CurrentNode);
                if (CurrentNode.Tsumego.Expected &&
                    CurrentNode.Move.WhoMoves == _humansColor &&
                    CurrentNode.Parent != null)
                {
                    UndoOneMove();
                }
            }
        }

        /// <summary>
        /// Checks if a move can be undone
        /// </summary>
        /// <returns></returns>
        private bool CanUndoOneMove() => CurrentNode?.Parent != null;

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
                MoveProcessingResult mpr = TsumegoProblem.TsumegoRuleset.ProcessMove(CurrentNode, move); // TODO Petr: ko???

                // Note that we're not handling ko. Most or all of our problems don't depend on ko, 
                // and which positions are correct or wrong is written in the .sgf file anyway, so this is not a big deal.

                if (mpr.Result == MoveResult.Legal)
                {
                    newNode.BoardState = mpr.NewBoard;
                    newNode.GroupState = mpr.NewGroupState;
                    CurrentNode.Branches.AddNode(newNode);
                    if (CurrentNode.Tsumego.Correct)
                    {
                        newNode.Tsumego.Correct = true;
                    }
                    else
                    {
                        newNode.Tsumego.Wrong = true;
                    }
                    CurrentNode = newNode;
                }
            }
            ReachNode(CurrentNode);
            if (CurrentNode.Tsumego.Expected && CurrentNode.Move.WhoMoves == _humansColor)
            {
                if (CurrentNode.Branches.Count(br => br.Tsumego.Expected) >= 1)
                {
                    // The opponent responds...
                    CurrentNode = CurrentNode.Branches.First(br => br.Tsumego.Expected);
                    ReachNode(CurrentNode);
                }
            }

        }

        private void ReachNode(GameTreeNode node)
        {
            bool mayContinue = node.Branches.Any(br => br.Tsumego.Expected);
            string status;
            if (node.Tsumego.Correct)
            {
                status = Localizer.Tsumego_StatusCorrect;
                var hashset = new HashSet<string>(_gameSettings.Tsumego.SolvedProblems);
                if (!hashset.Contains(_currentProblem.Name))
                {
                    _questsManager.TsumegoSolved();
                }
                hashset.Add(_currentProblem.Name);
                _gameSettings.Tsumego.SolvedProblems = hashset;
                WrongVisible = false;
                CorrectVisible = true;
            }
            else if (node.Tsumego.Wrong)
            {
                status = Localizer.Tsumego_StatusWrong;
                WrongVisible = true;
                CorrectVisible = false;
            }
            else
            {
                status = Localizer.Tsumego_StatusContinue;
                WrongVisible = false;
                CorrectVisible = false;
            }
            if (node.Parent == null)
            {
                if (_humansColor == StoneColor.Black)
                {
                    status = Localizer.Tsumego_BlackToPlay;
                }
                else
                {
                    status = Localizer.Tsumego_WhiteToPlay;
                }
            }
            if (node.Tsumego.Expected)
            {
                if (mayContinue && (node.Tsumego.Correct || node.Tsumego.Wrong))
                {
                    status += "\n" + Localizer.Tsumego_MoreMovesAvailable;
                }
            }
            else
            {
                if (!CorrectVisible)
                {
                    WrongVisible = true;
                }
                status += "\n" + Localizer.Tsumego_Unexpected;
            }
            /*
             // We'll see what users say, but this is probably unnecessary:
            if (afterUndo)
            {
                status += "\n(move undone)";
            }
            */

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
            RaisePropertyChanged(nameof(CurrentProblemPermanentlySolved));
            RaisePropertyChanged(nameof(UndoOneMoveCommand));
        }

        private void LoadProblem(TsumegoProblem problem)
        {
            Rectangle rect = problem.GetBoundingBoard();
            BoardViewModel.BoardControlState.OriginX = rect.X;
            BoardViewModel.BoardControlState.OriginY = rect.Y;
            BoardViewModel.BoardControlState.BoardWidth = rect.Width;
            BoardViewModel.BoardControlState.BoardHeight = rect.Height;
            _currentProblem = problem;
            _currentProblemTree = _currentProblem.SpawnThisProblem();
            CurrentProblemName = _currentProblem.Name;
            CurrentProblemInstructions = _currentProblemTree.Comment;
            CurrentNode = _currentProblemTree;
            _playerToMove = _currentProblem.ColorToPlay;
            _humansColor = _playerToMove;
            if (_humansColor == StoneColor.Black)
            {
                CurrentNodeStatus = Localizer.Tsumego_BlackToPlay;
            }
            else
            {
                CurrentNodeStatus = Localizer.Tsumego_WhiteToPlay;
            }
            WrongVisible = false;
            CorrectVisible = false;
            GoToPreviousProblemCommand.RaiseCanExecuteChanged();
            GoToNextProblemCommand.RaiseCanExecuteChanged();
            UndoOneMoveCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(CurrentProblemPermanentlySolved));
        }
    }
}
