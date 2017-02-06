using System;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.WindowsUniversal.Extensions;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public class RenderService
    {
        private BoardControlState _sharedBoardControlState;
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();

        public BoardControlState SharedBoardControlState
        {
            get { return _sharedBoardControlState; }
            private set { _sharedBoardControlState = value; }
        }

        public RenderService(BoardControlState sharedBoardControlState)
        {
            SharedBoardControlState = sharedBoardControlState;
        }

        private CanvasBitmap blackStoneBitmap;
        private CanvasBitmap whiteStoneBitmap;
        private CanvasBitmap oakBitmap;
        private CanvasBitmap kayaBitmap;
        private CanvasBitmap spaceBitmap;

        public void CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }
        async Task CreateResourcesAsync(CanvasControl sender)
        {
            blackStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/black.png");
            whiteStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/white.png");
            oakBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/oak.jpg");
            kayaBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/kaya.jpg");
            spaceBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/space.png");

        }


        /// <summary>
        /// Draws the entire game board for the provided state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="gameState"></param>
        public void Draw(CanvasControl sender, CanvasDrawEventArgs args, GameTreeNode gameState)
        {
            /*
            args.DrawingSession.Antialiasing = CanvasAntialiasing.Aliased;
            args.DrawingSession.TextAntialiasing = CanvasTextAntialiasing.Aliased;
            */
            int boardWidth = SharedBoardControlState.BoardWidth;
            int boardHeight = SharedBoardControlState.BoardHeight;
            int cellSize = SharedBoardControlState.CellSize;
            int halfSize = SharedBoardControlState.HalfCellSize;
            Position lastMove = gameState?.Move?.Kind == MoveKind.PlaceStone
                ? gameState.Move.Coordinates
                : Position.Undefined;

            sender.Width = SharedBoardControlState.BoardActualWidth;
            sender.Height = SharedBoardControlState.BoardActualHeight;

            DrawBackground(args);

            // TODO Perf. optimalization: Place drawing board and coordinates into a command list.

            if (_settings.Display.ShowCoordinates)
            {
                DrawBoardCoordinates(sender, args.DrawingSession, boardWidth, boardHeight);
            }
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(SharedBoardControlState.BoardBorderThickness, SharedBoardControlState.BoardBorderThickness);
            DrawBoardLines(args.DrawingSession, boardWidth, boardHeight);
            DrawBoardStarPoints(args.DrawingSession, SharedBoardControlState);

            if (_sharedBoardControlState.SelectedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    SharedBoardControlState.SelectedPosition.X,
                    (SharedBoardControlState.BoardHeight - 1) - SharedBoardControlState.SelectedPosition.Y,
                    SharedBoardControlState.SelectionColor.ToUWPColor());
            }
            if (_sharedBoardControlState.ShiningPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    SharedBoardControlState.ShiningPosition.X,
                    (SharedBoardControlState.BoardHeight - 1) - SharedBoardControlState.ShiningPosition.Y,
                    Color.FromArgb(140, 100, 200, 100));
            }

            if (gameState != null)
            {
                if (_settings.Tsumego.ShowPossibleMoves)
                {
                    foreach (var position in gameState.Tsumego.MarkedPositions)
                    {
                        DrawStoneCellBackground(args.DrawingSession,
                            position.X,
                            (SharedBoardControlState.BoardHeight - 1) - position.Y,
                            Color.FromArgb(100, 255, 50, 0));
                    }
                }
                GameBoard boardState = gameState.BoardState;

                for (int x = 0; x < SharedBoardControlState.BoardWidth; x++)
                {
                    for (int y = 0; y < SharedBoardControlState.BoardHeight; y++)
                    {
                        int translatedYCoordinate = (SharedBoardControlState.BoardHeight - y - 1);


                        if (boardState[x, y] != StoneColor.None)
                        {
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, boardState[x, y], 1);

                            if (_settings.Display.HighlightLastMove)
                            {
                                if (gameState?.Move?.Kind == MoveKind.PlaceStone &&
                                    gameState.Move.Coordinates.X == x &&
                                    gameState.Move.Coordinates.Y == y)
                                {
                                    args.DrawingSession.DrawEllipse(new Vector2(x * cellSize + halfSize,
                                        translatedYCoordinate * cellSize + halfSize), cellSize * 0.2f,
                                        cellSize * 0.2f,
                                        boardState[x, y] == StoneColor.White ? Colors.Black : Colors.White, 3);
                                }
                            }
                        }
                        /*
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, Colors.Black);
                        else if (boardState[x, y] == StoneColor.White)
                            DrawStone(args.DrawingSession, x, translatedYCoordinate, Colors.White);*/

                    }
                }
            }

            if (_sharedBoardControlState.MouseOverPosition.IsDefined)
            {
                // TODO only if legal
                if (_sharedBoardControlState.MouseOverShadowColor != StoneColor.None)
                {
                    DrawStone(args.DrawingSession, SharedBoardControlState.MouseOverPosition.X,
                        (SharedBoardControlState.BoardHeight - 1) - SharedBoardControlState.MouseOverPosition.Y,
                        _sharedBoardControlState.MouseOverShadowColor, 0.5);

                }
                else
                {
                    // legacy
                    DrawStoneCellBackground(
                        args.DrawingSession,
                        SharedBoardControlState.MouseOverPosition.X,
                        (SharedBoardControlState.BoardHeight - 1) - SharedBoardControlState.MouseOverPosition.Y,
                        SharedBoardControlState.HighlightColor.ToUWPColor());
                }

            }

        }

        private void DrawBackground(CanvasDrawEventArgs args)
        {
            args.DrawingSession.FillRectangle(
                0, 0,
                this.SharedBoardControlState.BoardActualWidth,
                this.SharedBoardControlState.BoardActualHeight,
                this._sharedBoardControlState.BoardColor.ToUWPColor());
            CanvasBitmap bitmapToDraw = null;
            switch (this._settings.Display.BoardTheme)
            {
                case BoardTheme.OakWood:
                    bitmapToDraw = this.oakBitmap;
                    break;
                case BoardTheme.KayaWood:
                    bitmapToDraw = this.kayaBitmap;
                    break;
                case BoardTheme.VirtualBoard:
                    bitmapToDraw = this.spaceBitmap;
                    break;
            }
            if (bitmapToDraw != null)
            {
                args.DrawingSession.DrawImage(bitmapToDraw,
                    new Rect(0, 0, this.SharedBoardControlState.BoardActualWidth, this.SharedBoardControlState.BoardActualHeight));
            }
            args.DrawingSession.DrawRectangle(
                0, 0,
                this.SharedBoardControlState.BoardActualWidth,
                this.SharedBoardControlState.BoardActualHeight,
                Colors.Black);
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
        private void DrawStone(CanvasDrawingSession drawingSession, int x, int y, StoneColor color, double opacity)
        {


            if (_settings.Display.StonesTheme == StoneTheme.PolishedBitmap)
            {
                double xPos = SharedBoardControlState.CellSize * (x + 0.025);
                double yPos = SharedBoardControlState.CellSize * (y + 0.025);
                drawingSession.DrawImage(color == StoneColor.Black ? blackStoneBitmap : whiteStoneBitmap,
                    new Rect(xPos, yPos,
                        SharedBoardControlState.CellSize * 0.95,
                        SharedBoardControlState.CellSize * 0.95), blackStoneBitmap.Bounds, (float)opacity);
            }
            else
            {
                // We need to translate the position of the stone by its half to get the center for the ellipse shape
                int xPos = SharedBoardControlState.CellSize * x + SharedBoardControlState.HalfCellSize;
                int yPos = SharedBoardControlState.CellSize * y + SharedBoardControlState.HalfCellSize;
                float radiusModifier = 0.4f;
                float radius = SharedBoardControlState.CellSize * radiusModifier;

                drawingSession.FillEllipse(
                    xPos,
                    yPos,
                    radius,
                    radius,
                    color == StoneColor.Black ? Colors.Black : Colors.White);
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
                SharedBoardControlState.CellSize * x,
                SharedBoardControlState.CellSize * y,
                SharedBoardControlState.CellSize,
                SharedBoardControlState.CellSize,
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

                CanvasTextLayout textLayout = new CanvasTextLayout(resourceCreator, ((char)(charCode)).ToString(), textFormat, SharedBoardControlState.CellSize, SharedBoardControlState.CellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;

                drawingSession.DrawTextLayout(
                    textLayout,
                    (i * SharedBoardControlState.CellSize) + SharedBoardControlState.BoardBorderThickness,
                    0,
                    Colors.Black);

                charCode++;
                textLayout.Dispose();
            }

            // Draw vertical numerical coordinates
            for (int i = 0; i < boardHeight; i++)
            {
                CanvasTextLayout textLayout = new CanvasTextLayout(resourceCreator, (boardHeight - i).ToString(), textFormat, SharedBoardControlState.CellSize, SharedBoardControlState.CellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;

                drawingSession.DrawTextLayout(
                    textLayout,
                    0,
                    (i * SharedBoardControlState.CellSize) + SharedBoardControlState.BoardBorderThickness,
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
                    SharedBoardControlState.HalfCellSize + i * SharedBoardControlState.CellSize,              // x1
                    SharedBoardControlState.HalfCellSize,                                              // y1
                    SharedBoardControlState.HalfCellSize + i * SharedBoardControlState.CellSize,              // x2
                    SharedBoardControlState.CellSize * boardHeight - SharedBoardControlState.HalfCellSize,    // y2
                    _settings.Display.BoardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black,
                    SharedBoardControlState.BoardLineThickness);
            }

            // Draw horizontal lines
            for (int i = 0; i < boardHeight; i++)
            {
                drawingSession.DrawLine(
                    SharedBoardControlState.HalfCellSize,                                              // x1
                    SharedBoardControlState.HalfCellSize + i * SharedBoardControlState.CellSize,              // y2
                    SharedBoardControlState.CellSize * boardWidth - SharedBoardControlState.HalfCellSize,     // x2
                    SharedBoardControlState.HalfCellSize + i * SharedBoardControlState.CellSize,              // y2
                    _settings.Display.BoardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black,
                    SharedBoardControlState.BoardLineThickness);
            }
        }

        private void DrawBoardStarPoints(CanvasDrawingSession drawingSession, BoardControlState boardControlState)
        {
            // Star point
            int boardSize = boardControlState.BoardWidth;

            switch (boardSize)
            {
                case 9:
                    drawingSession.FillEllipse(2.5f * boardControlState.CellSize, 2.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * boardControlState.CellSize, 2.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(4.5f * boardControlState.CellSize, 4.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(2.5f * boardControlState.CellSize, 6.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * boardControlState.CellSize, 6.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    break;
                case 13:
                    drawingSession.FillEllipse(3.5f * boardControlState.CellSize, 3.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardControlState.CellSize, 3.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f * boardControlState.CellSize, 6.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(3.5f * boardControlState.CellSize, 9.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardControlState.CellSize, 9.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    break;
                case 19:
                    drawingSession.FillEllipse(3.5f * boardControlState.CellSize, 3.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardControlState.CellSize, 3.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * boardControlState.CellSize, 3.5f * boardControlState.CellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f * boardControlState.CellSize, 9.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardControlState.CellSize, 9.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * boardControlState.CellSize, 9.5f * boardControlState.CellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f * boardControlState.CellSize, 15.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f * boardControlState.CellSize, 15.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f * boardControlState.CellSize, 15.5f * boardControlState.CellSize, 4, 4, Colors.Black);
                    break;
                default:
                    break;
            }
        }
    }
}
