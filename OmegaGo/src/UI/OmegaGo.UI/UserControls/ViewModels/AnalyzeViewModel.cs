using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Game.Tools;
using System;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class AnalyzeViewModel : ControlViewModelBase
    {
        private readonly IToolServices _toolServices;
        private ITool _selectedTool;

        private bool _isStoneToolSelected;        
        private bool _isCharacterToolSelected;
        private bool _isNumberToolSelected;
        private bool _isRectangleToolSelected;
        private bool _isTriangleToolSelected;
        private bool _isCircleToolSelected;
        private bool _isCrossToolSelected;

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

        public AnalyzeViewModel(IToolServices toolServices)
        {
            _toolServices = toolServices;
        }

        public event EventHandler<ITool> ToolChanged;
        public event EventHandler BackToGameRequested;

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

        public string NodeCommentary
        {
            get { return ToolServices.Node.Comment; }
            set { ToolServices.Node.Comment = value; RaisePropertyChanged(nameof(NodeCommentary)); }
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
        public PassTool PassTool { get; internal set; }
        public DeleteBranchTool DeleteBranchTool { get; internal set; }
        
        public SequenceMarkupTool CharacterMarkupTool { get; internal set; }
        public SequenceMarkupTool NumberMarkupTool { get; internal set; }
        public SimpleMarkupTool RectangleMarkupTool { get; internal set; }
        public SimpleMarkupTool TriangleMarkupTool { get; internal set; }
        public SimpleMarkupTool CircleMarkupTool { get; internal set; }
        public SimpleMarkupTool CrossMarkupTool { get; internal set; }

        public bool IsStoneToolSelected
        {
            get { return _isStoneToolSelected; }
            set { SetProperty(ref _isStoneToolSelected, value); }
        }

        public bool IsCharacterToolSelected
        {
            get { return _isCharacterToolSelected; }
            set { SetProperty(ref _isCharacterToolSelected, value); }
        }

        public bool IsNumberToolSelected
        {
            get { return _isNumberToolSelected; }
            set { SetProperty(ref _isNumberToolSelected, value); }
        }

        public bool IsRectangleToolSelected
        {
            get { return _isRectangleToolSelected; }
            set { SetProperty(ref _isRectangleToolSelected, value); }
        }

        public bool IsTriangleToolSelected
        {
            get { return _isTriangleToolSelected; }
            set { SetProperty(ref _isTriangleToolSelected, value); }
        }

        public bool IsCircleToolSelected
        {
            get { return _isCircleToolSelected; }
            set { SetProperty(ref _isCircleToolSelected, value); }
        }

        public bool IsCrossToolSelected
        {
            get { return _isCrossToolSelected; }
            set { SetProperty(ref _isCrossToolSelected, value); }
        }

        internal void OnNodeChanged()
        {
            RaisePropertyChanged(NodeCommentary);
        }

        private void UnselectAllTools()
        {
            IsStoneToolSelected = false;
            IsCharacterToolSelected = false;
            IsNumberToolSelected = false;
            IsRectangleToolSelected = false;
            IsTriangleToolSelected = false;
            IsCircleToolSelected = false;
            IsCrossToolSelected = false;
        }

        private void DeleteBranch()
        {
            DeleteBranchTool.Execute(ToolServices);
        }

        private void PlaceStone()
        {
            UnselectAllTools();
            IsStoneToolSelected = true;
            SelectedTool = StonePlacementTool;
        }
        
        private void PlaceCharacter()
        {
            UnselectAllTools();
            IsCharacterToolSelected = true;
            SelectedTool = CharacterMarkupTool;
        }

        private void PlaceNumber()
        {
            UnselectAllTools();
            IsNumberToolSelected = true;
            SelectedTool = NumberMarkupTool;
        }

        private void PlaceRectangle()
        {
            UnselectAllTools();
            IsRectangleToolSelected = true;
            SelectedTool = RectangleMarkupTool;
        }

        private void PlaceTriangle()
        {
            UnselectAllTools();
            IsTriangleToolSelected = true;
            SelectedTool = TriangleMarkupTool;
        }

        private void PlaceCircle()
        {
            UnselectAllTools();
            IsCircleToolSelected = true;
            SelectedTool = CircleMarkupTool;
        }

        private void PlaceCross()
        {
            UnselectAllTools();
            IsCrossToolSelected = true;
            SelectedTool = CrossMarkupTool;
        }

        private void BackToGame()
        {
            BackToGameRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Pass()
        {
            PassTool.Execute(ToolServices);
        }
    }
}
