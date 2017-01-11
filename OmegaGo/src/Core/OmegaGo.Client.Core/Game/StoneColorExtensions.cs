﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game
{
    public static class StoneColorExtensions
    {
        public static StoneColor GetOpponentColor(this StoneColor color)
        {
            if (color == StoneColor.Black) return StoneColor.White;
            if (color == StoneColor.White) return StoneColor.Black;
            return StoneColor.None;
        }
    }
}
