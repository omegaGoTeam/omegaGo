using MvvmCross.Core.ViewModels;
using OmegaGo.UI.Services.Files;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryViewModel : ViewModelBase
    {
        private readonly IFilePickerService _filePicker = null;

        private ObservableCollection<string> _gameList;
        private ObservableCollection<string> _gameSources;

        private int _selectedGameSourceItemIndex;

        private IMvxCommand _loadCommand;
        private IMvxCommand _loadFolderCommand;
        private IMvxCommand _deleteSelectionCommand;
        private ICommand _openFileCommand;

        public LibraryViewModel(IFilePickerService filePicker)
        {
            _filePicker = filePicker;

            _gameList = new ObservableCollection<string>()
            {
                "Progame", "Teaching game", "IGS Game #23"
            };

            _gameSources = new ObservableCollection<string>()
            {
                "Preinstalled", "Loaded", "IGS", "All", "Saved"
            };

            _selectedGameSourceItemIndex = 0;
        }

        public ObservableCollection<string> GameList => _gameList;

        public ObservableCollection<string> GameSource => _gameSources;

        public int SelectedGameSourceItemIndex
        {
            get { return _selectedGameSourceItemIndex; }
            set { SetProperty(ref _selectedGameSourceItemIndex, value); }
        }

        public IMvxCommand LoadCommand => _loadCommand ?? (_loadCommand = new MvxCommand(() => { }));
        public IMvxCommand LoadFolderCommand => _loadFolderCommand ?? (_loadFolderCommand = new MvxCommand(() => { }));
        public IMvxCommand DeleteSelectionCommand => _deleteSelectionCommand ?? (_deleteSelectionCommand = new MvxCommand(() => { }));
        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new MvxCommand(OpenFile));

        /// <summary>
        /// Opening SGF file directly
        /// </summary>
        private void OpenFile()
        {
            throw new NotImplementedException();
            ////TODO: Temporary implementation only
            //var fileContents = await _filePicker.PickAndReadFileAsync(".sgf");
            //SgfParser parser = new SgfParser();
            //var sgfCollection = parser.Parse(fileContents);
            //var gameTree = GameTreeConverter.FromSgfGameTree(sgfCollection.GameTrees.First());
            
            //ObsoleteGameInfo gameInfo = new ObsoleteGameInfo();

            //gameInfo.Players.Add(new GamePlayer("Black Player", "??", gameInfo));
            //gameInfo.Players.Add(new GamePlayer("White Player", "??", gameInfo));
            //foreach (var player in gameInfo.Players)
            //{
            //    player.Agent = new ObsoleteLocalAgent();
            //}

            //gameInfo.BoardSize = new GameBoardSize(19);
            //gameInfo.Ruleset = Ruleset.Create(RulesetType.Chinese, gameInfo.BoardSize, CountingType.Area);
            //FillBoard(gameInfo.Ruleset, gameTree, gameInfo.BoardSize);

            //ObsoleteGame game = new ObsoleteGame(gameInfo, gameInfo.GameController, null);
            //gameInfo.GameTree.GameTreeRoot = gameTree;
            //Mvx.RegisterSingleton<IObsoleteGame>(game);
            //ShowViewModel<GameViewModel>();
        }

        private void FillBoard(IRuleset ruleset, GameTreeNode rootNode, GameBoardSize boardSize)
        {
            GameBoard board = new GameBoard(boardSize);

            FillNode(ruleset, rootNode, board);
        }

        List<GameBoard> nodeHistory = new List<GameBoard>();
        GameTreeNode tmpNode;
        private void FillNode(IRuleset ruleset, GameTreeNode node, GameBoard previousBoard)
        {
            Move move = node.Move;
            
            if (move == null || move.Kind == MoveKind.None)
            {
                node.BoardState = new GameBoard(previousBoard);
            }
            else
            {
                nodeHistory.Clear();
                tmpNode = node.Parent;

                do
                {
                    if (node.Move.Kind == MoveKind.Pass || node.Move.Kind == MoveKind.PlaceStone)
                        nodeHistory.Insert(0, tmpNode.BoardState);

                    tmpNode = tmpNode.Parent;
                } while (tmpNode != null);

                var result = ruleset.ProcessMove(previousBoard, move, nodeHistory);
                node.BoardState = result.NewBoard;
            }
            
            foreach (GameTreeNode childNode in node.Branches)
            {
                FillNode(ruleset, childNode, node.BoardState);
            }
        }
    }
}
