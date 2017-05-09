using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Game;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.Core.Tests.Sgf;
using OmegaGo.Core.Game.GameTreeConversion;

namespace OmegaGo.Core.Tests.Game
{
    [TestClass]
    public class GameTreeConverterTests
    {
        [TestMethod]
        public void ComplexGameTreeCanBeConverted()
        {
            var parsedSgfTree = SgfTestHelpers.ParseFile(new SgfParser(), "Valid/ff4_ex.sgf");
            SgfToGameTreeConverter gameTreeConverter = new SgfToGameTreeConverter(parsedSgfTree.First());
            var gameTree = gameTreeConverter.Convert();
        }
    }
}
