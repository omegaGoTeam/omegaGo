using OmegaGo.UI.Services.Game;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    /// <summary>
    /// Presents an interactible Go board in a canvas.
    /// </summary>
    public sealed partial class BoardControl : UserControlBase
    {
        private BoardControlState _boardControlState;
        private InputService _inputService;
        private RenderService _renderService;

        /// <summary>
        /// Gets information stored with this board control, for example, styling or highlighted positions. This is the same object
        /// as in the view model.
        /// </summary>
        public BoardControlState BoardControlState => _boardControlState;
        public InputService InputService => _inputService;
        public RenderService RenderService => _renderService;

        public static readonly DependencyProperty ViewModelProperty =
                   DependencyProperty.Register(
                           "ViewModel",
                           typeof(BoardViewModel),
                           typeof(BoardControl),
                           new PropertyMetadata(null));

        /// <summary>
        /// The view model contains the displayed node as well as the same <see cref="BoardControlState"/>. Once set,
        /// the viewmodel must not change. 
        /// </summary>
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

            // TODO Petr: the following two lines don't seem to do anything
            _boardControlState.BoardWidth = ViewModel.BoardControlState.BoardWidth;
            _boardControlState.BoardHeight = ViewModel.BoardControlState.BoardHeight;
            _boardControlState.RedrawRequested += (s, ev) => canvas.Invalidate();
            _inputService.PointerTapped += (s, ev) => ViewModel.BoardTap(ev);
            
            ViewModel.BoardRedrawRequsted += (s, ev) => 
            {
                canvas.Invalidate();
            };            
        }

        private async void canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            await RenderService.CreateResources(sender, args);
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
