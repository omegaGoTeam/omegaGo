using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using OmegaGo.Core;
using System.Numerics;
using Windows.UI;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public class RenderService
    {
        private BoardData _sharedBoardData;
        
        public BoardData SharedBoardData
        {
            get { return _sharedBoardData; }
            private set { _sharedBoardData = value; }
        }

        public RenderService(BoardData sharedBoardData)
        {
            SharedBoardData = sharedBoardData;
        }

        public void CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {

        }

        public void Draw(CanvasControl sender, CanvasDrawEventArgs args, GameTreeNode gameState)
        {
            int boardWidth = SharedBoardData.BoardWidth;
            int boardHeight = SharedBoardData.BoardHeight;

            sender.Width = SharedBoardData.BoardActualWidth;
            sender.Height = SharedBoardData.BoardActualHeight;

            args.DrawingSession.FillRectangle(
                0, 0,
                SharedBoardData.BoardActualWidth,
                SharedBoardData.BoardActualHeight,
                _sharedBoardData.BoardColor);

            args.DrawingSession.DrawRectangle(
                0, 0,
                SharedBoardData.BoardActualWidth,
                SharedBoardData.BoardActualHeight,
                Colors.Black);

            CanvasTextFormat textFormat = new CanvasTextFormat() { WordWrapping = CanvasWordWrapping.NoWrap };

            for (int i = 0; i <= boardWidth; i++)
            {
                CanvasTextLayout textLayout = new CanvasTextLayout(sender, ((char)(65 + i)).ToString(), textFormat, SharedBoardData.CellSize, SharedBoardData.CellSize);
                textLayout.VerticalAlignment = CanvasVerticalAlignment.Center;

                args.DrawingSession.DrawTextLayout(
                    textLayout,
                    (i * SharedBoardData.CellSize - (float)textLayout.DrawBounds.Width * 0.5f) + SharedBoardData.BoardBorderThickness,
                    0,
                    Colors.Black);

                textLayout.Dispose();
            }

            for (int i = 0; i <= boardHeight; i++)
            {
                CanvasTextLayout textLayout = new CanvasTextLayout(sender, (boardHeight - i).ToString(), textFormat, SharedBoardData.CellSize, SharedBoardData.CellSize);
                textLayout.HorizontalAlignment = CanvasHorizontalAlignment.Center;

                args.DrawingSession.DrawTextLayout(
                    textLayout,
                    0,
                    (i * SharedBoardData.CellSize - (float)textLayout.DrawBounds.Height) + SharedBoardData.BoardBorderThickness,
                    Colors.Black);

                textLayout.Dispose();
            }

            textFormat.Dispose();
            args.DrawingSession.Transform = Matrix3x2.CreateTranslation(SharedBoardData.BoardBorderThickness, SharedBoardData.BoardBorderThickness);

            for (int i = 0; i <= boardWidth; i++)
            {
                args.DrawingSession.DrawLine(i * SharedBoardData.CellSize, 0, i * SharedBoardData.CellSize, SharedBoardData.CellSize * boardHeight, Colors.Black);
            }

            for (int i = 0; i <= boardHeight; i++)
            {
                args.DrawingSession.DrawLine(0, i * SharedBoardData.CellSize, SharedBoardData.CellSize * boardWidth, i * SharedBoardData.CellSize, Colors.Black);
            }

            // TODO check axis correctness
            if (gameState != null)
            {
                Core.StoneColor[,] boardState = gameState.BoardState;
                for (int x = 0; x < SharedBoardData.BoardWidth; x++)
                {
                    for (int y = 0; y < SharedBoardData.BoardHeight; y++)
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

            if (_sharedBoardData.HighlightedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    _sharedBoardData.HighlightedPosition.X,
                    _sharedBoardData.HighlightedPosition.Y,
                    _sharedBoardData.HighlightColor);
            }

            if (_sharedBoardData.SelectedPosition.IsDefined)
            {
                DrawStoneCellBackground(
                    args.DrawingSession,
                    _sharedBoardData.SelectedPosition.X,
                    _sharedBoardData.SelectedPosition.Y,
                    _sharedBoardData.SelectionColor);
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
                        SharedBoardData.CellSize * x,
                        SharedBoardData.CellSize * y,
                        SharedBoardData.CellSize * 0.4f,
                        SharedBoardData.CellSize * 0.4f,
                        Colors.Black);
                    break;
                case Core.StoneColor.White:
                    drawingSession.FillEllipse(
                        SharedBoardData.CellSize * x,
                        SharedBoardData.CellSize * y,
                        SharedBoardData.CellSize * 0.4f,
                        SharedBoardData.CellSize * 0.4f,
                        Colors.White);
                    break;
            }
        }

        private void DrawStoneCellBackground(CanvasDrawingSession drawingSession, int x, int y, Color backgroundColor)
        {
            drawingSession.FillRoundedRectangle(
                SharedBoardData.CellSize * x - SharedBoardData.HalfCellSize,
                SharedBoardData.CellSize * y - SharedBoardData.HalfCellSize,
                SharedBoardData.CellSize,
                SharedBoardData.CellSize, 
                4, 4,
                backgroundColor);
        }
    }
}
