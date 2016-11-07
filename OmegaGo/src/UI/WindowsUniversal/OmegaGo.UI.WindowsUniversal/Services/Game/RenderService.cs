using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public class RenderService
    {
        private BoardData _sharedBoardData;
        
        public const int StrokeThickness = 1;

        public RenderService(BoardData sharedBoardData)
        {
            _sharedBoardData = sharedBoardData;
        }

        public void CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {

        }

        public void Draw(CanvasControl sender, CanvasDrawEventArgs args, OmegaGo.Core.Game game)
        {
            int boardWidth = game.BoardSize.Width;
            int boardHeight = game.BoardSize.Height;

            sender.Width = BoardData.CellSize * boardWidth;
            sender.Height = BoardData.CellSize * boardHeight;

            args.DrawingSession.FillRectangle(
                0, 0, 
                BoardData.CellSize * boardWidth, 
                BoardData.CellSize * boardHeight, 
                _sharedBoardData.BoardColor);

            args.DrawingSession.DrawRectangle(
                0, 0,
                BoardData.CellSize * boardWidth,
                BoardData.CellSize * boardHeight,
                Colors.Black);

            for (int i = 0; i < boardWidth; i++)
            {
                args.DrawingSession.DrawLine(i * BoardData.CellSize, 0, i * BoardData.CellSize, BoardData.CellSize * boardHeight, Colors.Black);
            }

            for (int i = 0; i < boardHeight; i++)
            {
                args.DrawingSession.DrawLine(0, i * BoardData.CellSize, BoardData.CellSize * boardWidth, i * BoardData.CellSize, Colors.Black);
            }

            foreach (var move in game.PrimaryTimeline)
            {
                if (move.WhoMoves == Core.Color.Black)
                    DrawStone(args.DrawingSession, move.Coordinates.X, move.Coordinates.Y, Core.Color.Black);
                else if (move.WhoMoves == Core.Color.White)
                    DrawStone(args.DrawingSession, move.Coordinates.X, move.Coordinates.Y, Core.Color.White);
            }

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

        private void DrawStone(CanvasDrawingSession drawingSession, int x, int y, Core.Color stoneColor)
        {
            switch(stoneColor)
            {
                case Core.Color.Black:
                    drawingSession.FillEllipse(
                        BoardData.CellSize * x,
                        BoardData.CellSize * y,
                        BoardData.CellSize * 0.4f,
                        BoardData.CellSize * 0.4f,
                        Colors.Black);
                    break;
                case Core.Color.White:
                    drawingSession.FillEllipse(
                        BoardData.CellSize * x,
                        BoardData.CellSize * y,
                        BoardData.CellSize * 0.4f,
                        BoardData.CellSize * 0.4f,
                        Colors.White);
                    break;
            }
        }

        private void DrawStoneCellBackground(CanvasDrawingSession drawingSession, int x, int y, Color backgroundColor)
        {
            drawingSession.FillRoundedRectangle(
                BoardData.CellSize * x - BoardData.HalfCellSize,
                BoardData.CellSize * y - BoardData.HalfCellSize,
                BoardData.CellSize,
                BoardData.CellSize, 
                4, 4,
                backgroundColor);
        }
    }
}
