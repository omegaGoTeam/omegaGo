using MvvmCross.Core.ViewModels;
using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class TimelineViewModel : MvxViewModel
    {
        private GameTree _gameTree;
        private GameTreeNode _timelineNode;

        public GameTree GameTree
        {
            get { return _gameTree; }
            set { SetProperty(ref _gameTree, value); OnTimelineRedrawRequested(); }
        }

        public GameTreeNode TimelineNode
        {
            get { return _timelineNode; }
            set { SetProperty(ref _timelineNode, value); OnTimelineRedrawRequested(); }
        }

        public event EventHandler TimelineRedrawRequsted;

        public TimelineViewModel()
        {

        }

        // TODO GameTree should notify - NodeAddedEvent<GameTreeNode>, for now make public and. Called from GameViewModel
        public void OnTimelineRedrawRequested()
        {
            TimelineRedrawRequsted?.Invoke(this, EventArgs.Empty);
        }
    }
}
