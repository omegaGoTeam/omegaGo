﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF point
    /// </summary>
    public struct SgfPoint : IEquatable<SgfPoint>, IComparable<SgfPoint>
    {
        /// <summary>
        /// Creates a SGF Point
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="row">Row</param>
        public SgfPoint(int column, int row)
        {
            if (column < -1 || column > 52) throw new ArgumentOutOfRangeException(nameof(column));
            if (row < -1 || row > 52) throw new ArgumentOutOfRangeException(nameof(row));
            Column = column;
            Row = row;
        }

        /// <summary>
        /// Parses a SGF point from a property value
        /// </summary>
        /// <param name="value">Property value</param>
        /// <returns>SGF point</returns>
        public static SgfPoint Parse(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 2)
                throw new SgfParseException($"Invalid SGF point value: '{value}'");
            var columnChar = value[0];
            var rowChar = value[1];
            return new SgfPoint(
                    PointCharToValue(columnChar),
                    PointCharToValue(rowChar)
                );
        }

        /// <summary>
        /// Pass move
        /// </summary>
        public static SgfPoint Pass { get; } = new SgfPoint(-1, -1);

        /// <summary>
        /// Converts a SGF based char to internal coordinate value
        /// </summary>
        /// <param name="character">Character from SGF file</param>
        /// <returns>Coordinate value in SGF point structure</returns>
        private static int PointCharToValue(char character)
        {
            var isEnglishLower = (character >= 'a' && character <= 'z');
            var isEnglishUpper = (character >= 'A' && character <= 'Z');

            if (isEnglishLower)
            {
                return character - 'a';
            }
            else if (isEnglishUpper)
            {
                return character - 'A';
            }
            else
            {
                //invalid                
                throw new SgfParseException($"Invalid SGF point char: '{character}'");
            }
        }

        /// <summary>
        /// Value to point char
        /// </summary>
        /// <param name="value">Internal struct coordinate value</param>
        /// <returns>SGF coordinate char</returns>
        private static char ValueToPointChar(int value)
        {
            if (value < 0 || value > 52) throw new ArgumentOutOfRangeException(nameof(value));
            return (char)(value < 27 ? 'a' + value : 'A' + value);
        }

        /// <summary>
        /// Column
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Row
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Checks if the point represents a pass move
        /// </summary>
        public bool IsPass => Column == -1;

        public static bool operator ==(SgfPoint left, SgfPoint right) => left.Equals(right);

        public static bool operator !=(SgfPoint left, SgfPoint right) => !(left == right);

        /// <summary>
        /// Checks for SGF point equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SgfPoint other) => Row == other.Row && Column == other.Column;

        /// <summary>
        /// Checks for equality
        /// </summary>
        /// <param name="obj">Second instance</param>
        /// <returns>Are instances equal?</returns>
        public override bool Equals(object obj)
        {
            var secondPoint = obj as SgfPoint?;
            if (secondPoint != null)
            {
                return Equals(secondPoint.Value);
            }
            return false;
        }

        /// <summary>
        /// Hash code of the SGF point
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode() => Row * 101 + Column;

        /// <summary>
        /// String representation of the SGF point
        /// </summary>
        /// <returns>SGF value</returns>
        public override string ToString()
        {
            //handle pass move
            if (IsPass) return string.Empty;

            char columnChar = ValueToPointChar( Column );
            char rowChar = ValueToPointChar( Row );
        }
    }
}
