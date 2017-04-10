using MvvmCross.Platform.UI;
using OmegaGo.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;
using OmegaGo.UI.Utility;

namespace OmegaGo.UI.Services.Game
{
    /// <summary>
    /// Contains information associated with the user control that displays a Go board,
    /// such as styles or highlighted positions.
    /// </summary>
    public sealed class BoardControlState
    {
        private int _boardWidth;
        private int _boardHeight;
        
        private int _boardLineThickness;

        private MvxColor _boardColor;
        private MvxColor _highlightColor;
        private MvxColor _selectionColor;
        
        private Position _pointerOverPosition;
        private StoneColor _pointerOverShadowColor;
        private Position _shiningPosition;

        public BoardControlState()
        {
            _pointerOverPosition = Position.Undefined;
            _shiningPosition = Position.Undefined;
            
            _boardLineThickness = 1;

            _boardWidth = 1;
            _boardHeight = 1;

            _boardColor = new MvxColor(0xFD, 0xD2, 0x70, 0xFF);
            _highlightColor = new MvxColor(0xFF, 0xFF, 0xFF, 0x60);
        }

        public BoardControlState(GameBoardSize boardSize) 
            : this()
        {
            _boardWidth = boardSize.Width;
            _boardHeight = boardSize.Height;
        }
        public BoardControlState(Rectangle rectangle) 
            : this()
        {
            this.OriginX = rectangle.X;
            this.OriginY = rectangle.Y;
            _boardWidth = rectangle.Width;
            _boardHeight = rectangle.Height;
            TsumegoMode = true;
        }


        public int BoardLineThickness
        {
            get { return _boardLineThickness; }
            set { _boardLineThickness = value; }
        }

        public bool TsumegoMode { get; }
        public int LeftPadding { get; set; }
        public int TopPadding { get; set; }
        public int NewCellSize { get; set; }
        public int OriginX { get; set; }
        public int OriginY { get; set; }

        /// <summary>
        /// Gets or sets the color of the shadow stone displayed under the cursor when the user 
        /// mouses over the board
        /// </summary>
        public StoneColor PointerOverShadowColor
        {
            get { return _pointerOverShadowColor; }
            set { _pointerOverShadowColor = value; }
        }

        public MvxColor BoardColor
        {
            get { return _boardColor; }
            set { _boardColor = value; }
        }

        public MvxColor HighlightColor
        {
            get { return _highlightColor; }
            set { _highlightColor = value; }
        }

        public MvxColor SelectionColor
        {
            get { return _selectionColor; }
            set { _selectionColor = value; }
        }

        /// <summary>
        /// Gets or sets the position the mouse is hovering over.
        /// </summary>
        public Position PointerOverPosition
        {
            get { return _pointerOverPosition; }
            set { _pointerOverPosition = value; }
        }
        /// <summary>
        /// Gets or sets the position on a tutorial board that invites the player to tap on it.
        /// </summary>
        public Position ShiningPosition
        {
            get { return _shiningPosition; }
            set { _shiningPosition = value; }
        }

        /// <summary>
        /// Gets or sets the number of horizontal stones.
        /// </summary>
        public int BoardWidth
        {
            get { return _boardWidth; }
            set { _boardWidth = value; }
        }

        /// <summary>
        /// Gets or sets the number of vertical stones.
        /// </summary>
        public int BoardHeight
        {
            get { return _boardHeight; }
            set { _boardHeight = value; }
        }

        public bool ShowTerritory { get; set; }
        public TerritoryMap TerritoryMap { get; set; }

        /// <summary>
        /// Gets or sets an array determining whether it is legal to make a move on a given coordinates.
        /// This is a temporary property, to be replaced when tools are implemented.
        /// </summary>
        public MoveResult[,] TEMP_MoveLegality { get; set; }        
    }
}
