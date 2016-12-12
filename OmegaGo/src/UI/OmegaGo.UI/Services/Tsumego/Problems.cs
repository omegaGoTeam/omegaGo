using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Tsumego
{
    class Problems
    {
        static Problems()
        {
            AllProblems.Add(new Tsumego.TsumegoProblem("01 - 01"));
            AllProblems.Add(new Tsumego.TsumegoProblem("01 - 02"));
        }

        public static List<TsumegoProblem> AllProblems = new List<TsumegoProblem>();
    }
}
