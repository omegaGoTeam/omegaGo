using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed;

namespace OmegaGo.Core.Tests.Modes.LiveGame.Phases.HandicapPlacement.Fixed
{
    [TestClass]
    public class FixedHandicapPositionsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RequestingTooManyPositionsFor9Throws()
        {
            FixedHandicapPositions.GetHandicapStonePositions(new GameBoardSize(9), 6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RequestingTooManyPositionsFor13Throws()
        {
            FixedHandicapPositions.GetHandicapStonePositions(new GameBoardSize(13), 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RequestingTooManyPositionsFor19Throws()
        {
            FixedHandicapPositions.GetHandicapStonePositions(new GameBoardSize(19), 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RequestingPositionsForNonSquareBoardThrows()
        {
            FixedHandicapPositions.GetHandicapStonePositions(new GameBoardSize(9, 10), 6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RequestingPositionsForUnsupportedBoardSizeThrows()
        {
            FixedHandicapPositions.GetHandicapStonePositions(new GameBoardSize(8), 6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaximumHandicapThrowsForUnsupportedBoardSize()
        {
            FixedHandicapPositions.GetMaximumHandicap(new GameBoardSize(8));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MaximumHandicapThrowsForNonSquareBoardSize()
        {
            FixedHandicapPositions.GetMaximumHandicap(new GameBoardSize(9, 8));
        }

        [TestMethod]
        public void SupportedBoardSizesInclude9And13And19()
        {
            var supported = FixedHandicapPositions.GetSupportedBoardSizes().ToList();
            Assert.IsTrue(supported.Contains(new GameBoardSize(9)));
            Assert.IsTrue(supported.Contains(new GameBoardSize(13)));
            Assert.IsTrue(supported.Contains(new GameBoardSize(19)));
        }

        [TestMethod]
        public void MaximumHandicapIsAccordingToRulesFor9And13And19()
        {
            Assert.AreEqual(5, FixedHandicapPositions.GetMaximumHandicap(new GameBoardSize(9)));
            Assert.AreEqual(9, FixedHandicapPositions.GetMaximumHandicap(new GameBoardSize(13)));
            Assert.AreEqual(9, FixedHandicapPositions.GetMaximumHandicap(new GameBoardSize(19)));
        }

        [TestMethod]
        public void ThirdPositionOn9BoardMatches()
        {
            Assert.AreEqual(new Position(6, 2),
                FixedHandicapPositions.GetHandicapStonePositions(new GameBoardSize(9), 3).Last());
        }

        [TestMethod]
        public void NumberOfReturnedPositionsMatchesRequest()
        {
            Assert.AreEqual(7, FixedHandicapPositions.GetHandicapStonePositions(new GameBoardSize(19), 7).Count());
        }
    }
}