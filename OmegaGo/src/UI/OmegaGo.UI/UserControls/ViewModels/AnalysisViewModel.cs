using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Game.Tools;
using System;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class AnalysisViewModel : ControlViewModelBase
    {
        private readonly IToolServices _toolServices;
        private ITool _selectedTool;


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

        public AnalysisViewModel(IToolServices toolServices)
        {
            _toolServices = toolServices;
        }

        public event EventHandler<ITool> ToolChanged;
        public event EventHandler BackToGameRequested;
        public event EventHandler PassRequested;

        public IToolServices ToolServices
        {
            get { return _toolServices; }
        }

        public ITool SelectedTool
        {
            get { return _selectedTool; }
            private set
            {
                SetProperty(ref _selectedTool, value);
                ToolChanged?.Invoke(this, value);
            }
        }

        public MvxCommand PlaceStoneCommand => _placeStoneCommand ?? (_placeStoneCommand = new MvxCommand(
            () => { PlaceStone(); }));
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

        // Add null - visibility converter
        public StonePlacementTool StonePlacementTool { get; internal set; }
        public DeleteBranchTool DeleteBranchTool { get; internal set; }

        public SequenceMarkupTool CharacterMarkupTool { get; internal set; }
        public SequenceMarkupTool NumberMarkupTool { get; internal set; }
        public SimpleMarkupTool RectangleMarkupTool { get; internal set; }
        public SimpleMarkupTool TriangleMarkupTool { get; internal set; }
        public SimpleMarkupTool CircleMarkupTool { get; internal set; }
        public SimpleMarkupTool CrossMarkupTool { get; internal set; }

        private void DeleteBranch()
        {
            DeleteBranchTool.Execute(ToolServices);
        }

        private void PlaceStone()
        {
            SelectedTool = StonePlacementTool;
        }
        
        private void PlaceCharacter()
        {
            SelectedTool = CharacterMarkupTool;
        }

        private void PlaceNumber()
        {
            SelectedTool = NumberMarkupTool;
        }

        private void PlaceRectangle()
        {
            SelectedTool = RectangleMarkupTool;
        }

        private void PlaceTriangle()
        {
            SelectedTool = TriangleMarkupTool;
        }

        private void PlaceCircle()
        {
            SelectedTool = CircleMarkupTool;
        }

        private void PlaceCross()
        {
            SelectedTool = CrossMarkupTool;
        }

        private void BackToGame()
        {
            BackToGameRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Pass()
        {

        }
    }
}
