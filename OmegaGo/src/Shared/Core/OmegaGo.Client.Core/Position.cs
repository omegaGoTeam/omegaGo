using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    // TODO Keep struct or switch to class?
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

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

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
    }
}
