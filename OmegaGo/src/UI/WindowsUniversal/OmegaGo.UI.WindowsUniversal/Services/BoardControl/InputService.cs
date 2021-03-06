﻿using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.Services.Game;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public sealed class InputService
    {
        private BoardControlState _sharedBoardControlState;

        public BoardControlState SharedBoardControlState
        {
            get { return _sharedBoardControlState; }
            private set { _sharedBoardControlState = value; }
        }

        public event EventHandler<Position> PointerTapped;
        
        public InputService(BoardControlState sharedBoardControlData)
        {
            SharedBoardControlState = sharedBoardControlData;
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
            SharedBoardControlState.PointerOverPosition = pointerPosition;

            if (pointerPosition.IsDefined)
                PointerTapped?.Invoke(this, pointerPosition);
        }


        /// <summary>
        /// Handles pointer moved event.
        /// </summary>
        /// <param name="x">position on x axis</param>
        /// <param name="y">position on y axis</param>
        public void PointerMoved(int x, int y)
        {
            Position pointerPosition = TranslateToBoardPosition(x, y);
            SharedBoardControlState.PointerOverPosition = pointerPosition;
        }

        /// <summary>
        /// Handles pointer left event.
        /// </summary>
        public void PointerExited()
        {
            SharedBoardControlState.PointerOverPosition = Position.Undefined;
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

            x = x - SharedBoardControlState.LeftPadding;
            y = y - SharedBoardControlState.TopPadding;

            if (x > 0 && x < (_sharedBoardControlState.NewCellSize * _sharedBoardControlState.BoardWidth) &&
                y > 0 && y < (_sharedBoardControlState.NewCellSize * _sharedBoardControlState.BoardHeight))
            {
                position = new Position();
                position.X = x / SharedBoardControlState.NewCellSize + _sharedBoardControlState.OriginX;
                // Take into account that we number the other way around, where the zero would be, we have max index.
                // First calculate zero based index and then subtract from maximum index to get the resulting position.
                position.Y = (SharedBoardControlState.BoardHeight - 1) - (y / SharedBoardControlState.NewCellSize) + _sharedBoardControlState.OriginY;
            }
            else
            {
                position = Position.Undefined;
            }

            return position;
        }
    }
}
