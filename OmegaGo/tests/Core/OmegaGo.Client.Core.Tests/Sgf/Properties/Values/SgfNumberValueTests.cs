using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values
{
    [TestClass]
    public class SgfNumberValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void ParsingNullNumberValueThrows()
        {
            SgfNumberValue.Parse( null );
        }
    }
}
