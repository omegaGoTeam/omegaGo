using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public sealed class InputService
    {
        private BoardData _sharedBoardData;

        public BoardData SharedBoardData
        {
            get { return _sharedBoardData; }
            private set { _sharedBoardData = value; }
        }

        private GameViewModel _gvm;
        public InputService(BoardData sharedBoardData, GameViewModel gvm)
        {
            SharedBoardData = sharedBoardData;
            _gvm = gvm;
        }

        public void PointerDown(int x, int y)
        {
            
        }

        public void PointerUp(int x, int y)
        {
            SharedBoardData.SelectedPosition = TranslateToBoardPosition(x, y);
            _gvm.ClickOnPosition(SharedBoardData.SelectedPosition);
        }

        public void PointerMoved(int x, int y)
        {
            SharedBoardData.HighlightedPosition = TranslateToBoardPosition(x, y);
        }

        private Position TranslateToBoardPosition(int x, int y)
        {
            Position position;

            x = x - SharedBoardData.BoardBorderThickness;
            y = y - SharedBoardData.BoardBorderThickness;

            if (x > -SharedBoardData.HalfCellSize && x < (_sharedBoardData.BoardActualWidth - SharedBoardData.BoardBorderThickness - SharedBoardData.HalfCellSize) &&
                y > -SharedBoardData.HalfCellSize && y < (_sharedBoardData.BoardActualHeight - SharedBoardData.BoardBorderThickness - SharedBoardData.HalfCellSize))
            {
                position = new Position();
                position.X = (x + SharedBoardData.HalfCellSize) / SharedBoardData.CellSize;
                position.Y = (y + SharedBoardData.HalfCellSize) / SharedBoardData.CellSize;
            }
            else
            {
                position = Position.Undefined;
            }
            
            return position;
        }
    }
}
