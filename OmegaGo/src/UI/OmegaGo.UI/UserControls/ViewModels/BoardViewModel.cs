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
using OmegaGo.Core.Game.Tools;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class BoardViewModel : ControlViewModelBase
    {
        private BoardControlState _boardControlState;
        private GameTreeNode _gameTreeNode;

        private bool _isTouchInputOffsetEnabled;
        
        /// <summary>
        /// Initializes BoardViewModel.
        /// </summary>
        public BoardViewModel()
        {
            IsTouchInputOffsetEnabled = false;
        }

        /// <summary>
        /// Initializes BoardViewModel with given board size.
        /// </summary>
        /// <param name="boardSize">Board size</param>
        public BoardViewModel(GameBoardSize boardSize)
            : this()
        {
            BoardControlState = new BoardControlState(boardSize);
        }

        public BoardViewModel(Rectangle rectangle)
            : this()
        {
            BoardControlState = new BoardControlState(rectangle); ;
        }

        /// <summary>
        /// Occurs when the node that should be drawn is changed.
        /// </summary>
        public event EventHandler<GameTreeNode> NodeChanged;

        // This serves as a notifier for the UI, so it can tell the render service to / not to draw markups.
        // (This VM is being accessed in the UI from a draw thread - which does not allow access to DependencyProperties!)
        // TODO (future work)  Should also give actual value as well?
        //public event EventHandler<bool> MarkupRenderingChanged;

        internal event EventHandler<Position> BoardTapped;

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

        public IToolServices ToolServices
        {
            get { return _boardControlState.AnalyzeToolServices; }
            set { _boardControlState.AnalyzeToolServices = value; }
        }

        public ITool Tool
        {
            get { return _boardControlState.AnalyzeModeTool; }
            set { _boardControlState.AnalyzeModeTool = value; }
        }
        
        public bool IsMarkupDrawingEnabled
        {
            get { return _boardControlState.IsAnalyzeModeEnabled; }
            internal set { _boardControlState.IsAnalyzeModeEnabled = value; }
        }

        public bool IsShadowDrawingEnabled
        {
            get { return _boardControlState.IsShadowDrawingEnabled; }
            internal set { _boardControlState.IsShadowDrawingEnabled = value; }
        }

        public bool IsTouchInputOffsetEnabled
        {
            get { return _isTouchInputOffsetEnabled; }
            set { _isTouchInputOffsetEnabled = value; }
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
            NodeChanged?.Invoke(this, GameTreeNode);
        }
    }
}
