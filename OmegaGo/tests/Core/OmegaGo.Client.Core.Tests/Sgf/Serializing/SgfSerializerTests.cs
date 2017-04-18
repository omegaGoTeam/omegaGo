using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.Core.Sgf.Serializing;

namespace OmegaGo.Core.Tests.Sgf.Serializing
{
    [TestClass]
    public class SgfSerializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SerializeThrowsForNullInput()
        {
            new SgfSerializer().Serialize(null);
        }

        [TestMethod]
        public void MinimalGameTreeIsSuccessfullySerialized()
        {
            var targetSgf = "(;)";
            var parser = new SgfParser();
            var collection = parser.Parse(targetSgf);
            var serialized = new SgfSerializer().Serialize(collection);
            Assert.AreEqual(targetSgf,serialized);            
        }

        [TestMethod]
        public void CollectionOfTwoMinimalGameTreesIsSuccessfullySerialized()
        {
            var targetSgf = "(;)(;)";
            var parser = new SgfParser();
            var collection = parser.Parse(targetSgf);
            var serialized = new SgfSerializer().Serialize(collection);
            Assert.AreEqual(targetSgf, serialized);
        }

        [TestMethod]
        public void SimpleSgfInputIsSuccessfullySerialized()
        {
            var targetSgf =
                @"(;FF[4]C[root](;C[a];C[b](;C[c])(;C[d];C[e]))(;C[f](;C[g];C[h];C[i])(;C[j];LB[ab:Hello world!])))";
            var parser = new SgfParser();
            var collection = parser.Parse(targetSgf);
            var serialized = new SgfSerializer().Serialize(collection);
            Assert.AreEqual(targetSgf, serialized);
        }        
    }
}

