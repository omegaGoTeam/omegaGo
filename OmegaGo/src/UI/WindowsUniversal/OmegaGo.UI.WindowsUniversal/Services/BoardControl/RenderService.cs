using System;
using System.Linq;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using OmegaGo.UI.Services.Game;
using System.Numerics;
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
using OmegaGo.Core.Game.Markup;
using Microsoft.Graphics.Canvas.Geometry;
using OmegaGo.Core.Game.Tools;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public class RenderService
    {
        private const int ReferenceWidth = 600;
        private const int ReferenceHeight = 600;
        private const int ReferenceFontSize = 20;

        private static CanvasBitmap blackStoneBitmap;
        private static CanvasBitmap whiteStoneBitmap;
        private static CanvasBitmap oakBitmap;
        private static CanvasBitmap kayaBitmap;
        private static CanvasBitmap spaceBitmap;
        private static CanvasBitmap sabakiBoardBitmap;
        private static CanvasBitmap sabakiTatamiBitmap;
        private static CanvasBitmap sabakiBlackBitmap;
        private static CanvasBitmap sabakiWhiteBitmap;
        private static TaskCompletionSource<bool> BitmapInitializationCompletion = new TaskCompletionSource<bool>();

        private BoardControlState _sharedBoardControlState;
        private IGameSettings _settings = Mvx.Resolve<IGameSettings>();

        private double _flickerPercentage;
        private bool _flickerAscending = true;
        private const double _flickersPerSecond = 0.5f;

        private int _boardLineThickness;
        private int _boardBorderThickness;
        private int _cellSize;
        private int _halfSize;
        private FpsCounter _fpsCounter = new FpsCounter();
        private bool _highlightLastMove;

        private StoneTheme _stoneDisplayTheme;
        private BoardTheme _boardTheme;

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

        public static async Task CreateResourcesAsync(ICanvasResourceCreator sender)
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
            BitmapInitializationCompletion.SetResult(true);
        }

        public async Task CreateResources()
        {
            ReloadSettings();
            await BitmapInitializationCompletion.Task;
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
                new Rect(0, 0, clientWidth, clientHeight),
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
                (float)boardRectangle.X, 
                (float)boardRectangle.Y);
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
            lines.Dispose();


            // Shining position special case
            if (_sharedBoardControlState.ShiningPosition.IsDefined)
            {
                int x = this.SharedBoardControlState.ShiningPosition.X;
                int y = ((this.SharedBoardControlState.BoardHeight - 1) - this.SharedBoardControlState.ShiningPosition.Y);

                float minusWhat = (float)_flickerPercentage * _cellSize * 0.07f;

                session.FillRoundedRectangle(
                    _cellSize * x + minusWhat,
                    _cellSize * y + minusWhat, 
                    _cellSize - 2 * minusWhat, 
                    _cellSize - 2 * minusWhat,
                    4, 4,
                    Color.FromArgb(140, 100, 200, 100));
            }

            // Draw all stones for given game state
            DrawStones(gameState, session);


            // Mouse over position special case
            if (_sharedBoardControlState.PointerOverPosition.IsDefined)
            {
                if (SharedBoardControlState.IsAnalyzeModeEnabled)
                {
                    // Analyze mode is enabled, draw selected tool shadow item.
                    DrawAnalyzeToolShadow(session, SharedBoardControlState.AnalyzeModeTool);
                }
                else
                {
                    // TODO Petr : only if legal - use Ruleset IsLegalMove?
                    // But it would be slow, you can implement caching to check for each intersection only once
                    if (_sharedBoardControlState.PointerOverShadowColor != StoneColor.None && (
                        _sharedBoardControlState.TEMP_MoveLegality == null ||
                        _sharedBoardControlState.TEMP_MoveLegality[this.SharedBoardControlState.PointerOverPosition.X, this.SharedBoardControlState.PointerOverPosition.Y] == MoveResult.Legal))
                    {
                        DrawStone(session, this.SharedBoardControlState.PointerOverPosition.X, this.SharedBoardControlState.PointerOverPosition.Y, _sharedBoardControlState.PointerOverShadowColor, 0.5);
                    }
                }
            }

            // Draw markups if enabled
            if(SharedBoardControlState.IsAnalyzeModeEnabled)
            {
                session.Blend = CanvasBlend.SourceOver;
                DrawMarkups(session, gameState.Markups);
            }


            // Draw debug FPS
            session.Transform = Matrix3x2.Identity;

            if (!SimpleRenderService)
            {
                _fpsCounter.Draw(session, new Rect(clientWidth - 100, 10, 80, 30));
            }
        }

        public void Update(TimeSpan elapsedTime)
        {
            _flickerPercentage += _flickersPerSecond * elapsedTime.TotalSeconds * (_flickerAscending ? 1 : -1);

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
                        session.DrawImage(sabakiTatamiBitmap,
                            new Vector2(x * (float)sabakiTatamiBitmap.Bounds.Width,
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
                        DrawStoneCellBackground(session,
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
                                DrawCrossOutMark(session, x, y, Colors.Red, Colors.Transparent);
                            }

                            DrawTerritoryMark(session, x, y, SharedBoardControlState.TerritoryMap.Board[x, y]);
                        }
                    }
                }
            }
        }

        private void DrawBackground(Rect rect, CanvasDrawingSession session)
        {
            CanvasBitmap bitmapToDraw = null;

            switch (_boardTheme)
            {
                case BoardTheme.SolidColor:
                    session.FillRectangle(rect, _sharedBoardControlState.BoardColor.ToUWPColor());
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
                session.DrawImage(bitmapToDraw, rect);
            }

            session.DrawRectangle(rect, Colors.Black, 2);
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

                if (this._stoneDisplayTheme == StoneTheme.Sabaki)
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
        
        private void DrawTerritoryMark(CanvasDrawingSession session, int x, int y, Territory territory)
        {
            if (territory == Territory.Black || territory == Territory.White)
            {
                Color color = (territory == Territory.Black ? Colors.Black : Colors.White);

                DrawCrossOutMark(session, x, y, color, Colors.Transparent);
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

        /// <summary>
        /// Draws horizontal and vertical coordinates for the board.
        /// </summary>
        /// <param name="resourceCreator"></param>
        /// <param name="drawingSession">used for rendering</param>
        /// <param name="boardWidth">width of the game board</param>
        /// <param name="boardHeight">height of the game board</param>
        private void DrawBoardCoordinates(ICanvasResourceCreator resourceCreator, CanvasDrawingSession drawingSession, int boardWidth, int boardHeight)
        {
            if (!this.ShowCoordinates) return;
            int charCode = 65 + _sharedBoardControlState.OriginX;
            if ((char)charCode >= 'I')
            {
                charCode++;
            }

            float fontSize = Math.Min(boardWidth * _cellSize, boardHeight * _cellSize) / (float)ReferenceWidth;
            _textFormat.FontSize = ReferenceFontSize * fontSize;

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
                drawingSession.DrawLine(_halfSize + i * _cellSize,         // x1
                    _halfSize,                                                 // y1
                    _halfSize + i * _cellSize,                             // x2
                    _cellSize * boardHeight - _halfSize,                  // y2
                    _boardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black, _boardLineThickness);
            }

            // Draw horizontal lines
            for (int i = 0; i < boardHeight; i++)
            {
                drawingSession.DrawLine(_halfSize,                             // x1
                    _halfSize + i * _cellSize,                             // y2
                    _cellSize * boardWidth - _halfSize,                   // x2
                    _halfSize + i * _cellSize,                             // y2
                    _boardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black, _boardLineThickness);
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

        ///////
        //
        // Markups
        //
        //////

        private void DrawAnalyzeToolShadow(CanvasDrawingSession drawingSession, ITool tool)
        {
            Color markupColor = Color.FromArgb(255, 66, 24, 5);
            if (tool is IPlacementTool)
            {
                IPlacementTool markupTool = (IPlacementTool)tool;
                Position pointerPosition = SharedBoardControlState.AnalyzeToolServices.PointerOverPosition;
                GameTreeNode node = SharedBoardControlState.AnalyzeToolServices.Node;
                IShadowItem shadowItem = markupTool.GetShadowItem(SharedBoardControlState.AnalyzeToolServices);
                
                switch(shadowItem.ShadowItemKind)
                {
                    case ShadowItemKind.Label:
                        DrawLabelMark(drawingSession, pointerPosition.X, pointerPosition.Y, markupColor, Colors.Transparent, ((Label)shadowItem).Text);
                        break;
                    case ShadowItemKind.Circle:
                        DrawCircleMark(drawingSession, pointerPosition.X, pointerPosition.Y, markupColor, Colors.Transparent);
                        break;
                    case ShadowItemKind.Cross:
                        DrawCrossOutMark(drawingSession, pointerPosition.X, pointerPosition.Y, markupColor, Colors.Transparent);
                        break;
                    case ShadowItemKind.Square:
                        DrawSquareMark(drawingSession, pointerPosition.X, pointerPosition.Y, markupColor, Colors.Transparent);
                        break;
                    case ShadowItemKind.Triangle:
                        DrawTriangleMark(drawingSession, pointerPosition.X, pointerPosition.Y, markupColor, Colors.Transparent);
                        break;
                    case ShadowItemKind.Stone:
                        DrawStone(drawingSession, pointerPosition.X, pointerPosition.Y, ((Stone)shadowItem).Color, 0.5);
                        break;
                }
            }
        }

        private void DrawMarkups(CanvasDrawingSession drawingSession, MarkupInfo markupInfo)
        {
            Color markupColor = Colors.Maroon;
            Color backgroundColor = Colors.Transparent;

            //backgroundColor.A = 0xBB;
            //backgroundColor.B = (byte)(backgroundColor.B * backgroundColor.A / 255);
            //backgroundColor.G = (byte)(backgroundColor.G * backgroundColor.A / 255);
            //backgroundColor.R = (byte)(backgroundColor.R * backgroundColor.A / 255);

            // Draw labels - characters, numbers
            foreach (var labelMarkup in markupInfo.GetMarkups<Label>().ToArray())
            {
                DrawLabelMark(drawingSession, labelMarkup.Position.X, labelMarkup.Position.Y, markupColor, backgroundColor, labelMarkup.Text);
            }
            
            // Draw circles
            foreach (var circleMarkup in markupInfo.GetMarkups<Circle>().ToArray())
            {
                DrawCircleMark(drawingSession, circleMarkup.Position.X, circleMarkup.Position.Y, markupColor, backgroundColor);
            }

            // Draw crosses
            foreach (var crossMarkup in markupInfo.GetMarkups<Cross>().ToArray())
            {
                DrawCrossOutMark(drawingSession, crossMarkup.Position.X, crossMarkup.Position.Y, markupColor, backgroundColor);
            }

            // Draw squares
            foreach (var squareMarkup in markupInfo.GetMarkups<Square>().ToArray())
            {
                DrawSquareMark(drawingSession, squareMarkup.Position.X, squareMarkup.Position.Y, markupColor, backgroundColor);
            }

            // Draw triangles
            foreach (var triangleMarkup in markupInfo.GetMarkups<Triangle>().ToArray())
            {
                DrawTriangleMark(drawingSession, triangleMarkup.Position.X, triangleMarkup.Position.Y, markupColor, backgroundColor);
            }
        }

        private void DrawLabelMark(CanvasDrawingSession session, int x, int y, Color color, Color background, string text)
        {
            y = (this.SharedBoardControlState.BoardHeight - 1) - y;
            
            session.FillRectangle(
                _cellSize * x,
                _cellSize * y,
                _cellSize,
                _cellSize,
                background);

            session.DrawText(
                text, 
                new Vector2(
                    _cellSize * x + _cellSize * 0.35f,
                    _cellSize * y + _cellSize * 0.15f), 
                color);
        }

        private void DrawCircleMark(CanvasDrawingSession session, int x, int y, Color color, Color background)
        {
            y = (this.SharedBoardControlState.BoardHeight - 1) - y;

            float cellSizeHalf = _cellSize * 0.5f;

            session.FillRectangle(
                _cellSize * x,
                _cellSize * y,
                _cellSize,
                _cellSize,
                background);

            session.DrawEllipse(
                new Vector2(
                    _cellSize * x + cellSizeHalf, 
                    _cellSize * y + cellSizeHalf), 
                _cellSize * 0.4f, 
                _cellSize * 0.4f,
                color, 3);
        }

        private void DrawCrossOutMark(CanvasDrawingSession session, int x, int y, Color color, Color background)
        {
            y = (this.SharedBoardControlState.BoardHeight - 1) - y;

            session.FillRectangle(
                _cellSize * x,
                _cellSize * y,
                _cellSize,
                _cellSize,
                background);

            session.DrawLine(
                new Vector2(_cellSize * (x + 0.2f), _cellSize * (y + 0.2f)),
                new Vector2(_cellSize * (x + 0.8f), _cellSize * (y + 0.8f)),
                color, 3);

            session.DrawLine(
                new Vector2(_cellSize * (x + 0.8f), _cellSize * (y + 0.2f)),
                new Vector2(_cellSize * (x + 0.2f), _cellSize * (y + 0.8f)),
                color, 3);
        }

        private void DrawSquareMark(CanvasDrawingSession session, int x, int y, Color color, Color background)
        {
            y = (this.SharedBoardControlState.BoardHeight - 1) - y;

            session.FillRectangle(
                _cellSize * x,
                _cellSize * y,
                _cellSize,
                _cellSize,
                background);

            session.DrawRectangle(
                _cellSize * (x + 0.2f), 
                _cellSize * (y + 0.2f), 
                _cellSize * 0.6f,
                _cellSize * 0.6f,
                color, 
                3);
        }

        private void DrawTriangleMark(CanvasDrawingSession session, int x, int y, Color color, Color background)
        {
            y = (this.SharedBoardControlState.BoardHeight - 1) - y;

            session.FillRectangle(
                _cellSize * x,
                _cellSize * y,
                _cellSize,
                _cellSize,
                background);

            session.DrawLine(
                new Vector2(_cellSize * (x + 0.2f), _cellSize * (y + 0.8f)),
                new Vector2(_cellSize * (x + 0.5f), _cellSize * (y + 0.2f)),
                color, 3);

            session.DrawLine(
                new Vector2(_cellSize * (x + 0.5f), _cellSize * (y + 0.2f)),
                new Vector2(_cellSize * (x + 0.8f), _cellSize * (y + 0.8f)),
                color, 3);

            session.DrawLine(
                new Vector2(_cellSize * (x + 0.8f), _cellSize * (y + 0.8f)),
                new Vector2(_cellSize * (x + 0.2f), _cellSize * (y + 0.8f)),
                color, 3);
        }
    }
}
