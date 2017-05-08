using System;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents a single intersection on a Go board.
    /// The position [1, 0], for example, represents the position B1 on the board.
    /// 
    /// Whenever the game board is represented as a two-dimensional array, the first coordinate is the column ("X-coordinate") and the second coordinate is the row ("Y-coordinate"). For example, "currentBoard[2,5]" refers to the intersection C6, i.e. position [2,5].
    /// </summary>
    public struct Position : IEquatable<Position>
    {

        // We need to be able to easily translate 
        // provided char to range <0, TABLESIZE - 1> .

        // Provided chars should be in range <a,z>

        // ASCI
        // (int)a - 97
        // (int)z - 122
        private const int BEGINASCII = 97;
        // private const int ENDASCII = 122;

        public static readonly Position Undefined = new Position(-1, -1);

        private int _x;
        private int _y;

        /// <summary>
        /// Converts SGF point to Position
        /// </summary>
        /// <param name="sgfPoint">SGF point</param>
        /// <param name="size">Game board size is used for reversing Y coordinates.</param>
        /// <returns>Position</returns>
        internal static Position FromSgfPoint(SgfPoint sgfPoint, GameBoardSize size) => 
            new Position(sgfPoint.Column, size.Height - sgfPoint.Row - 1);

        internal SgfPoint ToSgfPoint( GameBoardSize size) => 
            new SgfPoint(X, size.Height - Y - 1);

        /// <summary>
        /// Gets or sets the letter-based coordinate that is usually put first (e.g. the "C" in "C6"). It is zero-based, i.e. it would be "2" for C6.
        /// </summary>
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Gets or sets the number-based coordinate that usually goes second (e.g. the "13" in "F13"). It is zero-based, i.e. it would be "12" for F13.
        /// </summary>
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public char CharX
        {
            get { return ToChar(X); }
            set { X = ToInt(value); }
        }

        public char CharY
        {
            get { return ToChar(Y); }
            set { Y = ToInt(value); }
        }

        private char ToChar(int i)
        {
            int charNum = i + BEGINASCII;

            return (char)charNum;
        }

        private int ToInt(char c)
        {
            int tablePos = (int)c - BEGINASCII;

            return tablePos;
        }

        public bool IsDefined => _x != -1;

        /// <summary>
        /// Creates a new <see cref="Position"/> instance from coordinates given in the IGS format. The IGS format coordinates go from A1 to Z25, with
        /// the letter "I" being skipped. IGS coordinates are used by the IGS server. 
        /// </summary>
        /// <param name="coordinates">Coordinates in the IGS format.</param>
        public static Position FromIgsCoordinates(string coordinates)
        {
            if (coordinates == null) throw new ArgumentNullException(nameof(coordinates));
            if (coordinates.Length < 2 || coordinates.Length > 3) throw new ArgumentException(nameof(coordinates));
            if (coordinates[0] < 'A' || coordinates[0] > 'Z' || coordinates[0] == 'I')
                throw new ArgumentException(nameof(coordinates));

            int y;
            if (!int.TryParse(coordinates.Substring(1), out y))
            {
                throw new ArgumentException(nameof(coordinates));
            }

            char xc = coordinates[0];
            int x = xc - 'A';
            if (xc >= 'J')
            {
                x--;
            }
            return new Position(x, y - 1);
        }

        /// <summary>
        /// Converts a zero-based numeric coordinate to IGS-style character coordinates, omitting the 'I' in the alphabet.
        /// </summary>
        /// <param name="xCoordinate">The coordinate, usually the X (horizontal) coordinate.</param>
        /// <exception cref="System.ArgumentException">We only support boards of sizes up to 25x25.</exception>
        public static string IntToIgsChar(int xCoordinate)
        {
            if (xCoordinate < 0 || xCoordinate >= 25)
            {
                return "[" + xCoordinate + "]";
            }
            char result = (char)('A' + xCoordinate);
            if (result >= 'I')
            {
                result = (char)(result + 1);
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns the position in the format "J4". Use for debugging.
        /// </summary>
        public override string ToString() => IntToIgsChar(X).ToString() + (Y + 1);
        // In case we need to debug numerics again: 
        // Returns the position in the format X:Y(IGSCOOR), e.g. "8:3(J4)". Use for debugging.
        //public override string ToString() => X + ":" + Y + "(" + IntToIgsChar(X).ToString() + (Y + 1) + ")";

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="x">The zero-based x-coordinate ("column").</param>
        /// <param name="y">The zero-based y-coordinate ("row").</param>
        public Position(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        /// <summary>
        /// Returns the position as IGS-style coordinates, e.g. "J4" or "C11".
        /// </summary>
        public string ToIgsCoordinates() => IntToIgsChar(X).ToString() + (Y + 1);

        public bool Equals(Position position)
        {
            return position.X == this.X && position.Y == this.Y;
        }
        public static bool operator ==(Position position, Position position2)
        {
            return position.Equals(position2);
        }
        public static bool operator !=(Position position, Position position2)
        {
            return !position.Equals(position2);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Position && Equals((Position)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this._x * 397) ^ this._y;
            }
        }

    }
}
