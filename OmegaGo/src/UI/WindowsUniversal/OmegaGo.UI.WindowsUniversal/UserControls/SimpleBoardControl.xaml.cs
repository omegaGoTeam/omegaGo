using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.Utility;
using OmegaGo.UI.WindowsUniversal.Extensions;
using OmegaGo.UI.WindowsUniversal.Extensions.Colors;
using OmegaGo.UI.WindowsUniversal.Services.Game;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class SimpleBoardControl : UserControl
    {
        private RenderService _renderService = new RenderService(null);
        public RenderService RenderService => _renderService;

        public static readonly DependencyProperty GameBoardProperty = DependencyProperty.Register(
            "GameBoard", typeof(GameBoard), typeof(SimpleBoardControl), new PropertyMetadata(default(GameBoard), GameBoardChanged));

        private static void GameBoardChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var board = (SimpleBoardControl)dependencyObject;
            board.canvas.Invalidate();
        }

        public GameBoard GameBoard
        {
            get { return (GameBoard)GetValue(GameBoardProperty); }
            set { SetValue(GameBoardProperty, value); }
        }

        public SimpleBoardControl()
        {
            this.InitializeComponent();
        }

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            if (GameBoard != null)
            {
                RenderService.SharedBoardControlState = new BoardControlState(Rectangle.GetBoundingRectangle(GameBoard));
                RenderService.ShowCoordinates = false;
                RenderService.SimpleRenderService = true;
                RenderService.Draw(sender, sender.Size.Width, sender.Size.Height, args.DrawingSession,
                    new GameTreeNode()
                    {
                        BoardState = GameBoard
                    });
            }
        }


      

        private void canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            RenderService.CreateResources(sender, args);
        }
    }
}
