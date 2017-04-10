using MvvmCross.Core.ViewModels;
using System;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class AnalysisViewModel : ControlViewModelBase
    {
        private object _selectedTool;

        private MvxCommand _placeStoneCommand;
        private MvxCommand _deleteBranchCommand;

        private MvxCommand _placeCharacterCommand;
        private MvxCommand _placeNumberCommand;
        private MvxCommand _placeRectangleCommand;
        private MvxCommand _placeTriangleCommand;
        private MvxCommand _placeCircleCommand;
        private MvxCommand _placeCrossCommand;

        private MvxCommand _backToGameCommand;
        private MvxCommand _passCommand;

        //public event EventHandler<object> ToolChanged;
        public event EventHandler BranchDeletionRequested;
        public event EventHandler BackToGameRequested;
        public event EventHandler PassRequested;

        public object SelectedTool
        {
            get { return _selectedTool; }
        }

        public MvxCommand PlaceStoneCommand => _placeStoneCommand ?? (_placeStoneCommand = new MvxCommand(
            () => { PlaceStore(); }));
        public MvxCommand DeleteBranchCommand => _deleteBranchCommand ?? (_deleteBranchCommand = new MvxCommand(
            () => { DeleteBranch(); }));

        public MvxCommand PlaceCharacterCommand => _placeCharacterCommand ?? (_placeCharacterCommand = new MvxCommand(
            () => { PlaceCharacter(); }));
        public MvxCommand PlaceNumberCommand => _placeNumberCommand ?? (_placeNumberCommand = new MvxCommand(
            () => { PlaceNumber(); }));
        public MvxCommand PlaceRectangleCommand => _placeRectangleCommand ?? (_placeRectangleCommand = new MvxCommand(
            () => { PlaceRectangle(); }));
        public MvxCommand PlaceTriangleCommand => _placeTriangleCommand ?? (_placeTriangleCommand = new MvxCommand(
            () => { PlaceTriangle(); }));
        public MvxCommand PlaceCircleCommand => _placeCircleCommand ?? (_placeCircleCommand = new MvxCommand(
            () => { PlaceCircle(); }));
        public MvxCommand PlaceCrossCommand => _placeCrossCommand ?? (_placeCrossCommand = new MvxCommand(
            () => { PlaceCross(); }));

        public MvxCommand BackToGameCommand => _backToGameCommand ?? (_backToGameCommand = new MvxCommand(
            () => { BackToGame(); }));
        public MvxCommand PassCommand => _passCommand ?? (_passCommand = new MvxCommand(
            () => { Pass(); }));


        public AnalysisViewModel()
        {

        }


        private void PlaceStore()
        {

        }

        private void DeleteBranch()
        {

        }

        private void PlaceCharacter()
        {

        }

        private void PlaceNumber()
        {

        }

        private void PlaceRectangle()
        {

        }

        private void PlaceTriangle()
        {

        }

        private void PlaceCircle()
        {

        }

        private void PlaceCross()
        {

        }

        private void BackToGame()
        {
            BackToGameRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Pass()
        {

        }

        private void ChangeTool(object tool)
        {
            _selectedTool = tool;
        }
    }
}
