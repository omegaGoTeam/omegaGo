using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    public class HandicapPositions
    {
        public const int MaxFixedHandicap9 = 5;
        public const int MaxFixedHandicap13 = 9;
        public const int MaxFixedHandicap19 = 9;

        public static readonly Position[] FixedHandicapPositions9 = new Position[] { new Position(6, 6),
                                                                     new Position(2, 2),
                                                                     new Position(6, 2),
                                                                     new Position(2, 6),
                                                                     new Position(4, 4) };

        public static readonly Position[] FixedHandicapPositions13 = new Position[] { new Position(9, 9),
                                                                      new Position(3, 3),
                                                                      new Position(9, 3),
                                                                      new Position(3, 9),
                                                                      new Position(9, 6),
                                                                      new Position(3, 6),
                                                                      new Position(6, 9),
                                                                      new Position(6, 3),
                                                                      new Position(6, 6) };

        public static readonly Position[] FixedHandicapPositions19 = new Position[] { new Position(15, 15),
                                                                      new Position(3, 3),
                                                                      new Position(15, 3),
                                                                      new Position(3, 15),
                                                                      new Position(15, 9),
                                                                      new Position(3, 9),
                                                                      new Position(9, 15),
                                                                      new Position(9, 3),
                                                                      new Position(9, 9) };
        public enum Type
        {
            Free,
            Fixed
        }
        
    }
}
