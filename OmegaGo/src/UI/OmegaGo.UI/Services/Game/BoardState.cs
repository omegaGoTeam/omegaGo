using MvvmCross.Platform.UI;
using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Game
{
    public sealed class BoardState : INotifyPropertyChanged
    {
        private int _cellSize;
        private int _halfCellSize;

        private int _boardWidth;
        private int _boardHeight;
        private int _boardActualWidth;
        private int _boardActualHeight;

        private int _boardBorderThickness;
        private int _boardLineThickness;

        private MvxColor _boardColor;
        private MvxColor _highlightColor;
        private MvxColor _selectionColor;

        private Position _selectedPosition;
        private Position _highlightedPosition;

        public int CellSize
        {
            get { return _cellSize; }
            set
            {
                _cellSize = value;
                _halfCellSize = value / 2;
                UpdateActualBoardSize();
                OnPropertyChanged(nameof(CellSize), true);
            }
        }

        public int HalfCellSize => _halfCellSize;

        public int BoardBorderThickness
        {
            get { return _boardBorderThickness; }
            set { _boardBorderThickness = value; UpdateActualBoardSize(); OnPropertyChanged(nameof(BoardBorderThickness), true); }
        }

        public int BoardLineThickness
        {
            get { return _boardBorderThickness; }
            set { _boardBorderThickness = value; OnPropertyChanged(nameof(BoardLineThickness), true); }
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

        public Position SelectedPosition
        {
            get { return _selectedPosition; }
            set
            {
                if (value.Equals(_selectedPosition))
                    return;

                _selectedPosition = value;
                OnPropertyChanged(nameof(SelectedPosition), true);
            }
        }

        public Position HighlightedPosition
        {
            get { return _highlightedPosition; }
            set
            {
                if (value.Equals(_highlightedPosition))
                    return;

                _highlightedPosition = value;
                OnPropertyChanged(nameof(HighlightedPosition), true);
            }
        }

        public int BoardWidth
        {
            get { return _boardWidth; }
            set { _boardWidth = value; UpdateActualBoardSize(); OnPropertyChanged(nameof(BoardWidth), true); }
        }

        public int BoardHeight
        {
            get { return _boardHeight; }
            set { _boardHeight = value; UpdateActualBoardSize(); OnPropertyChanged(nameof(BoardHeight), true); }
        }

        public int BoardActualWidth => _boardActualWidth;
        public int BoardActualHeight => _boardActualHeight;

        public event EventHandler RedrawRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public BoardState()
        {
            _selectedPosition = Position.Undefined;
            _highlightedPosition = Position.Undefined;

            _cellSize = 32;
            _halfCellSize = _cellSize / 2;

            _boardBorderThickness = 40;
            _boardLineThickness = 1;

            _boardWidth = 1;
            _boardHeight = 1;
            UpdateActualBoardSize();
            
            _boardColor = new MvxColor(0xFD, 0xD2, 0x70, 0xFF);
            _highlightColor = new MvxColor(0xFF, 0xFF, 0xFF, 0x60);
            _selectionColor = new MvxColor(0xFF, 0xFF, 0xFF, 0xA0);
        }

        private void OnPropertyChanged(string propertyName, bool shouldRedraw = false)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (shouldRedraw)
                RedrawRequested?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateActualBoardSize()
        {
            _boardActualWidth = BoardWidth * CellSize + 2 * BoardBorderThickness;
            _boardActualHeight = BoardHeight * CellSize + 2 * BoardBorderThickness;
        }
    }
}
