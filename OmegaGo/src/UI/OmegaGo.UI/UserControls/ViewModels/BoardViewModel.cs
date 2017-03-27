﻿using MvvmCross.Core.ViewModels;
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
    // TODO Vita : It would be nice to have a way how to choose selected-highlighted tile
    public sealed class BoardViewModel : ControlViewModelBase
    {
        private BoardControlState _boardControlState;

        private GameTreeNode _gameTreeNode;

        // TODO Is this the correct location for this?
        private bool _isMarkupDrawingEnabled;
        
        public BoardControlState BoardControlState
        {
            get { return _boardControlState; }
            set { SetProperty(ref _boardControlState, value); OnBoardChanged(); }
        }

        public GameTreeNode GameTreeNode
        {
            get { return _gameTreeNode; }
            set { SetProperty(ref _gameTreeNode, value); OnBoardChanged(); }
        }

        // TODO Is this the correct location for this?
        public bool IsMarkupDrawingEnabled
        {
            get { return _isMarkupDrawingEnabled; }
            set
            {
                SetProperty(ref _isMarkupDrawingEnabled, value);
                MarkupSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<GameTreeNode> BoardRedrawRequested;

        // This serves as a notifier for the UI, so it can tell the render service to / not to draw markups.
        // (This VM is being accessed in the UI from a draw thread - which does not allow access to DependencyProperties!)
        // TODO Should also give actual value as well?
        public event EventHandler MarkupSettingsChanged;

        internal event EventHandler<Position> BoardTapped;

        public BoardViewModel()
        {
            _isMarkupDrawingEnabled = false;
        }

        /// <summary>
        /// Create board view model with give board size
        /// </summary>
        /// <param name="boardSize">Board size</param>
        public BoardViewModel(GameBoardSize boardSize)
        {
            BoardControlState = new BoardControlState(boardSize);
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
