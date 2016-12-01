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


        /// <summary>
        /// Handles pointer pressed event.
        /// </summary>
        /// <param name="x">position on x axis</param>
        /// <param name="y">position on y axis</param>
        public void PointerDown(int x, int y)
        {
            
        }


        /// <summary>
        /// Handles pointer released event.
        /// </summary>
        /// <param name="x">position on x axis</param>
        /// <param name="y">position on y axis</param>
        public void PointerUp(int x, int y)
        {
            Position pointerPosition = TranslateToBoardPosition(x, y);

            SharedBoardState.SelectedPosition = pointerPosition;

            if(pointerPosition.IsDefined)
                PointerTapped(this, pointerPosition);
        }


        /// <summary>
        /// Handles pointer moved event.
        /// </summary>
        /// <param name="x">position on x axis</param>
        /// <param name="y">position on y axis</param>
        public void PointerMoved(int x, int y)
        {
            SharedBoardState.HighlightedPosition = TranslateToBoardPosition(x, y);
        }

        /// <summary>
        /// Translates screen position to map position.
        /// </summary>
        /// <param name="x">map position on x axis</param>
        /// <param name="y">map position on y axis</param>
        /// <returns>a board position</returns>
        private Position TranslateToBoardPosition(int x, int y)
        {
            Position position;

            x = x - SharedBoardState.BoardBorderThickness;
            y = y - SharedBoardState.BoardBorderThickness;

            if (x > 0 && x < (_sharedBoardState.BoardActualWidth - 2 * SharedBoardState.BoardBorderThickness) &&
                y > 0 && y < (_sharedBoardState.BoardActualHeight - 2 * SharedBoardState.BoardBorderThickness))
            {
                position = new Position();
                position.X = x / SharedBoardState.CellSize;
                // Take into account that we number the other way around, where the zero would be, we have max index.
                // First calculate zero based index and then subtract from maximum index to get the resulting position.
                position.Y = (SharedBoardState.BoardHeight - 1) - (y / SharedBoardState.CellSize);
            }
            else
            {
                position = Position.Undefined;
            }

            return position;
        }
    }
}
