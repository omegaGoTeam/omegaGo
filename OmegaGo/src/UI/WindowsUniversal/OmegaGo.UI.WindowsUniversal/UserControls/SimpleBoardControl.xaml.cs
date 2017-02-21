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
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.WindowsUniversal.Extensions;
using OmegaGo.UI.WindowsUniversal.Services.Game;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class SimpleBoardControl : UserControl
    {
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

        private float _cellSize = 0;
        private float _halfCellSize = 0;

        private void canvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            args.DrawingSession.Clear(Colors.Transparent);
            if (GameBoard != null)
            {
                BoardControlState state = new BoardControlState(GameBoard.Size);

                var boardSide = (float)Math.Max(sender.ActualHeight, sender.ActualWidth);

                _cellSize = boardSide / Math.Max(GameBoard.Size.Width, GameBoard.Size.Height);
                _halfCellSize = _cellSize / 2f;

                args.DrawingSession.FillRectangle(
                    0, 0, boardSide, boardSide, state.BoardColor.ToUWPColor());

                args.DrawingSession.DrawRectangle(
                    0, 0,
                    boardSide, boardSide,
                    Colors.Black);

                // TODO Martin : Perf. optimization: Place drawing board and coordinates into a command list.                               
                DrawBoardLines(args.DrawingSession);
                DrawBoardStarPoints(args.DrawingSession);

                for (int x = 0; x < GameBoard.Size.Width; x++)
                {
                    for (int y = 0; y < GameBoard.Size.Height; y++)
                    {
                        int translatedYCoordinate = (GameBoard.Size.Height - y - 1);

                        if (GameBoard[x, y] == StoneColor.Black)
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, Colors.Black);
                        else if (GameBoard[x, y] == StoneColor.White)
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, Colors.White);
                    }
                }
            }
        }


        /// <summary>
        /// Draws a stone at the specified position with specified color.
        /// </summary>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="x">position on x axis</param>
        /// <param name="y">position on y axis</param>
        /// <param name="stoneColor">color of the stone</param>
        private void DrawStone(CanvasDrawingSession drawingSession, int x, int y, Color color)
        {
            // We need to translate the position of the stone by its half to get the center for the ellipse shape
            float xPos = _cellSize * x + _halfCellSize;
            float yPos = _cellSize * y + _halfCellSize;
            float radiusModifier = 0.4f;
            float radius = _cellSize * radiusModifier;

            drawingSession.FillEllipse(
                xPos,
                yPos,
                radius,
                radius,
                color);
        }

        /// <summary>
        /// Draw the game board. Draws horizontal and vertical lines.
        /// </summary>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="boardWidth">width of the game board</param>
        /// <param name="boardHeight">height of the game board</param>
        private void DrawBoardLines(CanvasDrawingSession drawingSession)
        {
            // Each line starts / end in the middle of the cell -> Start with offset HalfCellSize AND end with minut offset HalfCellSize

            // Draw vertical lines
            for (int i = 0; i < GameBoard.Size.Width; i++)
            {
                drawingSession.DrawLine(
                    _halfCellSize + i * _cellSize,              // x1
                    _halfCellSize,                                              // y1
                    _halfCellSize + i * _cellSize,              // x2
                    _cellSize * GameBoard.Size.Height - _halfCellSize,    // y2
                    Colors.Black,
                    1);
            }

            // Draw horizontal lines
            for (int i = 0; i < GameBoard.Size.Height; i++)
            {
                drawingSession.DrawLine(
                    _halfCellSize,                                              // x1
                    _halfCellSize + i * _cellSize,              // y2
                    _cellSize * GameBoard.Size.Width - _halfCellSize,     // x2
                    _halfCellSize + i * _cellSize,              // y2
                    Colors.Black,
                    1);
            }
        }

        private void DrawBoardStarPoints(CanvasDrawingSession drawingSession)
        {
            // Star point
            int boardSize = GameBoard.Size.Width;
            switch (boardSize)
            {
                case 9:
                    drawingSession.FillEllipse(2.5f * _cellSize, 2.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * _cellSize, 2.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(4.5f * _cellSize, 4.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(2.5f * _cellSize, 6.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * _cellSize, 6.5f * _cellSize, 4, 4, Colors.Black);
                    break;
                case 13:
                    drawingSession.FillEllipse(3.5f * _cellSize, 3.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * _cellSize, 3.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * _cellSize, 6.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(3.5f * _cellSize, 9.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * _cellSize, 9.5f * _cellSize, 4, 4, Colors.Black);
                    break;
                case 19:
                    drawingSession.FillEllipse(3.5f * _cellSize, 3.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * _cellSize, 3.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * _cellSize, 3.5f * _cellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f * _cellSize, 9.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * _cellSize, 9.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * _cellSize, 9.5f * _cellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f * _cellSize, 15.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * _cellSize, 15.5f * _cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * _cellSize, 15.5f * _cellSize, 4, 4, Colors.Black);
                    break;
                default:
                    break;
            }
        }
    }
}
