using System;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class TimelineViewModel : ControlViewModelBase
    {
        private GameTree _gameTree;
        private GameTreeNode _selectedTimelineNode;
        
        /// <summary>
        /// Creates timeline view model with a given game tree.
        /// </summary>
        /// <param name="gameTree">Game tree</param>
        public TimelineViewModel(GameTree gameTree)
        {
            GameTree = gameTree;
            GameTree.LastNodeChanged += (s, node) => OnTimelineRedrawRequested();
        }
        
        // Indended for the UI View.
        /// <summary>
        /// Occurs when game tree representing the game changes and timeline should be redrawn.
        /// </summary>
        public event EventHandler TimelineRedrawRequested;
        
        // Intended for the ViewModel.
        /// <summary>
        /// Occurs when the selected game tree node changes.
        /// </summary>
        internal event EventHandler<GameTreeNode> TimelineSelectionChanged;

        /// <summary>
        /// Gets a value representing the game tree.
        /// </summary>
        public GameTree GameTree
        {
            get { return _gameTree; }
            private set { SetProperty(ref _gameTree, value); OnTimelineRedrawRequested(); }
        }

        /// <summary>
        /// Gets a value representing the currently selected game tree node.
        /// </summary>
        public GameTreeNode SelectedTimelineNode
        {
            get { return _selectedTimelineNode; }
            internal set { SetProperty(ref _selectedTimelineNode, value); OnTimelineRedrawRequested(); }
        }

        /// <summary>
        /// Sets the provided node as the selected node. 
        /// LiveGameViewModel gets also notified about the change.
        /// </summary>
        /// <param name="node">a node to set as selected</param>
        public void SetSelectedNode(GameTreeNode node)
        {
            // Set selected node, this also rises redraw request in the UWP.UI
            SelectedTimelineNode = node;
            // Notify the ViewModel about the change
            OnTimelineSelectionChanged();
        }

        internal void RaiseGameTreeChanged()
        {
            OnTimelineRedrawRequested();
        }

        private void OnTimelineRedrawRequested()
        {
            TimelineRedrawRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnTimelineSelectionChanged()
        {
            TimelineSelectionChanged?.Invoke(this, SelectedTimelineNode);
        }
    }
}
