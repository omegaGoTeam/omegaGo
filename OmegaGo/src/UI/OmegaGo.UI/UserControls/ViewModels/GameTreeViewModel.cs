using System;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class GameTreeViewModel : ControlViewModelBase
    {
        private GameTree _gameTree;
        private GameTreeNode _selectedGameTreeNode;
        
        /// <summary>
        /// Creates game tree view model with a given game tree.
        /// </summary>
        /// <param name="gameTree">Game tree</param>
        public GameTreeViewModel(GameTree gameTree)
        {
            GameTree = gameTree;
            GameTree.LastNodeChanged += (s, node) => OnGameTreeRedrawRequested();
        }
        
        // Indended for the UI View.
        /// <summary>
        /// Occurs when game tree representing the game changes and game tree should be redrawn.
        /// </summary>
        public event EventHandler GameTreeRedrawRequested;
        
        // Intended for the ViewModel.
        /// <summary>
        /// Occurs when the selected game tree node changes.
        /// </summary>
        internal event EventHandler<GameTreeNode> GameTreeSelectionChanged;

        /// <summary>
        /// Gets a value representing the game tree.
        /// </summary>
        public GameTree GameTree
        {
            get { return _gameTree; }
            private set { SetProperty(ref _gameTree, value); OnGameTreeRedrawRequested(); }
        }

        /// <summary>
        /// Gets a value representing the currently selected game tree node.
        /// </summary>
        public GameTreeNode SelectedGameTreeNode
        {
            get { return _selectedGameTreeNode; }
            internal set { SetProperty(ref _selectedGameTreeNode, value); OnGameTreeRedrawRequested(); }
        }

        /// <summary>
        /// Sets the provided node as the selected node. 
        /// LiveGameViewModel gets also notified about the change.
        /// </summary>
        /// <param name="node">a node to set as selected</param>
        public void SetSelectedNode(GameTreeNode node)
        {
            // Set selected node, this also rises redraw request in the UWP.UI
            SelectedGameTreeNode = node;
            // Notify the ViewModel about the change
            OnGameTreeSelectionChanged();
        }

        internal void RaiseGameTreeChanged()
        {
            OnGameTreeRedrawRequested();
        }

        private void OnGameTreeRedrawRequested()
        {
            GameTreeRedrawRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnGameTreeSelectionChanged()
        {
            GameTreeSelectionChanged?.Invoke(this, SelectedGameTreeNode);
        }
    }
}
