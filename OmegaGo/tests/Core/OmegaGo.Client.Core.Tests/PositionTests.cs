using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core;

namespace OmegaGo.Client.Core.Tests
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void IgsCoordinatesAreWellConverted()
        {
            Assert.AreEqual(new Position(0, 0), Position.FromIgsCoordinates("A1"));
            Assert.AreEqual(new Position(8, 8), Position.FromIgsCoordinates("J9"));
            Assert.AreEqual(new Position(4, 4), Position.FromIgsCoordinates("E5"));
            Assert.AreEqual(new Position(8, 7), Position.FromIgsCoordinates("J8"));
            Assert.AreEqual(new Position(18, 17), Position.FromIgsCoordinates("T18"));
            Assert.AreEqual(new Position(24, 24), Position.FromIgsCoordinates("Z25"));
        }
        [TestMethod]
        public void NumberToIgsChar()
        {
            Assert.AreEqual('A', Position.IntToIgsChar(0));
            Assert.AreEqual('J', Position.IntToIgsChar(8));
            Assert.AreEqual('C', Position.IntToIgsChar(2));
            Assert.AreEqual('Y', Position.IntToIgsChar(23));

        }
    }
}
