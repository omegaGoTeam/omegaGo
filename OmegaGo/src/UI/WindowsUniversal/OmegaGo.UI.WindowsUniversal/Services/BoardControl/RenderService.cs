﻿using System;
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
            get { return this._sharedBoardControlState; }
            private set { this._sharedBoardControlState = value; }
        }
        public RenderService(BoardControlState sharedBoardControlState)
        {
            this.SharedBoardControlState = sharedBoardControlState;
            this._textFormat = new CanvasTextFormat() { WordWrapping = CanvasWordWrapping.NoWrap };
        }

        private CanvasBitmap blackStoneBitmap;
        private CanvasBitmap whiteStoneBitmap;
        private CanvasBitmap oakBitmap;
        private CanvasBitmap kayaBitmap;
        private CanvasBitmap spaceBitmap;
        private StoneTheme stoneDisplayTheme;
        private BoardTheme _boardTheme;
        private bool _showCoordinates;
        private int _boardBorderThickness;
        private int _boardLineThickness;
        private int _cellSize;
        private int _halfSize;
        private bool _tsumegoShow;
        private FpsCounter _fpsCounter = new FpsCounter();
        private bool _highlightLastMove;
        private void ReloadSettings()
        {
            // TODO call this when tsumego checkbox changes
            this.stoneDisplayTheme = this._settings.Display.StonesTheme;
            this._boardTheme = this._settings.Display.BoardTheme;
            this._showCoordinates = this._settings.Display.ShowCoordinates;
            this._boardLineThickness = this.SharedBoardControlState.BoardLineThickness;
            this._tsumegoShow = this._settings.Tsumego.ShowPossibleMoves;
            this._highlightLastMove = this._settings.Display.HighlightLastMove;
        }

        public void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            ReloadSettings();
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }
        async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            this.blackStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/black.png");
            this.whiteStoneBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/white.png");
            this.oakBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/oak.jpg");
            this.kayaBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/kaya.jpg");
            this.spaceBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Textures/space.png");

        }


        private void DrawBoard(CanvasDrawingSession session, double clientWidth, double clientHeight, Rect boardRectangle)
        {
            session.FillRectangle(new Rect(0, 0, clientWidth, clientHeight), Colors.LightYellow);
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
            int boardWidth = this.SharedBoardControlState.BoardWidth;
            int widthWithBorder = boardWidth + (this._showCoordinates ? 2 : 0);
            int boardHeight = this.SharedBoardControlState.BoardHeight;
            Rect boardRectangle = RenderUtilities.Scale(new Rect(0, 0, clientWidth, clientHeight), 
                boardWidth + (this._showCoordinates ? 2 : 0), boardHeight + (this._showCoordinates ? 2 : 0));
            this._cellSize = (int)(boardRectangle.Width/widthWithBorder);
            this._halfSize = this._cellSize/2;

            // Draw parts
            DrawBoard(args.DrawingSession, clientWidth, clientHeight, boardRectangle);

            // Draw coordinates   
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(
            (float)boardRectangle.X , (float)boardRectangle.Y);
            DrawBoardCoordinates(sender, args.DrawingSession, boardWidth, boardHeight);


            // Draw grid
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(
                (float)boardRectangle.X + this._boardBorderThickness, (float)boardRectangle.Y + this._boardBorderThickness);
            this.SharedBoardControlState.LeftPadding = (int)boardRectangle.X + this._boardBorderThickness;
            this.SharedBoardControlState.TopPadding = (int)boardRectangle.Y + this._boardBorderThickness;
            this.SharedBoardControlState.NewCellSize = this._cellSize;

            DrawBoardLines(args.DrawingSession, boardWidth, boardHeight);
            DrawBoardStarPoints(args.DrawingSession, boardWidth, boardHeight);


            // Shining position special case
            if (this._sharedBoardControlState.ShiningPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession, this.SharedBoardControlState.ShiningPosition.X, this.SharedBoardControlState.ShiningPosition.Y,
                    Color.FromArgb(140, 100, 200, 100));
            }

            DrawStones(gameState, args.DrawingSession);


            // Mouse over position special case
            if (this._sharedBoardControlState.MouseOverPosition.IsDefined)
            {
                // TODO only if legal
                if (this._sharedBoardControlState.MouseOverShadowColor != StoneColor.None)
                {
                    DrawStone(args.DrawingSession, this.SharedBoardControlState.MouseOverPosition.X, this.SharedBoardControlState.MouseOverPosition.Y, this._sharedBoardControlState.MouseOverShadowColor, 0.5);

                }
                else
                {
                    // legacy
                    DrawStoneCellBackground(
                        args.DrawingSession, this.SharedBoardControlState.MouseOverPosition.X, this.SharedBoardControlState.MouseOverPosition.Y, this.SharedBoardControlState.HighlightColor.ToUWPColor());
                }

            }

            args.DrawingSession.Transform = Matrix3x2.Identity;
            this._fpsCounter.Draw(args, new Rect(clientWidth - 100, 10, 80, 30));


        }

        private void DrawStones(GameTreeNode gameState, CanvasDrawingSession session)
        {
            if (gameState != null)
            {
                if (this._tsumegoShow)
                {
                    foreach (var position in gameState.TsumegoMarkedPositions)
                    {
                        DrawStoneCellBackground(session,
                            position.X,
                            position.Y,
                            Color.FromArgb(100, 255, 50, 0));
                    }
                }
                GameBoard boardState = gameState.BoardState;

                for (int x = 0; x < this.SharedBoardControlState.BoardWidth; x++)
                {
                    for (int y = 0; y < this.SharedBoardControlState.BoardHeight; y++)
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
                                    session.DrawEllipse(new Vector2(x *this._cellSize + this._halfSize,
                                        translatedYCoordinate *this._cellSize + this._halfSize), this._cellSize * 0.2f, this._cellSize * 0.2f,
                                        boardState[x, y] == StoneColor.White ? Colors.Black : Colors.White, 3);
                                }
                            }
                        }

                    }
                }
            }
        }
        
        private CanvasTextFormat _textFormat;

        private void DrawBackground(Rect rect, CanvasDrawingSession session)
        {
            CanvasBitmap bitmapToDraw = null;
            switch (this._boardTheme)
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
        /// <param name="color"></param>
        /// <param name="opacity"></param>
        private void DrawStone(CanvasDrawingSession drawingSession, int x, int y, StoneColor color, double opacity)
        {
            y= (this.SharedBoardControlState.BoardHeight - 1) -y;

            
            if (this.stoneDisplayTheme == StoneTheme.PolishedBitmap)
            {
                double xPos = this._cellSize * (x + 0.025);
                double yPos = this._cellSize * (y + 0.025);
                drawingSession.DrawImage(color == StoneColor.Black ? this.blackStoneBitmap : this.whiteStoneBitmap,
                    new Rect(xPos, yPos, this._cellSize*0.95, this._cellSize * 0.95), this.blackStoneBitmap.Bounds, (float) opacity);
            }
            else
            {
                // We need to translate the position of the stone by its half to get the center for the ellipse shape
                int xPos = this._cellSize*x + this._halfSize;
                int yPos = this._cellSize * y + this._halfSize;
                float radiusModifier = 0.4f;
                float radius = this._cellSize * radiusModifier;

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
            drawingSession.FillRoundedRectangle(this._cellSize * x, this._cellSize * ((this.SharedBoardControlState.BoardHeight - 1) - y), this._cellSize, this._cellSize, 
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
            if (!this._showCoordinates) return;
            this._boardBorderThickness = this._cellSize;
            int charCode = 65;
            
            // Draw horizontal char coordinates
            for (int i = 0; i < boardWidth; i++)
            {
                if ((char)charCode == 'I')
                    charCode++;

                CanvasTextLayout textLayout = RenderUtilities.GetCachedCanvasTextLayout(resourceCreator, ((char) (charCode)).ToString(), this._textFormat, this._cellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                
                drawingSession.DrawTextLayout(
                    textLayout,
                    ((i+1) *this._cellSize),
                    0,
                    Colors.Black);
                drawingSession.DrawTextLayout(
                  textLayout,
                  ((i + 1) *this._cellSize), this._cellSize * (boardHeight+1),
                  Colors.Black);

                charCode++;
            }
 
            // Draw vertical numerical coordinates
            for (int i = 0; i < boardHeight; i++)
            {
                CanvasTextLayout textLayout = RenderUtilities.GetCachedCanvasTextLayout(resourceCreator, (boardHeight - i).ToString(), this._textFormat, this._cellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;
                
                drawingSession.DrawTextLayout(
                    textLayout,
                    0,
                    ((i+1) *this._cellSize),
                    Colors.Black);
                drawingSession.DrawTextLayout(
                    textLayout, this._cellSize * (boardWidth +1),
                    ((i+1) *this._cellSize),
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
                drawingSession.DrawLine(this._halfSize + i *this._cellSize,              // x1
                    this._halfSize,                                              // y1
                    this._halfSize + i *this._cellSize,              // x2
                    this._cellSize * boardHeight - this._halfSize,    // y2
                    this._boardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black, this._boardLineThickness);
            }

            // Draw horizontal lines
            for (int i = 0; i < boardHeight; i++)
            {
                drawingSession.DrawLine(this._halfSize,                                              // x1
                    this._halfSize + i *this._cellSize,              // y2
                    this._cellSize * boardWidth - this._halfSize,     // x2
                    this._halfSize + i *this._cellSize,              // y2
                    this._boardTheme == BoardTheme.VirtualBoard ? Colors.Cyan : Colors.Black, this._boardLineThickness);
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
                    drawingSession.FillEllipse(2.5f *this._cellSize, 2.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f *this._cellSize, 2.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(4.5f *this._cellSize, 4.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(2.5f *this._cellSize, 6.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f *this._cellSize, 6.5f *this._cellSize, 4, 4, Colors.Black);
                    break;
                case 13:
                    drawingSession.FillEllipse(3.5f *this._cellSize, 3.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f *this._cellSize, 3.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(6.5f *this._cellSize, 6.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(3.5f *this._cellSize, 9.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f *this._cellSize, 9.5f *this._cellSize, 4, 4, Colors.Black);
                    break;
                case 19:
                    drawingSession.FillEllipse(3.5f *this._cellSize, 3.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f *this._cellSize, 3.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f *this._cellSize, 3.5f *this._cellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f *this._cellSize, 9.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f *this._cellSize, 9.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f *this._cellSize, 9.5f *this._cellSize, 4, 4, Colors.Black);

                    drawingSession.FillEllipse(3.5f *this._cellSize, 15.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(9.5f *this._cellSize, 15.5f *this._cellSize, 4, 4, Colors.Black);
                    drawingSession.FillEllipse(15.5f *this._cellSize, 15.5f *this._cellSize, 4, 4, Colors.Black);
                    break;
            }
        }
    }
}
