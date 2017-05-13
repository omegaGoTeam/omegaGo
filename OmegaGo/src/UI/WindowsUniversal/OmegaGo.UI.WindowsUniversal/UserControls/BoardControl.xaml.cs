using OmegaGo.UI.Services.Game;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using Windows.Foundation;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core.Game;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Windows.Devices.Input;

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    /// <summary>
    /// Presents an interactible Go board in a canvas.
    /// </summary>
    public sealed partial class BoardControl : UserControlBase
    {
        private const int TOUCHOFFSET = -25;

        private BoardControlState _boardControlState;
        private InputService _inputService;
        private RenderService _renderService;
        private GameTreeNode _currentGameTreeNode;

        public static readonly DependencyProperty ViewModelProperty =
                   DependencyProperty.Register(
                           "ViewModel",
                           typeof(BoardViewModel),
                           typeof(BoardControl),
                           new PropertyMetadata(null, ViewModelChanged));

        public BoardControl()
        {
            this.InitializeComponent();
            this.canvas.TargetElapsedTime = TimeSpan.FromMilliseconds(32);
        }

        /// <summary>
        /// Gets information stored with this board control, for example, styling or highlighted positions. This is the same object
        /// as in the view model.
        /// </summary>
        public BoardControlState BoardControlState => _boardControlState;
        public InputService InputService => _inputService;
        public RenderService RenderService => _renderService;

        /// <summary>
        /// The view model contains the displayed node as well as the same <see cref="BoardControlState"/>. Once set,
        /// the viewmodel must not change. 
        /// </summary>
        public BoardViewModel ViewModel
        {
            get { return (BoardViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private static void ViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BoardControl boardControl = d as BoardControl;

            if(boardControl != null)
            {
                var task = boardControl.InitializeVM(e.NewValue as BoardViewModel);
            }
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width))
            {
                return new Size(availableSize.Height, availableSize.Height);
            }

            if (double.IsInfinity(availableSize.Height))
            {
                return new Size(availableSize.Width, availableSize.Width);
            }

            return availableSize;
        }        

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Point pointerPosition = GetPointerPosition(e);

            InputService.PointerDown((int)pointerPosition.X, (int)pointerPosition.Y);
        }

        private void canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Point pointerPosition = GetPointerPosition(e);

            InputService.PointerUp((int)pointerPosition.X, (int)pointerPosition.Y);
        }
        
        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point pointerPosition = GetPointerPosition(e);

            InputService.PointerMoved((int)pointerPosition.X, (int)pointerPosition.Y);
        }

        private void Canvas_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            InputService.PointerExited();
        }

        // Helper method to offset touch pointer position for better stone shadow visibility.
        private Point GetPointerPosition(PointerRoutedEventArgs e)
        {
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                pointerPosition.Y += TOUCHOFFSET;

            return pointerPosition;
        }
        
        private async void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            await RenderService.AwaitResources();
            RenderService.Draw(
                sender, 
                sender.Size.Width, 
                sender.Size.Height, 
                args.DrawingSession, 
                _currentGameTreeNode);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            RenderService.Update(args.Timing.ElapsedTime);
        }

        private async Task InitializeVM(BoardViewModel viewModel)
        {
            if (viewModel == null)
                return;

            ViewModel.NodeChanged += (sender, node) => _currentGameTreeNode = node;
            _currentGameTreeNode = ViewModel.GameTreeNode;
            _boardControlState = ViewModel.BoardControlState;
            _renderService = new RenderService(_boardControlState);
            _inputService = new InputService(_boardControlState);

            await RenderService.AwaitResources();
            
            canvas.Draw += canvas_Draw;
            canvas.Update += canvas_Update;

            canvas.PointerMoved += canvas_PointerMoved;
            canvas.PointerPressed += canvas_PointerPressed;
            canvas.PointerReleased += canvas_PointerReleased;
            canvas.PointerExited += Canvas_PointerExited;

            _inputService.PointerTapped += (sender, ev) => ViewModel.BoardTap(ev);
        }
    }
}
