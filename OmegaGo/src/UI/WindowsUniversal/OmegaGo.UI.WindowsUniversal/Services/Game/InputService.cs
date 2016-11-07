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
        
        public InputService(BoardData sharedBoardData)
        {
            _sharedBoardData = sharedBoardData;
        }

        public void PointerDown(int x, int y)
        {
            
        }

        public void PointerUp(int x, int y)
        {
            if (x > 0 && x < _sharedBoardData.BoardRealWidth &&
                y > 0 && y < _sharedBoardData.BoardRealHeight)
            {
                Position position = new Position();
                position.X = (x + BoardData.HalfCellSize) / BoardData.CellSize;
                position.Y = (y + BoardData.HalfCellSize) / BoardData.CellSize;

                _sharedBoardData.SelectedPosition = position;
            }
            else
            {
                _sharedBoardData.SelectedPosition = Position.Undefined;
            }
        }

        public void PointerMoved(int x, int y)
        {
            if (x > 0 && x < _sharedBoardData.BoardRealWidth &&
                y > 0 && y < _sharedBoardData.BoardRealHeight)
            {
                Position position = new Position();
                position.X = (x + BoardData.HalfCellSize) / BoardData.CellSize;
                position.Y = (y + BoardData.HalfCellSize) / BoardData.CellSize;
                
                _sharedBoardData.HighlightedPosition = position;
            }
            else
            {
                _sharedBoardData.HighlightedPosition = Position.Undefined;
            }
        }
    }
}
