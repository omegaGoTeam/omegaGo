using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public sealed class BoardData : INotifyPropertyChanged
    {
        private int _cellSize;
        private int _halfCellSize;

        private int _boardWidth;
        private int _boardHeight;

        private Color _boardColor;
        private Color _highlightColor;
        private Color _selectionColor;

        private Position _selectedPosition;
        private Position _highlightedPosition;
        
        public Color BoardColor
        {
            get { return _boardColor; }
            set { _boardColor = value; OnPropertyChanged(nameof(BoardColor), true); }
        }

        public Color HighlightColor
        {
            get { return _highlightColor; }
            set { _highlightColor = value; OnPropertyChanged(nameof(HighlightColor), true); }
        }

        public Color SelectionColor
        {
            get { return _selectionColor; }
            set { _selectionColor = value; OnPropertyChanged(nameof(SelectionColor), true); }
        }
        
        public int CellSize
        {
            get { return _cellSize; }
            set
            {
                _cellSize = value;
                _halfCellSize = value / 2;
                OnPropertyChanged(nameof(CellSize), true);
            }
        }

        public int HalfCellSize
        {
            get { return _halfCellSize; }
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
            set { _boardWidth = value; OnPropertyChanged(nameof(BoardWidth), true); }
        }

        public int BoardHeight
        {
            get { return _boardHeight; }
            set { _boardHeight = value; OnPropertyChanged(nameof(BoardHeight), true); }
        }

        public int BoardRealWidth => CellSize * BoardWidth;
        public int BoardRealHeight => CellSize * BoardHeight;

        public event EventHandler RedrawRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        public BoardData()
        {
            _selectedPosition = Position.Undefined;
            _highlightedPosition = Position.Undefined;

            _cellSize = 32;
            _halfCellSize = _cellSize / 2;

            _boardWidth = 1;
            _boardHeight = 1;

            _boardColor = new Color() { A = 0xFF, B = 0x70, G = 0xD2, R = 0xFD };
            _highlightColor = new Color() { A = 0x60, B = 0xFF, G = 0xFF, R = 0xFF };
            _selectionColor = new Color() { A = 0xA0, B = 0xFF, G = 0xFF, R = 0xFF };
        }

        private void OnPropertyChanged(string propertyName, bool shouldRedraw = false)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (shouldRedraw)
                RedrawRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
