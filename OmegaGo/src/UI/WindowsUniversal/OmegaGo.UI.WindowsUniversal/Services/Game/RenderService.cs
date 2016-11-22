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

            CanvasTextFormat textFormat = new CanvasTextFormat() { WordWrapping = CanvasWordWrapping.NoWrap };

            for (int i = 0; i <= boardWidth; i++)
            {
                CanvasTextLayout textLayout = new CanvasTextLayout(sender, ((char)(65 + i)).ToString(), textFormat, SharedBoardState.CellSize, SharedBoardState.CellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;

                args.DrawingSession.DrawTextLayout(
                    textLayout,
                    (i * SharedBoardState.CellSize - (float)textLayout.DrawBounds.Width * 0.5f) + SharedBoardState.BoardBorderThickness,
                    0,
                    Colors.Black);

                textLayout.Dispose();
            }

            for (int i = 0; i <= boardHeight; i++)
            {
                CanvasTextLayout textLayout = new CanvasTextLayout(sender, (boardHeight - i).ToString(), textFormat, SharedBoardState.CellSize, SharedBoardState.CellSize);
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;

                args.DrawingSession.DrawTextLayout(
                    textLayout,
                    0,
                    (i * SharedBoardState.CellSize - (float)textLayout.DrawBounds.Height) + SharedBoardState.BoardBorderThickness,
                    Colors.Black);

                textLayout.Dispose();
            }

            textFormat.Dispose();
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(SharedBoardState.BoardBorderThickness, SharedBoardState.BoardBorderThickness);

            for (int i = 0; i <= boardWidth; i++)
            {
                args.DrawingSession.DrawLine(i * SharedBoardState.CellSize, 0, i * SharedBoardState.CellSize, SharedBoardState.CellSize * boardHeight, Colors.Black);
            }

            for (int i = 0; i <= boardHeight; i++)
            {
                args.DrawingSession.DrawLine(0, i * SharedBoardState.CellSize, SharedBoardState.CellSize * boardWidth, i * SharedBoardState.CellSize, Colors.Black);
            }

            // TODO check axis correctness
            if (gameState != null)
            {
                Core.StoneColor[,] boardState = gameState.BoardState;
                for (int x = 0; x < SharedBoardState.BoardWidth; x++)
                {
                    for (int y = 0; y < SharedBoardState.BoardHeight; y++)
                    {
                        if (boardState[x, y] == Core.StoneColor.Black)
                            DrawStone(args.DrawingSession, x, y, Core.StoneColor.Black);
                        else if (boardState[x, y] == Core.StoneColor.White)
                            DrawStone(args.DrawingSession, x, y, Core.StoneColor.White);
                    }
                }
            }
            
            // Old rendering
            //foreach (var move in game.PrimaryTimeline)
            //{
            //    if (move.WhoMoves == Core.StoneColor.Black)
            //        DrawStone(args.DrawingSession, move.Coordinates.X, move.Coordinates.Y, Core.StoneColor.Black);
            //    else if (move.WhoMoves == Core.StoneColor.White)
            //        DrawStone(args.DrawingSession, move.Coordinates.X, move.Coordinates.Y, Core.StoneColor.White);
            //}

            if (_sharedBoardState.HighlightedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    _sharedBoardState.HighlightedPosition.X,
                    _sharedBoardState.HighlightedPosition.Y,
                    _sharedBoardState.HighlightColor.ToUWPColor());
            }

            if (_sharedBoardState.SelectedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    _sharedBoardState.SelectedPosition.X,
                    _sharedBoardState.SelectedPosition.Y,
                    _sharedBoardState.SelectionColor.ToUWPColor());
            }
        }

        public void Update()
        {

        }

        private void DrawStone(CanvasDrawingSession drawingSession, int x, int y, Core.StoneColor stoneColor)
        {
            switch(stoneColor)
            {
                case Core.StoneColor.Black:
                    drawingSession.FillEllipse(
                        SharedBoardState.CellSize * x,
                        SharedBoardState.CellSize * y,
                        SharedBoardState.CellSize * 0.4f,
                        SharedBoardState.CellSize * 0.4f,
                        Colors.Black);
                    break;
                case Core.StoneColor.White:
                    drawingSession.FillEllipse(
                        SharedBoardState.CellSize * x,
                        SharedBoardState.CellSize * y,
                        SharedBoardState.CellSize * 0.4f,
                        SharedBoardState.CellSize * 0.4f,
                        Colors.White);
                    break;
            }
        }

        private void DrawStoneCellBackground(CanvasDrawingSession drawingSession, int x, int y, Color backgroundColor)
        {
            drawingSession.FillRoundedRectangle(
                SharedBoardState.CellSize * x - SharedBoardState.HalfCellSize,
                SharedBoardState.CellSize * y - SharedBoardState.HalfCellSize,
                SharedBoardState.CellSize,
                SharedBoardState.CellSize, 
                4, 4,
                backgroundColor);
        }
    }
}
