using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;

namespace OmegaGo.Core.Tests.Sgf
{
    [TestClass]
    public class SgfCollectionTests
    {
        [TestMethod]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void ConstructorThrowsWhenNullIsSupplied()
        {
            SgfCollection collection = new SgfCollection( null );
        }
    }
}