using MvvmCross.Core.ViewModels;
using OmegaGo.Core.Game.Tools;
using System;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public sealed class AnalyzeViewModel : ControlViewModelBase
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

        /// <summary>
        /// Initializes a new instance of the AnalyzeViewModel class.
        /// </summary>
        /// <param name="toolServices">tool services that will be used by the analyze mode tools</param>
        public AnalyzeViewModel(IToolServices toolServices)
        {
            _toolServices = toolServices;
        }

        /// <summary>
        /// Occurs when currently selected tool changes.
        /// </summary>
        public event EventHandler<ITool> ToolChanged;

        /// <summary>
        /// Occurs when player requests the exit of analyze mode and return to normal game.
        /// </summary>
        public event EventHandler BackToGameRequested;

        /// <summary>
        /// Gets the tool services provider.
        /// </summary>
        public IToolServices ToolServices
        {
            get { return _toolServices; }
        }

        /// <summary>
        /// Gets the currently selected tool.
        /// </summary>
        public ITool SelectedTool
        {
            get { return _selectedTool; }
            private set
            {
                SetProperty(ref _selectedTool, value);
                ToolChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the commertary of the current node.
        /// </summary>
        public string NodeCommentary
        {
            get { return ToolServices.Node?.Comment; }
            set
            {
                // TODO Vita this can happen when there is no move yet. This should be fixed once every game starts with an empty node.
                if (ToolServices.Node == null)
                    return;

                ToolServices.Node.Comment = value;
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

        // In future possibly add null - visibility converter for tools that might not be available for all game types.
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
            get { return SelectedTool == StonePlacementTool; }
        }

        public bool IsCharacterToolSelected
        {
            get { return SelectedTool == CharacterMarkupTool; }
        }

        public bool IsNumberToolSelected
        {
            get { return SelectedTool == NumberMarkupTool; }
        }

        public bool IsRectangleToolSelected
        {
            get { return SelectedTool == RectangleMarkupTool; }
        }

        public bool IsTriangleToolSelected
        {
            get { return SelectedTool == TriangleMarkupTool; }
        }

        public bool IsCircleToolSelected
        {
            get { return SelectedTool == CircleMarkupTool; }
        }

        public bool IsCrossToolSelected
        {
            get { return SelectedTool == CrossMarkupTool; }
        }

        /// <summary>
        /// Notifies this view model that node changed.
        /// This is used to update the node commentary.
        /// </summary>
        internal void OnNodeChanged()
        {
            RaisePropertyChanged(nameof(NodeCommentary));
        }

        private void RaiseToolPropertiesChanged()
        {
            RaisePropertyChanged(nameof(IsStoneToolSelected));
            RaisePropertyChanged(nameof(IsCharacterToolSelected));
            RaisePropertyChanged(nameof(IsNumberToolSelected));
            RaisePropertyChanged(nameof(IsRectangleToolSelected));
            RaisePropertyChanged(nameof(IsTriangleToolSelected));
            RaisePropertyChanged(nameof(IsCircleToolSelected));
            RaisePropertyChanged(nameof(IsCrossToolSelected));
        }

        private void DeleteBranch()
        {
            DeleteBranchTool.Execute(ToolServices);
        }

        private void PlaceStone()
        {
            SelectedTool = StonePlacementTool;
            RaiseToolPropertiesChanged();
        }
        
        private void PlaceCharacter()
        {
            SelectedTool = CharacterMarkupTool;
            RaiseToolPropertiesChanged();
        }

        private void PlaceNumber()
        {
            SelectedTool = NumberMarkupTool;
            RaiseToolPropertiesChanged();
        }

        private void PlaceRectangle()
        {
            SelectedTool = RectangleMarkupTool;
            RaiseToolPropertiesChanged();
        }

        private void PlaceTriangle()
        {
            SelectedTool = TriangleMarkupTool;
            RaiseToolPropertiesChanged();
        }

        private void PlaceCircle()
        {
            SelectedTool = CircleMarkupTool;
            RaiseToolPropertiesChanged();
        }

        private void PlaceCross()
        {
            SelectedTool = CrossMarkupTool;
            RaiseToolPropertiesChanged();
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
