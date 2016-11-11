using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameView : ViewBase
    {
        private BoardData _boardData;
        private InputService _inputService;
        private RenderService _renderService;
        
        public GameViewModel VM => (GameViewModel)this.ViewModel;

        public BoardData BoardData
        {
            get { return _boardData; }
        }
        public InputService InputService
        {
            get { return _inputService; }
        }
        public RenderService RenderService
        {
            get  { return _renderService; }
        }
        
        public GameView()
        {
            _boardData = new BoardData();
            _inputService = new InputService(_boardData);
            _renderService = new RenderService(_boardData);
            
            this.InitializeComponent();
        }
        
        private void ViewBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _boardData.BoardWidth = VM.Game.BoardSize.Width;
            _boardData.BoardHeight = VM.Game.BoardSize.Height;
            _boardData.RedrawRequested += (s, ev) => canvas.Invalidate();
        }

        private void canvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            RenderService.CreateResources(sender, args);
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            RenderService.Draw(sender, args, VM.Game);
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
