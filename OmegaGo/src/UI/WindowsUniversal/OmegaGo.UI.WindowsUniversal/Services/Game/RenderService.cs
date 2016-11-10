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

        public void Draw(CanvasControl sender, CanvasDrawEventArgs args, OmegaGo.Core.Game game)
        {
            int boardWidth = game.BoardSize.Width;
            int boardHeight = game.BoardSize.Height;

            sender.Width = SharedBoardData.CellSize * boardWidth;
            sender.Height = SharedBoardData.CellSize * boardHeight;

            args.DrawingSession.FillRectangle(
                0, 0,
                SharedBoardData.CellSize * boardWidth,
                SharedBoardData.CellSize * boardHeight, 
                _sharedBoardData.BoardColor);

            args.DrawingSession.DrawRectangle(
                0, 0,
                SharedBoardData.CellSize * boardWidth,
                SharedBoardData.CellSize * boardHeight,
                Colors.Black);

            for (int i = 0; i < boardWidth; i++)
            {
                args.DrawingSession.DrawLine(i * SharedBoardData.CellSize, 0, i * SharedBoardData.CellSize, SharedBoardData.CellSize * boardHeight, Colors.Black);
            }

            for (int i = 0; i < boardHeight; i++)
            {
                args.DrawingSession.DrawLine(0, i * SharedBoardData.CellSize, SharedBoardData.CellSize * boardWidth, i * SharedBoardData.CellSize, Colors.Black);
            }

            foreach (var move in game.PrimaryTimeline)
            {
                if (move.WhoMoves == Core.StoneColor.Black)
                    DrawStone(args.DrawingSession, move.Coordinates.X, move.Coordinates.Y, Core.StoneColor.Black);
                else if (move.WhoMoves == Core.StoneColor.White)
                    DrawStone(args.DrawingSession, move.Coordinates.X, move.Coordinates.Y, Core.StoneColor.White);
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
