using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.WindowsUniversal.Extensions;
using System.Numerics;
using Windows.UI;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public class RenderService
    {
        private BoardState _sharedBoardState;
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        
        public BoardState SharedBoardState
        {
            get { return _sharedBoardState; }
            private set { _sharedBoardState = value; }
        }

        public RenderService(BoardState sharedBoardState)
        {
            SharedBoardState = sharedBoardState;
        }

        public void CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {

        }


        /// <summary>
        /// Draws the entire game board for the provided state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="gameState"></param>
        public void Draw(CanvasControl sender, CanvasDrawEventArgs args, GameTreeNode gameState)
        {
            int boardWidth = SharedBoardState.BoardWidth;
            int boardHeight = SharedBoardState.BoardHeight;

            sender.Width = SharedBoardState.BoardActualWidth;
            sender.Height = SharedBoardState.BoardActualHeight;
            
            args.DrawingSession.FillRectangle(
                0, 0,
                SharedBoardState.BoardActualWidth,
                SharedBoardState.BoardActualHeight,
                _sharedBoardState.BoardColor.ToUWPColor());

            args.DrawingSession.DrawRectangle(
                0, 0,
                SharedBoardState.BoardActualWidth,
                SharedBoardState.BoardActualHeight,
                Colors.Black);

            // TODO Perf. optimalization: Place drawing board and coordinates into a command list.

            if (_settings.Display.ShowCoordinates)
            {
                DrawBoardCoordinates(sender, args.DrawingSession, boardWidth, boardHeight);
            }
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(SharedBoardState.BoardBorderThickness, SharedBoardState.BoardBorderThickness);
            DrawBoardLines(args.DrawingSession, boardWidth, boardHeight);
            DrawBoardStarPoints(args.DrawingSession, SharedBoardState);

            if (_sharedBoardState.SelectedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    SharedBoardState.SelectedPosition.X,
                    (SharedBoardState.BoardHeight - 1) - SharedBoardState.SelectedPosition.Y,
                    SharedBoardState.SelectionColor.ToUWPColor());
            }
            if (_sharedBoardState.ShiningPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    SharedBoardState.ShiningPosition.X,
                    (SharedBoardState.BoardHeight - 1) - SharedBoardState.ShiningPosition.Y,
                    Color.FromArgb(140, 100, 200,100));
            }

            if (gameState != null)
            {
                if (_settings.Tsumego_ShowPossibleMoves)
                {
                    foreach (var position in gameState.TsumegoMarkedPositiongs)
                    {
                        DrawStoneCellBackground(args.DrawingSession, 
                            position.X,
                            (SharedBoardState.BoardHeight - 1) - position.Y,
                            Color.FromArgb(100, 255, 50, 0));
                    }
                }
                GameBoard boardState = gameState.BoardState;
             
                for (int x = 0; x < SharedBoardState.BoardWidth; x++)
                {
                    for (int y = 0; y < SharedBoardState.BoardHeight; y++)
                    {
                        int translatedYCoordinate = (SharedBoardState.BoardHeight - y - 1);

                        if (boardState[x, y] == StoneColor.Black)
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, Colors.Black);
                        else if (boardState[x, y] == StoneColor.White)
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, Colors.White);
                    }
                }
            }
            
            if (_sharedBoardState.HighlightedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    SharedBoardState.HighlightedPosition.X,
                    (SharedBoardState.BoardHeight - 1) - SharedBoardState.HighlightedPosition.Y,
                    SharedBoardState.HighlightColor.ToUWPColor());
            }
            
        }

        public void Update()
        {

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
            int xPos = SharedBoardState.CellSize * x + SharedBoardState.HalfCellSize;
            int yPos = SharedBoardState.CellSize * y + SharedBoardState.HalfCellSize;
            float radiusModifier = 0.4f;
            float radius = SharedBoardState.CellSize * radiusModifier;

            drawingSession.FillEllipse(
                xPos,
                yPos,
                radius,
                radius,
                color);
        }
        
        /// <summary>
        /// Draws a background for an intersection.
        /// </summary>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="x">position on x axis</param>
        /// <param name="y">position on y axis</param>
        /// <param name="backgroundColor">background color</param>
        private void DrawStoneCellBackground(CanvasDrawingSession drawingSession, int x, int y, Color backgroundColor)
        {
            // No need to find center as we are drawing a rectangle from top left position - which we have
            drawingSession.FillRoundedRectangle(
                SharedBoardState.CellSize * x,
                SharedBoardState.CellSize * y,
                SharedBoardState.CellSize,
                SharedBoardState.CellSize, 
                4, 4,
                backgroundColor);
        }
        
        /// <summary>
        /// Draws horizontal and vertical coordinates for the board.
        /// </summary>
        /// <param name="resourceCreator">used for storing graphical resources</param>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="boardWidth">width of the game board</param>
        /// <param name="boardHeight">height of the game board</param>
        private void DrawBoardCoordinates(ICanvasResourceCreator resourceCreator, CanvasDrawingSession drawingSession, int boardWidth, int boardHeight)
        {
            CanvasTextFormat textFormat = new CanvasTextFormat() { WordWrapping = CanvasWordWrapping.NoWrap };
            int charCode = 65;
            
            // Draw horizontal char coordinates
            for (int i = 0; i < boardWidth; i++)
            {
                if ((char)charCode == 'I')
                    charCode++;

                CanvasTextLayout textLayout = new CanvasTextLayout(resourceCreator, ((char)(charCode)).ToString(), textFormat, SharedBoardState.CellSize, SharedBoardState.CellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                
                drawingSession.DrawTextLayout(
                    textLayout,
                    (i * SharedBoardState.CellSize) + SharedBoardState.BoardBorderThickness,
                    0,
                    Colors.Black);

                charCode++;
                textLayout.Dispose();
            }
 
            // Draw vertical numerical coordinates
            for (int i = 0; i < boardHeight; i++)
            {
                CanvasTextLayout textLayout = new CanvasTextLayout(resourceCreator, (boardHeight - i).ToString(), textFormat, SharedBoardState.CellSize, SharedBoardState.CellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                
                drawingSession.DrawTextLayout(
                    textLayout,
                    0,
                    (i * SharedBoardState.CellSize) + SharedBoardState.BoardBorderThickness,
                    Colors.Black);

                textLayout.Dispose();
            }

            textFormat.Dispose();
        }

        /// <summary>
        /// Draw the game board. Draws horizontal and vertical lines.
        /// </summary>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="boardWidth">width of the game board</param>
        /// <param name="boardHeight">height of the game board</param>
        private void DrawBoardLines(CanvasDrawingSession drawingSession, int boardWidth, int boardHeight)
        {
            // Each line starts / end in the middle of the cell -> Start with offset HalfCellSize AND end with minut offset HalfCellSize

            // Draw vertical lines
            for (int i = 0; i < boardWidth; i++)
            {
                drawingSession.DrawLine(
                    SharedBoardState.HalfCellSize + i * SharedBoardState.CellSize,              // x1
                    SharedBoardState.HalfCellSize,                                              // y1
                    SharedBoardState.HalfCellSize + i * SharedBoardState.CellSize,              // x2
                    SharedBoardState.CellSize * boardHeight - SharedBoardState.HalfCellSize,    // y2
                    Colors.Black,
                    SharedBoardState.BoardLineThickness);
            }

            // Draw horizontal lines
            for (int i = 0; i < boardHeight; i++)
            {
                drawingSession.DrawLine(
                    SharedBoardState.HalfCellSize,                                              // x1
                    SharedBoardState.HalfCellSize + i * SharedBoardState.CellSize,              // y2
                    SharedBoardState.CellSize * boardWidth - SharedBoardState.HalfCellSize,     // x2
                    SharedBoardState.HalfCellSize + i * SharedBoardState.CellSize,              // y2
                    Colors.Black,
                    SharedBoardState.BoardLineThickness);
            }
        }

        private void DrawBoardStarPoints(CanvasDrawingSession drawingSession, BoardState boardState)
        {
            // Star point
            int boardSize = boardState.BoardWidth;

            switch(boardSize)
            {
                case 9:
                    drawingSession.FillEllipse(2.5f * boardState.CellSize, 2.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * boardState.CellSize, 2.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(4.5f * boardState.CellSize, 4.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(2.5f * boardState.CellSize, 6.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * boardState.CellSize, 6.5f * boardState.CellSize, 4, 4, Colors.Black);
                    break;
                case 13:
                    drawingSession.FillEllipse(3.5f * boardState.CellSize, 3.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardState.CellSize, 3.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * boardState.CellSize, 6.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(3.5f * boardState.CellSize, 9.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardState.CellSize, 9.5f * boardState.CellSize, 4, 4, Colors.Black);
                    break;
                case 19:
                    drawingSession.FillEllipse(3.5f * boardState.CellSize, 3.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardState.CellSize, 3.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * boardState.CellSize, 3.5f * boardState.CellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f * boardState.CellSize, 9.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardState.CellSize, 9.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * boardState.CellSize, 9.5f * boardState.CellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f * boardState.CellSize, 15.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardState.CellSize, 15.5f * boardState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * boardState.CellSize, 15.5f * boardState.CellSize, 4, 4, Colors.Black);
                    break;
                default:
                    break;
            }
        }
    }
}
