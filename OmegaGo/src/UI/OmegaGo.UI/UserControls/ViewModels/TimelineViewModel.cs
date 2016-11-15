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
            set { SetProperty(ref _gameTree, value); }
        }

        public GameTreeNode TimelineNode
        {
            get { return _timelineNode; }
            set { SetProperty(ref _timelineNode, value); }
        }

        public TimelineViewModel()
        {

        }
    }
}
