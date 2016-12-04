using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values
{
    [TestClass]
    public class SgfPointTests
    {
        [TestMethod]
        public void SgfPointComparisonWorks()
        {
            SgfPoint first = new SgfPoint(0, 0);
            SgfPoint second = new SgfPoint(1, 1);
        }
    }
}
