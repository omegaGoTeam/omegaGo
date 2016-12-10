using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values
{
    [TestClass]
    public class SgfComposePropertyValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfComposeNullParseThrows()
        {
            SgfComposePropertyValue<string, string>.Parse(null, SgfNumberValue.Parse, SgfNumberValue.Parse);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfComposeLeftParserNullParseThrows()
        {
            SgfComposePropertyValue<string, string>.Parse("1:2", null, SgfNumberValue.Parse);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfComposeRightParserNullParseThrows()
        {
            SgfComposePropertyValue<string, string>.Parse("1:2", SgfNumberValue.Parse, null);
        }
    }
}
