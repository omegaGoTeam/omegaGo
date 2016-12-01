using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.WindowsUniversal.Extensions;
using System.Numerics;
using Windows.UI;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public class RenderService
    {
        private BoardState _sharedBoardState;
        
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

            DrawBoardCoordinates(sender, args.DrawingSession, boardWidth, boardHeight);
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(SharedBoardState.BoardBorderThickness, SharedBoardState.BoardBorderThickness);
            DrawBoardLines(args.DrawingSession, boardWidth, boardHeight);
            
            if (gameState != null)
            {
                StoneColor[,] boardState = gameState.BoardState;
                for (int x = 0; x < SharedBoardState.BoardWidth; x++)
                {
                    for (int y = 0; y < SharedBoardState.BoardHeight; y++)
                    {
                        int translatedYCoordinate = (SharedBoardState.BoardHeight - y - 1);

                        if (boardState[x, y] == StoneColor.Black)
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, StoneColor.Black);
                        else if (boardState[x, y] == StoneColor.White)
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, StoneColor.White);
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

            if (_sharedBoardState.SelectedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    SharedBoardState.SelectedPosition.X,
                    (SharedBoardState.BoardHeight - 1) - SharedBoardState.SelectedPosition.Y,
                    SharedBoardState.SelectionColor.ToUWPColor());
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
        private void DrawStone(CanvasDrawingSession drawingSession, int x, int y, StoneColor stoneColor)
        {
            // We need to translate the position of the stone by its half to get the center for the ellipse shape
            int xPos = SharedBoardState.CellSize * x + SharedBoardState.HalfCellSize;
            int yPos = SharedBoardState.CellSize * y + SharedBoardState.HalfCellSize;
            float radiusModifier = 0.4f;
            float radius = SharedBoardState.CellSize * radiusModifier;

            switch (stoneColor)
            {
                case StoneColor.Black:
                    drawingSession.FillEllipse(
                        xPos,
                        yPos,
                        radius,
                        radius,
                        Colors.Black);
                    break;
                case StoneColor.White:
                    drawingSession.FillEllipse(
                        xPos,
                        yPos,
                        radius,
                        radius,
                        Colors.White);
                    break;
            }
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
                    Colors.Black);
            }

            // Draw horizontal lines
            for (int i = 0; i < boardHeight; i++)
            {
                drawingSession.DrawLine(
                    SharedBoardState.HalfCellSize,                                              // x1
                    SharedBoardState.HalfCellSize + i * SharedBoardState.CellSize,              // y2
                    SharedBoardState.CellSize * boardWidth - SharedBoardState.HalfCellSize,     // x2
                    SharedBoardState.HalfCellSize + i * SharedBoardState.CellSize,              // y2
                    Colors.Black);
            }
        }
    }
}
