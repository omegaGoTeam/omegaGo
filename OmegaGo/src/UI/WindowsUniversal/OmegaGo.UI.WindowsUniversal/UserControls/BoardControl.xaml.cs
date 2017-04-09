﻿using OmegaGo.UI.Services.Game;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core.Game;
using System;

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

        private GameTreeNode currentGameTreeNode;

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
            set {
                SetValue(ViewModelProperty, value);
            }
        }

        public BoardControl()
        {
            this.InitializeComponent();
            this.canvas.TargetElapsedTime = System.TimeSpan.FromMilliseconds(32);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //Size size = new Size(Math.Max(availableSize.Width, availableSize.Height), Math.Max(availableSize.Width, availableSize.Height));
            //this.Content.Measure(size);

            //return size;

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

        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    if (finalSize.Width == 0)
        //    {
        //        Size newSize = new Size(finalSize.Height, finalSize.Height);

        //        return base.ArrangeOverride(newSize);
        //    }
        //    if (finalSize.Height == 0)
        //    {
        //        Size newSize = new Size(finalSize.Width, finalSize.Width);

        //        return base.ArrangeOverride(newSize);
        //    }

        //    return base.ArrangeOverride(finalSize);
        //}

        private void BoardControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.BoardRedrawRequested += ViewModel_BoardRedrawRequested;
            currentGameTreeNode = ViewModel.GameTreeNode;
            _boardControlState = ViewModel.BoardControlState;
            _renderService = new RenderService(_boardControlState);
            _inputService = new InputService(_boardControlState);
            
            _inputService.PointerTapped += (s, ev) => ViewModel.BoardTap(ev);
        }

        private void ViewModel_BoardRedrawRequested(object sender, GameTreeNode e)
        {
            currentGameTreeNode = e;
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

        private void canvas_CreateResources_1(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            RenderService.CreateResources(sender, args);
        }

        private void canvas_Draw_1(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            RenderService.Draw(sender, sender.Size.Width, sender.Size.Height, args.DrawingSession, currentGameTreeNode);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            RenderService.Update(args.Timing.ElapsedTime);
        }
    }
}
