using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    // TODO Keep struct or switch to class?
    //   Petr: I suggest keeping struct. There is no reason as regards code clarity or simpleness to pick over the other, I think, and it's small immutable
    //         data object. There are sometimes structs.
    // TODO Petr: I suggest making this immutable.

    /// <summary>
    /// Represents a single intersection on a Go board.
    /// The position [1, 0], for example, represents the position B1 on the board.
    /// </summary>
    public struct Position
    {
        // We need to be able to easily translate 
        // provided char to range <0, TABLESIZE - 1> .

        // Provided chars should be in range <a,z>

        // ASCI
        // (int)a - 97
        // (int)z - 122
        private const int BEGINASCII = 97;
        private const int ENDASCII = 122;

        private int _x;
        private int _y;

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
            return new Position
            {
                _x = x,
                _y = y - 1
            };
        }

        /// <summary>
        /// Converts a zero-based numeric coordinate to IGS-style character coordinates, omitting the 'I' in the alphabet.
        /// </summary>
        /// <param name="xCoordinate">The coordinate, usually the X (horizontal) coordinate.</param>
        /// <exception cref="System.ArgumentException">We only support boards of sizes up to 25x25.</exception>
        public static char IntToIgsChar(int xCoordinate)
        {
            if (xCoordinate < 0 || xCoordinate >= 25) throw new ArgumentException("We only support boards of sizes up to 25x25.", nameof(xCoordinate));

            char result = (char)('A' + xCoordinate);
            if (result >= 'I')
            {
                result = (char)(result + 1);
            }
            return result;
        }

        /// <summary>
        /// Returns the position int the format X:Y(IGSCOOR), e.g. "8:3(J4)". Use for debugging.
        /// </summary>
        public override string ToString() => X + ":" + Y + "(" + IntToIgsChar(X).ToString() + (Y + 1) + ")";
    }
}
