using MvvmCross.Core.ViewModels;
using OmegaGo.Core;
using OmegaGo.UI.Services.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.UserControls.ViewModels
{
    // TODO It would be nice to have a way how to choose selected-highlighted tile
    public sealed class BoardViewModel : MvxViewModel
    {
        private BoardState _boardState;

        private GameTreeNode _gameTreeNode;

        public GameTreeNode GameTreeNode
        {
            get { return _gameTreeNode; }
            set { SetProperty(ref _gameTreeNode, value); OnBoardChanged(); }
        }

        public BoardState BoardState
        {
            get { return _boardState; }
            set { SetProperty(ref _boardState, value); OnBoardChanged(); }
        }

        public event EventHandler<GameTreeNode> BoardRedrawRequsted;
        internal event EventHandler<Position> BoardTapped;

        public BoardViewModel()
        {

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
            BoardRedrawRequsted?.Invoke(this, GameTreeNode);
        }
    }
}
