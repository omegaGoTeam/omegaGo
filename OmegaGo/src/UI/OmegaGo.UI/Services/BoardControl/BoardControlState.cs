using MvvmCross.Platform.UI;
using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.Services.Game
{
    /// <summary>
    /// Contains information associated with the user control that displays a Go board,
    /// such as styles or highlighted positions.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public sealed class BoardControlState : INotifyPropertyChanged
    {
        private int _boardWidth;
        private int _boardHeight;
        
        private int _boardLineThickness;

        private MvxColor _boardColor;
        private MvxColor _highlightColor;
        private MvxColor _selectionColor;
        
        private Position _mouseOverPosition;
        private Position _shiningPosition;

        public BoardControlState()
        {
            this._mouseOverPosition = Position.Undefined;
            _shiningPosition = Position.Undefined;
            
            _boardLineThickness = 1;

            _boardWidth = 1;
            _boardHeight = 1;

            _boardColor = new MvxColor(0xFD, 0xD2, 0x70, 0xFF);
            _highlightColor = new MvxColor(0xFF, 0xFF, 0xFF, 0x60);
        }

        public BoardControlState(GameBoardSize boardSize) :
            this()
        {
            _boardWidth = boardSize.Width;
            _boardHeight = boardSize.Height;
        }
        

        public int BoardLineThickness
        {
            get { return _boardLineThickness; }
            set { _boardLineThickness = value; OnPropertyChanged(nameof(BoardLineThickness), true); }
        }

        private StoneColor _mouseOverShadowColor;
        public int LeftPadding;
        public int TopPadding;
        public int NewCellSize;

        /// <summary>
        /// Gets or sets the color of the shadow stone displayed under the cursor when the user 
        /// mouses over the board
        /// </summary>
        public StoneColor MouseOverShadowColor
        {
            get { return _mouseOverShadowColor; }
            set { _mouseOverShadowColor = value; OnPropertyChanged(nameof(MouseOverShadowColor), true); }
        }

        public MvxColor BoardColor
        {
            get { return _boardColor; }
            set { _boardColor = value; OnPropertyChanged(nameof(BoardColor), true); }
        }

        public MvxColor HighlightColor
        {
            get { return _highlightColor; }
            set { _highlightColor = value; OnPropertyChanged(nameof(HighlightColor), true); }
        }

        public MvxColor SelectionColor
        {
            get { return _selectionColor; }
            set { _selectionColor = value; OnPropertyChanged(nameof(SelectionColor), true); }
        }

        /// <summary>
        /// Gets or sets the position the mouse is hovering over.
        /// </summary>
        public Position MouseOverPosition
        {
            get { return this._mouseOverPosition; }
            set
            {
                if (value.Equals(this._mouseOverPosition))
                    return;

                this._mouseOverPosition = value;
                OnPropertyChanged(nameof(this.MouseOverPosition), true);
            }
        }
        /// <summary>
        /// Gets or sets the position on a tutorial board that invites the player to tap on it.
        /// </summary>
        public Position ShiningPosition
        {
            get { return _shiningPosition; }
            set
            {
                if (value.Equals(_shiningPosition))
                    return;

                _shiningPosition = value;
                OnPropertyChanged(nameof(ShiningPosition), true);
            }
        }

        /// <summary>
        /// Gets or sets the number of horizontal stones.
        /// </summary>
        public int BoardWidth
        {
            get { return _boardWidth; }
            set
            {
                _boardWidth = value;
                OnPropertyChanged(nameof(BoardWidth), true);
            }
        }

        /// <summary>
        /// Gets or sets the number of vertical stones.
        /// </summary>
        public int BoardHeight
        {
            get { return _boardHeight; }
            set
            {
                _boardHeight = value;
                OnPropertyChanged(nameof(BoardHeight), true);
            }
        }
        

        public event EventHandler RedrawRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName, bool shouldRedraw = false)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (shouldRedraw)
                RedrawRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
