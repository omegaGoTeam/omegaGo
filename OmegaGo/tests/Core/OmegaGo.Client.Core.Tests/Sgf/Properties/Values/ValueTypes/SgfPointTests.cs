using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Game;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values.ValueTypes
{
    [TestClass]
    public class SgfPointTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullThrownForNullSgfPointParseAttempt()
        {
            SgfPoint.Parse(null);
        }

        [TestMethod]
        public void SgfPointPassIsCorrectlyParsed()
        {
            var point = SgfPoint.Parse(string.Empty);
            Assert.IsTrue(point.IsInherentlyPass);
        }

        [TestMethod]
        public void SgfPointEqualityIsCorrectlyEvaluated()
        {
            var first = SgfPoint.Parse("fO");
            var second = SgfPoint.Parse("fO");
            var third = SgfPoint.Parse("aa");
            Assert.IsTrue(first.Equals(second));
            Assert.IsFalse(first.Equals(third));
        }

        [TestMethod]
        public void SgfPointEqualityOperatorIsCorrectlyEvaluated()
        {
            var first = SgfPoint.Parse("fO");
            var second = SgfPoint.Parse("fO");
            var third = SgfPoint.Parse("aa");
            Assert.IsTrue(first == second);
            Assert.IsTrue(first != third);
        }

        [TestMethod]
        public void SgfInherentPassPointIsCorrectlySerialized()
        {
            var point = SgfPoint.Pass;
            Assert.AreEqual(string.Empty, point.ToString());
        }

        [TestMethod]
        public void SgfPointsAreCorrectlySerialized()
        {
            var firstPoint = SgfPoint.Parse("Ce");
            var secondPoint = SgfPoint.Parse("ZZ");
            var thirdPoint = SgfPoint.Parse("aa");
            var fourthPoint = SgfPoint.Parse("fO");
            Assert.AreEqual("Ce", firstPoint.ToString());
            Assert.AreEqual("ZZ", secondPoint.ToString());
            Assert.AreEqual("aa", thirdPoint.ToString());
            Assert.AreEqual("fO", fourthPoint.ToString());
        }

        [TestMethod]
        public void NormalSgfPointIsParsedCorrectly()
        {
            var point = SgfPoint.Parse("Ce");
            Assert.AreEqual(28, point.Column);
            Assert.AreEqual(4, point.Row);
            Assert.IsFalse(point.IsInherentlyPass);
        }

        [TestMethod]
        public void TtPointIsNotRecognizedAsPassInherently()
        {
            var point = SgfPoint.Parse("tt");
            Assert.IsFalse(point.IsInherentlyPass);
        }

        [TestMethod]
        public void TtPointIsNotRecognizedAsPassForLargeGameBoard()
        {
            var point = SgfPoint.Parse("tt");
            Assert.IsFalse(point.IsPass(new GameBoardSize(50)));
        }

        [TestMethod]
        public void TtPointIsRecognizedAsPassForSmallerGameBoard()
        {
            var point = SgfPoint.Parse("tt");
            Assert.IsTrue(point.IsPass(new GameBoardSize(19)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooSmallValueIsNotAllowedForSgfPoint()
        {
            new SgfPoint(-3, -3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooLargeValueIsNotAllowedForSgfPoint()
        {
            new SgfPoint(100, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void InvalidPointCantBeParsed()
        {
            SgfPoint.Parse("12");
        }

        [TestMethod]
        public void ToStringSerializesPointProperly()
        {
            var point = SgfPoint.Parse("cc");
            Assert.AreEqual("cc", point.ToString());
        }
    }
}
