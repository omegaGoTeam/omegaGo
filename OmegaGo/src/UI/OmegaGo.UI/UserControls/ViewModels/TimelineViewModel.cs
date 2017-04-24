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

        public GameTree GameTree
        {
            get { return _gameTree; }
            set { SetProperty(ref _gameTree, value); OnTimelineRedrawRequested(); }
        }

        public GameTreeNode SelectedTimelineNode
        {
            get { return _selectedTimelineNode; }
            set { SetProperty(ref _selectedTimelineNode, value); OnTimelineSelectionChanged(); }
        }

        // Indended for the UI View to subscribe and refresh the timeline accordingly.
        public event EventHandler TimelineRedrawRequsted;
        // Intended for the ViewModel to know when currently rendered node should be changed.
        internal event EventHandler<GameTreeNode> TimelineSelectionChanged;

        /// <summary>
        /// Creates timeline view model
        /// </summary>
        public TimelineViewModel()
        {

        }

        /// <summary>
        /// Creates timeline view model with a given game tree
        /// </summary>
        /// <param name="gameTree">Game tree</param>
        public TimelineViewModel(GameTree gameTree)
        {
            GameTree = gameTree;
            GameTree.LastNodeChanged += (s, node) => OnTimelineRedrawRequested();
        }
        
        public void RaiseGameTreeChanged()
        {
            OnTimelineRedrawRequested();
        }

        // TODO Petr: GameTree now has LastNodeChanged event, use it to fix this - GameTree should notify - NodeAddedEvent<GameTreeNode>, for now make public and. Called from GameViewModel
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
