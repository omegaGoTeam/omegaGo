using MvvmCross.Core.ViewModels;
using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class TimelineViewModel : ControlViewModelBase
    {
        private GameTree _gameTree;
        private GameTreeNode _selectedTimelineNode;
        
        /// <summary>
        /// Creates timeline view model with a given game tree
        /// </summary>
        /// <param name="gameTree">Game tree</param>
        public TimelineViewModel(GameTree gameTree)
        {
            GameTree = gameTree;
            GameTree.LastNodeChanged += (s, node) => OnTimelineRedrawRequested();
        }


        // Indended for the UI View to subscribe and refresh the timeline accordingly.
        public event EventHandler TimelineRedrawRequsted;
        // Intended for the ViewModel to know when currently rendered node should be changed.
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
            TimelineRedrawRequsted?.Invoke(this, EventArgs.Empty);
        }

        private void OnTimelineSelectionChanged()
        {
            TimelineSelectionChanged?.Invoke(this, SelectedTimelineNode);
        }
    }
}
