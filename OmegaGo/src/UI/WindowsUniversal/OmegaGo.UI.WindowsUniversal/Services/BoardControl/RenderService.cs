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
using OmegaGo.UI.WindowsUniversal.Services.BoardControl;

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
            _textFormat = new CanvasTextFormat() { WordWrapping = CanvasWordWrapping.NoWrap };
        }

        private CanvasBitmap blackStoneBitmap;
        private CanvasBitmap whiteStoneBitmap;
        private CanvasBitmap oakBitmap;
        private CanvasBitmap kayaBitmap;
        private CanvasBitmap spaceBitmap;
        private StoneTheme stoneDisplayTheme;
        private BoardTheme _boardTheme;
        private bool _showCoordinates;
        private int _boardBorderThickness = 0;
        private int _boardLineThickness;
        private int _cellSize;
        private int _halfSize;
        private FpsCounter _fpsCounter = new FpsCounter();

        public void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            stoneDisplayTheme = _settings.Display.StonesTheme;
            _boardTheme = _settings.Display.BoardTheme;
            _showCoordinates = _settings.Display.ShowCoordinates;
            _boardLineThickness = SharedBoardControlState.BoardLineThickness;
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }
        async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            blackStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/black.png");
            whiteStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/white.png");
            oakBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/oak.jpg");
            kayaBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/kaya.jpg");
            spaceBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/space.png");

        }


        private void DrawBoard(CanvasDrawingSession session, double clientWidth, double clientHeight, Rect boardRectangle)
        {
            session.FillRectangle(new Windows.Foundation.Rect(0, 0, clientWidth, clientHeight), Colors.LightYellow);
            DrawBackground(boardRectangle, session);
        }
        /// <summary>
        /// Draws the entire game board for the provided state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="gameState"></param>
        public void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args, GameTreeNode gameState)
        {

            args.DrawingSession.Antialiasing = CanvasAntialiasing.Aliased;
            // Calculations
            double clientWidth = sender.Size.Width;
            double clientHeight = sender.Size.Height;
            int boardWidth = SharedBoardControlState.BoardWidth;
            int widthWithBorder = boardWidth + (_showCoordinates ? 2 : 0);
            int boardHeight = SharedBoardControlState.BoardHeight;
            Rect boardRectangle = RenderUtilities.Scale(new Rect(0, 0, clientWidth, clientHeight), 
                boardWidth + (_showCoordinates ? 2 : 0), boardHeight + (_showCoordinates ? 2 : 0));
            _cellSize = (int)(boardRectangle.Width/widthWithBorder);
            _halfSize = _cellSize/2;

            // Draw parts
            DrawBoard(args.DrawingSession, clientWidth, clientHeight, boardRectangle);

            // Draw coordinates   
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(
            (float)boardRectangle.X , (float)boardRectangle.Y);
            DrawBoardCoordinates(sender, args.DrawingSession, boardWidth, boardHeight);


            // Draw grid
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(
                (float)boardRectangle.X + _boardBorderThickness, (float)boardRectangle.Y + _boardBorderThickness);

            DrawBoardLines(args.DrawingSession, boardWidth, boardHeight);
            DrawBoardStarPoints(args.DrawingSession, boardWidth, boardHeight);


            // Shining position special case
            if (_sharedBoardControlState.ShiningPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    SharedBoardControlState.ShiningPosition.X,
                    (SharedBoardControlState.BoardHeight - 1) - SharedBoardControlState.ShiningPosition.Y,
                    Color.FromArgb(140, 100, 200, 100));
            }


            // Mouse over position special case
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

            args.DrawingSession.Transform = Matrix3x2.Identity;
            _fpsCounter.Draw(args, new Rect(clientWidth - 100, 10, 80, 30));
#if false
          

            if (gameState != null)
            {
                if (_settings.Tsumego.ShowPossibleMoves)
                {
                    foreach (var position in gameState.TsumegoMarkedPositions)
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
                                    args.DrawingSession.DrawEllipse(new Vector2(x*cellSize + halfSize,
                                        translatedYCoordinate*cellSize + halfSize), cellSize*0.2f,
                                        cellSize*0.2f,
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
#endif


        }

       


        private string fpsString = "";
        private int framesSinceLastSecond = 0;
        private DateTime lastFpsEmit = DateTime.Now;
        private CanvasTextFormat _textFormat;

        private void DrawBackground(Rect rect, CanvasDrawingSession session)
        {
            CanvasBitmap bitmapToDraw = null;
            switch (_boardTheme)
            {
                case BoardTheme.SolidColor:
                    session.FillRectangle(rect, this._sharedBoardControlState.BoardColor.ToUWPColor());
                    break;
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
                session.DrawImage(bitmapToDraw, rect);
            }
            session.DrawRectangle(rect, Colors.Black);
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
          
            
            if (stoneDisplayTheme == StoneTheme.PolishedBitmap)
            {
                double xPos = _cellSize * (x + 0.025);
                double yPos = _cellSize * (y + 0.025);
                drawingSession.DrawImage(color == StoneColor.Black ? blackStoneBitmap : whiteStoneBitmap,
                    new Rect(xPos, yPos,
                        _cellSize*0.95,
                        _cellSize * 0.95), blackStoneBitmap.Bounds, (float) opacity);
            }
            else
            {
                // We need to translate the position of the stone by its half to get the center for the ellipse shape
                int xPos = _cellSize*x + _halfSize;
                int yPos = _cellSize * y + _halfSize;
                float radiusModifier = 0.4f;
                float radius = _cellSize * radiusModifier;

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
                _cellSize * x,
                _cellSize * y,
                _cellSize,
                _cellSize, 
                4, 4,
                backgroundColor);
        }
        
        /// <summary>
        /// Draws horizontal and vertical coordinates for the board.
        /// </summary>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="boardWidth">width of the game board</param>
        /// <param name="boardHeight">height of the game board</param>
        private void DrawBoardCoordinates(ICanvasResourceCreator resourceCreator, CanvasDrawingSession drawingSession, int boardWidth, int boardHeight)
        {
            if (!_showCoordinates) return;
            _boardBorderThickness = _cellSize;
            int charCode = 65;
            
            // Draw horizontal char coordinates
            for (int i = 0; i < boardWidth; i++)
            {
                if ((char)charCode == 'I')
                    charCode++;

                CanvasTextLayout textLayout = RenderUtilities.GetCachedCanvasTextLayout(resourceCreator, ((char) (charCode)).ToString(), this._textFormat, _cellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                
                drawingSession.DrawTextLayout(
                    textLayout,
                    ((i+1) * _cellSize),
                    0,
                    Colors.Black);
                drawingSession.DrawTextLayout(
                  textLayout,
                  ((i + 1) * _cellSize),
                  _cellSize * (boardHeight+1),
                  Colors.Black);

                charCode++;
            }
 
            // Draw vertical numerical coordinates
            for (int i = 0; i < boardHeight; i++)
            {
                CanvasTextLayout textLayout = RenderUtilities.GetCachedCanvasTextLayout(resourceCreator, (boardHeight - i).ToString(), this._textFormat, _cellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                
                drawingSession.DrawTextLayout(
                    textLayout,
                    0,
                    ((i+1) * _cellSize),
                    Colors.Black);
                drawingSession.DrawTextLayout(
                    textLayout,
                    _cellSize * (boardWidth +1),
                    ((i+1) * _cellSize),
                    Colors.Black);
            }
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
                    _halfSize + i * _cellSize,              // x1
                    _halfSize,                                              // y1
                    _halfSize + i * _cellSize,              // x2
                    _cellSize * boardHeight - _halfSize,    // y2
                    _boardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black,
                    _boardLineThickness);
            }

            // Draw horizontal lines
            for (int i = 0; i < boardHeight; i++)
            {
                drawingSession.DrawLine(
                    _halfSize,                                              // x1
                    _halfSize + i * _cellSize,              // y2
                   _cellSize * boardWidth - _halfSize,     // x2
                    _halfSize + i * _cellSize,              // y2
                    _boardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black,
                    _boardLineThickness);
            }
        }

        private void DrawBoardStarPoints(CanvasDrawingSession drawingSession, int boardWidth, int boardHeight)
        {
            if (boardWidth != boardHeight)
            {
                // No star points for non-square boards
                return;
            }
            switch(boardWidth)
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
