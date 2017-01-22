using OmegaGo.UI.Services.Game;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class BoardControl : UserControl
    {
        private BoardControlState _boardControlState;
        private InputService _inputService;
        private RenderService _renderService;
        
        public BoardControlState BoardControlState => _boardControlState;
        public InputService InputService => _inputService;
        public RenderService RenderService => _renderService;

        public static readonly DependencyProperty ViewModelProperty =
                   DependencyProperty.Register(
                           "ViewModel",
                           typeof(BoardViewModel),
                           typeof(BoardControl),
                           new PropertyMetadata(null));

        public BoardViewModel ViewModel
        {
            get { return (BoardViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public BoardControl()
        {
            this.InitializeComponent();
        }

        private void BoardControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO implement easy re-registering for varying BoardControlState
            _boardControlState = ViewModel.BoardControlState;
            _renderService = new RenderService(_boardControlState);
            _inputService = new InputService(_boardControlState);

            _boardControlState.BoardWidth = ViewModel.BoardControlState.BoardWidth;
            _boardControlState.BoardHeight = ViewModel.BoardControlState.BoardHeight;
            _boardControlState.RedrawRequested += (s, ev) => canvas.Invalidate();
            _inputService.PointerTapped += (s, ev) => ViewModel.BoardTap(ev);
            
            ViewModel.BoardRedrawRequsted += (s, ev) => 
            {
                if(ev.Move?.Kind == MoveKind.PlaceStone )
                    _boardControlState.SelectedPosition = ev.Move.Coordinates;

                canvas.Invalidate();
            };            
        }

        private void canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            RenderService.CreateResources(sender, args);
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            RenderService.Draw(sender, args, ViewModel.GameTreeNode);
        }

        private void canvas_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            InputService.PointerDown((int)pointerPosition.X, (int)pointerPosition.Y);
        }

        private void canvas_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            InputService.PointerUp((int)pointerPosition.X, (int)pointerPosition.Y);
        }

        private void canvas_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            InputService.PointerMoved((int)pointerPosition.X, (int)pointerPosition.Y);
        }
    }
}
