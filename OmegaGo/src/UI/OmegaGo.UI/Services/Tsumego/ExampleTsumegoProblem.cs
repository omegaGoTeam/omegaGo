using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;
using OmegaGo.Core.Game;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.UI.Services.Tsumego
{
    public class ExampleTsumegoProblem : TsumegoProblem
    {
        public ExampleTsumegoProblem() :
            base("Example", new SgfParser().Parse(@"(;FF[4]GM[1]SZ[19]AP[online-go.com:1]
SO[https://online-go.com/puzzle/2824]
GN[Cho Chikun's Encyclopedia of Life and Death - Elementary - 1 / 900]
AW[ab][bb][cb][db][da]
AB[bc][cc][dc][eb][fb][be]
PL[B]C[Welcome to this work-in-progress. I'm adding the rest of the puzzles every so often, and the first few are available (albeit they're all ranked as 25kyu, incorrectly).

Feel free to leave feedback!

Last updated at February 26th, 2016.

---

# Black to kill.]
(;B[ba]C[Correct.])
(;B[ca];W[ba]C[Wrong.])
(;B[aa];W[ba]C[Wrong.])
(;B[ac];W[ba]C[Wrong.])
(;B[ea];W[ba]C[Wrong.])
)").First(), StoneColor.Black)
        {
        }
    }
}
