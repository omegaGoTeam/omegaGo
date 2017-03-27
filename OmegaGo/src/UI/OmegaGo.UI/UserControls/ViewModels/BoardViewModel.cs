using MvvmCross.Core.ViewModels;
using OmegaGo.Core;
using OmegaGo.UI.Services.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.UI.Utility;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class BoardViewModel : ControlViewModelBase
    {
        private BoardControlState _boardControlState;

        private GameTreeNode _gameTreeNode;

        public GameTreeNode GameTreeNode
        {
            get { return _gameTreeNode; }
            set { SetProperty(ref _gameTreeNode, value); OnBoardChanged(); }
        }

        public BoardControlState BoardControlState
        {
            get { return _boardControlState; }
            set { SetProperty(ref _boardControlState, value); OnBoardChanged(); }
        }

        public event EventHandler<GameTreeNode> BoardRedrawRequested;

        internal event EventHandler<Position> BoardTapped;

        public BoardViewModel()
        {

        }

        /// <summary>
        /// Create board view model with give board size
        /// </summary>
        /// <param name="boardSize">Board size</param>
        public BoardViewModel(GameBoardSize boardSize)
        {
            BoardControlState = new BoardControlState( boardSize ); ;
        }
        public BoardViewModel(Rectangle rectangle)
        {
            BoardControlState = new BoardControlState(rectangle); ;
        }

        public void BoardTap(Position position)
        {
            BoardTapped?.Invoke(this, position);
        }

        public void Redraw()
        {
            OnBoardChanged();
        }

        private void OnBoardChanged()
        {
            BoardRedrawRequested?.Invoke(this, GameTreeNode);
        }
    }
}
