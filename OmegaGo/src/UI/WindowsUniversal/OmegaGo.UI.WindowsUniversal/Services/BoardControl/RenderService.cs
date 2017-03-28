using System;
using System.Linq;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.WindowsUniversal.Extensions;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Settings;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Board.Styles;
using OmegaGo.UI.WindowsUniversal.Extensions.Colors;
using OmegaGo.UI.WindowsUniversal.Services.BoardControl;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public sealed class RenderService
    {
        private const double FLICKERSPERSECOND = 0.5f;

        private static CanvasBitmap blackStoneBitmap;
        private static CanvasBitmap whiteStoneBitmap;
        private static CanvasBitmap oakBitmap;
        private static CanvasBitmap kayaBitmap;
        private static CanvasBitmap spaceBitmap;
        private static CanvasBitmap sabakiBoardBitmap;
        private static CanvasBitmap sabakiTatamiBitmap;
        private static CanvasBitmap sabakiBlackBitmap;
        private static CanvasBitmap sabakiWhiteBitmap;

        private static int resourceCreationAssigned = 0;
        private static TaskCompletionSource<bool> resourcesCreation = new TaskCompletionSource<bool>();

        private BoardControlState _sharedBoardControlState;
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();
        private double _flickerPercentage;
        private bool _flickerAscending = true;

        private StoneTheme _stoneDisplayTheme;
        private BoardTheme _boardTheme;

        private int _boardBorderThickness;
        private int _boardLineThickness;
        private int _cellSize;
        private int _halfSize;
        private FpsCounter _fpsCounter = new FpsCounter();
        private bool _highlightLastMove;
        private CanvasTextFormat _textFormat;
        
        public RenderService(BoardControlState sharedBoardControlState)
        {
            this.SharedBoardControlState = sharedBoardControlState;
            _textFormat = new CanvasTextFormat() { WordWrapping = CanvasWordWrapping.NoWrap };
        }

        public BoardControlState SharedBoardControlState
        {
            get { return _sharedBoardControlState; }
            set { _sharedBoardControlState = value; }
        }

        public bool ShowCoordinates { get; set; }

        public bool SimpleRenderService { get; set; }


        public void CreateResources(ICanvasResourceCreator sender, CanvasCreateResourcesEventArgs args)
        {
            ReloadSettings();
            args.TrackAsyncAction(EnsureResourcesExistAsync(sender).AsAsyncAction());
        }
        
        public static async Task ResetResources()
        {
            if (resourceCreationAssigned == 1)
            {
                await resourcesCreation.Task;
            }

            resourcesCreation = new TaskCompletionSource<bool>();
            resourceCreationAssigned = 0;
        }
        
        /// <summary>
        /// Draws the entire game board for the provided state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="session"></param>
        /// <param name="gameState"></param>
        public void Draw(ICanvasResourceCreator sender, double width, double height, CanvasDrawingSession session, GameTreeNode gameState)
        {
            // Calculations
            double clientWidth = width;
            double clientHeight = height;

            int boardWidth = this.SharedBoardControlState.BoardWidth;
            int widthWithBorder = boardWidth + (this.ShowCoordinates ? 2 : 0);
            int boardHeight = this.SharedBoardControlState.BoardHeight;

            Rect boardRectangle = RenderUtilities.Scale(
                                    new Rect(
                                        0, 0, 
                                        clientWidth, 
                                        clientHeight),
                                        boardWidth + (this.ShowCoordinates ? 2 : 0), 
                                        boardHeight + (this.ShowCoordinates ? 2 : 0));

            _cellSize = (int)(boardRectangle.Width / widthWithBorder);
            _halfSize = _cellSize / 2;

            if (this.ShowCoordinates)
            {
                _boardBorderThickness = _cellSize;
            }
            else
            {
                _boardBorderThickness = 0;
            }

            this.SharedBoardControlState.LeftPadding = (int)boardRectangle.X + _boardBorderThickness;
            this.SharedBoardControlState.TopPadding = (int)boardRectangle.Y + _boardBorderThickness;
            this.SharedBoardControlState.NewCellSize = _cellSize;
            // The above should only be probably called on demand, not always

            // Draw parts
            DrawBoard(session, clientWidth, clientHeight, boardRectangle);

            // Draw coordinates   
            session.Transform = Matrix3x2.CreateTranslation(
            (float)boardRectangle.X, (float)boardRectangle.Y);
            DrawBoardCoordinates(sender, session, boardWidth, boardHeight);


            // Draw grid
            session.Transform = Matrix3x2.CreateTranslation(
                (float)boardRectangle.X + _boardBorderThickness, 
                (float)boardRectangle.Y + _boardBorderThickness);

            CanvasCommandList lines = new CanvasCommandList(sender);
            using (CanvasDrawingSession linesSession = lines.CreateDrawingSession())
            {
                linesSession.Antialiasing = CanvasAntialiasing.Aliased;
                DrawBoardLines(linesSession, boardWidth, boardHeight);
            }
            session.DrawImage(lines);
            DrawBoardStarPoints(session, boardWidth, boardHeight);

            // Shining position special case
            if (_sharedBoardControlState.ShiningPosition.IsDefined)
            {
                int x = this.SharedBoardControlState.ShiningPosition.X;
                int y = ((this.SharedBoardControlState.BoardHeight - 1) - this.SharedBoardControlState.ShiningPosition.Y);
                float minusWhat = (float)_flickerPercentage * _cellSize * 0.07f;
                session.FillRoundedRectangle(
                    _cellSize * x + minusWhat,
                    _cellSize * y + minusWhat, _cellSize - 2 * minusWhat, _cellSize - 2 * minusWhat,
                    4, 4,
                    Color.FromArgb(140, 100, 200, 100));
            }

            DrawStones(gameState, session);


            // Pointer over position special case
            if (_sharedBoardControlState.PointerOverPosition.IsDefined)
            {
                // TODO Petr : only if legal - use Ruleset IsLegalMove?
                // But it would be slow, you can implement caching to check for each intersection only once
                if (_sharedBoardControlState.MouseOverShadowColor != StoneColor.None)
                {
                    DrawStone(session, this.SharedBoardControlState.PointerOverPosition.X, this.SharedBoardControlState.PointerOverPosition.Y, _sharedBoardControlState.MouseOverShadowColor, 0.5);
                }
            }

            session.Transform = Matrix3x2.Identity;

            if (!SimpleRenderService)
            {
                _fpsCounter.Draw(session, new Rect(clientWidth - 100, 10, 80, 30));
            }

            lines.Dispose();
        }

        public void Update(TimeSpan elapsedTime)
        {
            _flickerPercentage += FLICKERSPERSECOND * elapsedTime.TotalSeconds * (_flickerAscending ? 1 : -1);

            if (_flickerPercentage >= 1)
            {
                _flickerPercentage = 1;
                _flickerAscending = false;
            }
            else if (_flickerPercentage <= 0)
            {
                _flickerPercentage = 0;
                _flickerAscending = true;
            }
        }

        private static async Task EnsureResourcesExistAsync(ICanvasResourceCreator sender)
        {
            if (Interlocked.CompareExchange(ref resourceCreationAssigned, 1, 0) == 1)
            {
                // wait
                await resourcesCreation.Task;
            }
            else
            {
                await CreateResourcesAsync(sender);
                resourcesCreation.SetResult(true);
            }
        }

        private static async Task CreateResourcesAsync(ICanvasResourceCreator sender)
        {
            blackStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/black.png");
            whiteStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/white.png");
            oakBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/oak.jpg");
            kayaBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/kaya.jpg");
            spaceBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/space.png");
            sabakiTatamiBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/SabakiTatami.png");
            sabakiWhiteBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/SabakiWhite.png");
            sabakiBlackBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/SabakiBlack.png");
            sabakiBoardBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/SabakiBoard.png");
        }

        private void ReloadSettings()
        {
            _stoneDisplayTheme = _settings.Display.StonesTheme;
            _boardTheme = _settings.Display.BoardTheme;
            this.ShowCoordinates = _settings.Display.ShowCoordinates;
            _boardLineThickness = this.SharedBoardControlState?.BoardLineThickness ?? 1;
            _highlightLastMove = _settings.Display.HighlightLastMove;
        }

        private void DrawBoard(CanvasDrawingSession session, double clientWidth, double clientHeight, Rect boardRectangle)
        {
            // Draw tatami mats
            if (!SimpleRenderService)
            {
                int columns = (int)Math.Ceiling(clientWidth / sabakiTatamiBitmap.Bounds.Width);
                int rows = (int)Math.Ceiling(clientHeight / sabakiTatamiBitmap.Bounds.Height);

                for (int x = 0; x < columns; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        session.DrawImage(
                            sabakiTatamiBitmap,
                            new Vector2(
                                x * (float)sabakiTatamiBitmap.Bounds.Width,
                                y * (float)sabakiTatamiBitmap.Bounds.Height));
                    }
                }
            }

            // Draw board
            DrawBackground(boardRectangle, session);
        }

        private void DrawStones(GameTreeNode gameState, CanvasDrawingSession session)
        {
            if (gameState != null)
            {
                if (gameState.Tsumego.MarkedPositions.Any() &&
                    _settings.Tsumego.ShowPossibleMoves)
                {
                    foreach (var position in gameState.Tsumego.MarkedPositions)
                    {
                        DrawStoneCellBackground(
                            session,
                            position.X,
                            position.Y,
                            Color.FromArgb(100, 255, 50, 0));
                    }
                }

                GameBoard boardState = gameState.BoardState;

                for (int x = SharedBoardControlState.OriginX; x < this.SharedBoardControlState.BoardWidth + SharedBoardControlState.OriginX; x++)
                {
                    for (int y = SharedBoardControlState.OriginY; y < this.SharedBoardControlState.BoardHeight + SharedBoardControlState.OriginY; y++)
                    {
                        int translatedYCoordinate = (this.SharedBoardControlState.BoardHeight - y - 1);

                        if (boardState[x, y] != StoneColor.None)
                        {
                            DrawStone(session, x, y, boardState[x, y], 1);

                            if (_highlightLastMove)
                            {
                                if (gameState?.Move?.Kind == MoveKind.PlaceStone &&
                                    gameState.Move.Coordinates.X == x &&
                                    gameState.Move.Coordinates.Y == y)
                                {
                                    session.DrawEllipse(
                                        new Vector2(
                                            (x - SharedBoardControlState.OriginX) * _cellSize + _halfSize,
                                            (translatedYCoordinate + SharedBoardControlState.OriginY) * _cellSize + _halfSize), 
                                        _cellSize * 0.2f, 
                                        _cellSize * 0.2f,
                                        boardState[x, y] == StoneColor.White ? Colors.Black : Colors.White, 
                                        3);
                                }
                            }

                        }
                        if (SharedBoardControlState.ShowTerritory &&
                            SharedBoardControlState.TerritoryMap != null)
                        {
                            if (SharedBoardControlState.TerritoryMap.DeadPositions.Contains(new Position(x, y)))
                            {
                                DrawCrossOutMark(session, x, y, Colors.Red);
                            }

                            DrawTerritoryMark(session, x, y, SharedBoardControlState.TerritoryMap.Board[x, y]);
                        }
                    }
                }
            }
        }
        
        private void DrawBackground(Rect rect, CanvasDrawingSession drawingSession)
        {
            CanvasBitmap bitmapToDraw = null;

            switch (_boardTheme)
            {
                case BoardTheme.SolidColor:
                    drawingSession.FillRectangle(rect, _sharedBoardControlState.BoardColor.ToUWPColor());
                    break;
                case BoardTheme.OakWood:
                    bitmapToDraw = oakBitmap;
                    break;
                case BoardTheme.KayaWood:
                    bitmapToDraw = kayaBitmap;
                    break;
                case BoardTheme.VirtualBoard:
                    bitmapToDraw = spaceBitmap;
                    break;
                case BoardTheme.SabakiBoard:
                    bitmapToDraw = sabakiBoardBitmap;
                    break;
            }

            if (bitmapToDraw != null)
            {
                drawingSession.DrawImage(bitmapToDraw, rect);
            }

            drawingSession.DrawRectangle(rect, Colors.Black, 2);
        }
        
        /// <summary>
        /// Draws a stone at the specified position with specified color.
        /// </summary>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="x">position on x axis</param>
        /// <param name="y">position on y axis</param>
        /// <param name="color"></param>
        /// <param name="opacity"></param>
        private void DrawStone(CanvasDrawingSession drawingSession, int x, int y, StoneColor color, double opacity)
        {
            x -= SharedBoardControlState.OriginX;
            y = (this.SharedBoardControlState.BoardHeight - 1) - (y - SharedBoardControlState.OriginY);
            
            if (_stoneDisplayTheme == StoneTheme.PolishedBitmap || _stoneDisplayTheme == StoneTheme.Sabaki)
            {
                CanvasBitmap bitmap = color == StoneColor.Black ? blackStoneBitmap : whiteStoneBitmap;

                if (_stoneDisplayTheme == StoneTheme.Sabaki)
                {
                    bitmap = color == StoneColor.Black ? sabakiBlackBitmap : sabakiWhiteBitmap;
                }

                double xPos = _cellSize * (x + 0.025);
                double yPos = _cellSize * (y + 0.025);

                drawingSession.DrawImage(
                    bitmap,
                    new Rect(
                        xPos, 
                        yPos, 
                        _cellSize * 0.95, 
                        _cellSize * 0.95), 
                    blackStoneBitmap.Bounds, 
                    (float)opacity);
            }
            else
            {
                // We need to translate the position of the stone by its half to get the center for the ellipse shape
                int xPos = _cellSize * x + _halfSize;
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

        private void DrawCrossOutMark(CanvasDrawingSession session, int x, int y, Color color)
        {
            y = (this.SharedBoardControlState.BoardHeight - 1) - y;

            session.DrawLine(
                new Vector2(
                    _cellSize * (x + 0.2f), 
                    _cellSize * (y + 0.2f)),
                new Vector2(
                    _cellSize * (x + 0.8f), 
                    _cellSize * (y + 0.8f)),
                color, 3);

            session.DrawLine(
                new Vector2(
                    _cellSize * (x + 0.8f), 
                    _cellSize * (y + 0.2f)),
                new Vector2(
                    _cellSize * (x + 0.2f), 
                    _cellSize * (y + 0.8f)),
                color, 3);
        }

        private void DrawTerritoryMark(CanvasDrawingSession session, int x, int y, Territory territory)
        {
            if (territory == Territory.Black || territory == Territory.White)
            {
                Color color = (territory == Territory.Black ? Colors.Black : Colors.White);
                DrawCrossOutMark(session, x, y, color);
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
                _cellSize * (x - SharedBoardControlState.OriginX),
                _cellSize * ((this.SharedBoardControlState.BoardHeight - 1) - (y - SharedBoardControlState.OriginY)), _cellSize, _cellSize,
                4, 4,
                backgroundColor);
        }

        /////////
        //
        //  Board
        //
        /////////


        /// <summary>
        /// Draws horizontal and vertical coordinates for the board.
        /// </summary>
        /// <param name="resourceCreator"></param>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="boardWidth">width of the game board</param>
        /// <param name="boardHeight">height of the game board</param>
        private void DrawBoardCoordinates(ICanvasResourceCreator resourceCreator, CanvasDrawingSession drawingSession, int boardWidth, int boardHeight)
        {
            if (!this.ShowCoordinates)
                return;

            int charCode = 65 + _sharedBoardControlState.OriginX;

            if ((char)charCode >= 'I')
            {
                charCode++;
            }

            // Draw horizontal char coordinates
            for (int i = 0; i < boardWidth; i++)
            {
                if ((char)charCode == 'I')
                    charCode++;

                CanvasTextLayout textLayout = RenderUtilities.GetCachedCanvasTextLayout(resourceCreator, ((char)(charCode)).ToString(), _textFormat, _cellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;

                drawingSession.DrawTextLayout(
                    textLayout,
                    ((i + 1) * _cellSize),
                    0,
                    Colors.Black);
                drawingSession.DrawTextLayout(
                  textLayout,
                  ((i + 1) * _cellSize), _cellSize * (boardHeight + 1),
                  Colors.Black);

                charCode++;
            }

            // Draw vertical numerical coordinates
            for (int i = 0; i < boardHeight; i++)
            {
                int y = (boardHeight - i) + _sharedBoardControlState.OriginY;
                CanvasTextLayout textLayout = RenderUtilities.GetCachedCanvasTextLayout(resourceCreator, y.ToString(), _textFormat, _cellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;

                drawingSession.DrawTextLayout(
                    textLayout,
                    0,
                    ((i + 1) * _cellSize),
                    Colors.Black);
                drawingSession.DrawTextLayout(
                    textLayout, _cellSize * (boardWidth + 1),
                    ((i + 1) * _cellSize),
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
                    _halfSize + i * _cellSize,          // x1
                    _halfSize,                          // y1
                    _halfSize + i * _cellSize,          // x2
                    _cellSize * boardHeight - _halfSize,// y2
                    _boardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black, 
                    _boardLineThickness);
            }

            // Draw horizontal lines
            for (int i = 0; i < boardHeight; i++)
            {
                drawingSession.DrawLine(
                    _halfSize,                          // x1
                    _halfSize + i * _cellSize,          // y2
                    _cellSize * boardWidth - _halfSize, // x2
                    _halfSize + i * _cellSize,          // y2
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

            switch (boardWidth)
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
            }
        }

        /////////
        //
        //  Markups
        //
        /////////

        private void DrawMarkups(GameTreeNode gameState, CanvasDrawingSession drawingSession)
        {
            // Draws Triangles, Rectangles, Circles, Characters, Numbers
        }
    }
}
