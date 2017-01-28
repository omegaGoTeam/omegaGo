using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Tests.Rules
{
    [TestClass]
    public class AdvancedLegalityTests
    {
        private Ruleset ruleset;
        private TestGame testGame;

        [TestInitialize]
        public void Initialize()
        {
            ruleset = new ChineseRuleset(new GameBoardSize(5));
            testGame = TestGame.New(5, ruleset);
        }

        [TestMethod]
        public void MiddleSuicide()
        {
            testGame.Place("B3", StoneColor.Black)
                .Place("C4", StoneColor.Black)
                .Place("D3", StoneColor.Black)
                .Place("C2", StoneColor.Black);
            Assert.AreEqual(MoveResult.SelfCapture,
                testGame.IsLegal("C3", StoneColor.White));
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("C3", StoneColor.Black));

        }

        [TestMethod]
        public void EyeShape()
        {
            testGame.Place("B3", StoneColor.Black)
                .Place("C4", StoneColor.Black)
                .Place("D4", StoneColor.Black)
                .Place("E3", StoneColor.Black)
                .Place("C2", StoneColor.Black)
                .Place("D2", StoneColor.Black);
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("C3", StoneColor.White));
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("C3", StoneColor.Black));
            testGame.Place("C3", StoneColor.White);
            Assert.AreEqual(MoveResult.SelfCapture,
                testGame.IsLegal("D3", StoneColor.White));
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("D3", StoneColor.Black));
        }

        [TestMethod]
        public void TwoEyes()
        {
            testGame
                .Place("B5", StoneColor.Black)
                .Place("A4", StoneColor.Black)
                .Place("B4", StoneColor.Black)
                .Place("B3", StoneColor.Black)
                .Place("B2", StoneColor.Black)
                .Place("A1", StoneColor.Black)
                .Place("B1", StoneColor.Black)
                .Place("A2", StoneColor.White);
            Assert.AreEqual(MoveResult.SelfCapture,
                testGame.IsLegal("A3", StoneColor.White));
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("C3", StoneColor.White));
            Assert.AreEqual(MoveResult.SelfCapture,
                testGame.IsLegal("A5", StoneColor.White));
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("A5", StoneColor.Black));
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("A3", StoneColor.Black));

            testGame.Place("A3", StoneColor.Black);
            testGame.Place("A2", StoneColor.None);

            Assert.AreEqual(MoveResult.SelfCapture,
                testGame.IsLegal("A2", StoneColor.White));
            Assert.AreEqual(MoveResult.SelfCapture,
                testGame.IsLegal("A5", StoneColor.White));

            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("A2", StoneColor.Black));
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("A5", StoneColor.Black));
        }

        [TestMethod]
        public void Ladder()
        {
            testGame.Place("A1", StoneColor.White)
                .Place("A2", StoneColor.Black);

            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("B1", StoneColor.White));
            testGame.Place("B1", StoneColor.White);

            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("C1", StoneColor.Black));
            testGame.Place("C1", StoneColor.Black);

            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("B2", StoneColor.White));
            testGame.Place("B2", StoneColor.White);

            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("B3", StoneColor.Black));
            testGame.Place("B3", StoneColor.Black);

            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("C2", StoneColor.White));
            testGame.Place("C2", StoneColor.White);

            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("D2", StoneColor.Black));
            testGame.Place("D2", StoneColor.Black);
        }

        [TestMethod]
        public void QuadraKill()
        {
            testGame
                .Place("A3", StoneColor.White)
                .Place("B4", StoneColor.White)
                .Place("C5", StoneColor.White)
                .Place("D4", StoneColor.White)
                .Place("E3", StoneColor.White)
                .Place("D2", StoneColor.White)
                .Place("C1", StoneColor.White)
                .Place("B2", StoneColor.White)
                .Place("B3", StoneColor.Black)
                .Place("C4", StoneColor.Black)
                .Place("D3", StoneColor.Black)
                .Place("C2", StoneColor.Black)
                ;
            Assert.AreEqual(MoveResult.Legal,
                testGame.IsLegal("C3", StoneColor.White));
            Assert.AreEqual(MoveResult.SelfCapture,
                testGame.IsLegal("C3", StoneColor.Black));

        }
    }
}
