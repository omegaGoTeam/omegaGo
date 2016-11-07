using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.WindowsUniversal.Services.Game
{
    public sealed class BoardData : INotifyPropertyChanged
    {
        public const int CellSize = 32;
        public const int HalfCellSize = CellSize / 2;
        public Windows.UI.Color BoardColor = new Windows.UI.Color() { A = 0xFF, B = 0x70, G = 0xD2, R = 0xFD };
        public Windows.UI.Color HighlightColor = new Windows.UI.Color() { A = 0x60, B = 0xFF, G = 0xFF, R = 0xFF };
        public Windows.UI.Color SelectionColor = new Windows.UI.Color() { A = 0xA0, B = 0xFF, G = 0xFF, R = 0xFF };

        private Position _selectedPosition;
        private Position _highlightedPosition;
        private int _boardWidth;
        private int _boardHeight;

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

            _boardWidth = 1;
            _boardHeight = 1;
        }

        private void OnPropertyChanged(string propertyName, bool shouldRedraw = false)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (shouldRedraw)
                RedrawRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
