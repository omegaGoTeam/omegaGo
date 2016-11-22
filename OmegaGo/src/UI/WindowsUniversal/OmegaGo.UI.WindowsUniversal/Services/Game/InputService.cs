using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.Services.Game;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public sealed class InputService
    {
        private BoardState _sharedBoardState;

        public BoardState SharedBoardState
        {
            get { return _sharedBoardState; }
            private set { _sharedBoardState = value; }
        }

        public event EventHandler<Position> PointerTapped;
        
        public InputService(BoardState sharedBoardData)
        {
            SharedBoardState = sharedBoardData;
        }

        public void PointerDown(int x, int y)
        {
            
        }

        public void PointerUp(int x, int y)
        {
            Position pointerPosition = TranslateToBoardPosition(x, y);

            SharedBoardState.SelectedPosition = pointerPosition;

            if(pointerPosition.IsDefined)
                PointerTapped(this, pointerPosition);
        }

        public void PointerMoved(int x, int y)
        {
            SharedBoardState.HighlightedPosition = TranslateToBoardPosition(x, y);
        }

        private Position TranslateToBoardPosition(int x, int y)
        {
            Position position;

            x = x - SharedBoardState.BoardBorderThickness;
            y = y - SharedBoardState.BoardBorderThickness;

            if (x > -SharedBoardState.HalfCellSize && x < (_sharedBoardState.BoardActualWidth - SharedBoardState.BoardBorderThickness - SharedBoardState.CellSize) &&
                y > -SharedBoardState.HalfCellSize && y < (_sharedBoardState.BoardActualHeight - SharedBoardState.BoardBorderThickness - SharedBoardState.CellSize))
            {
                position = new Position();
                position.X = (x + SharedBoardState.HalfCellSize) / SharedBoardState.CellSize;
                position.Y = (y + SharedBoardState.HalfCellSize) / SharedBoardState.CellSize;
            }
            else
            {
                position = Position.Undefined;
            }
            
            return position;
        }
    }
}
