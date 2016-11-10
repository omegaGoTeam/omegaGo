using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public InputService(BoardData sharedBoardData)
        {
            SharedBoardData = sharedBoardData;
        }

        public void PointerDown(int x, int y)
        {
            
        }

        public void PointerUp(int x, int y)
        {
            SharedBoardData.SelectedPosition = TranslateToBoardPosition(x, y);
        }

        public void PointerMoved(int x, int y)
        {
            SharedBoardData.HighlightedPosition = TranslateToBoardPosition(x, y);
        }

        private Position TranslateToBoardPosition(int x, int y)
        {
            Position position;

            if (x > 0 && x < _sharedBoardData.BoardRealWidth &&
                y > 0 && y < _sharedBoardData.BoardRealHeight)
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
