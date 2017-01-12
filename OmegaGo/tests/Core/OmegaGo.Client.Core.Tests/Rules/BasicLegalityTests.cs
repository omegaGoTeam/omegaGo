using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Tests.Rules
{
    [TestClass]
    public class BasicLegalityTests
    {
        private Ruleset ruleset;
        private TestGame testGame;

        [TestInitialize]
        public void Initialize()
        {
            ruleset = new ChineseRuleset(new GameBoardSize(3));
            testGame = TestGame.New(3);
        }

        [TestMethod]
        public void PlaceStoneOnEmptyBoard()
        {
            Assert.AreEqual(MoveResult.Legal,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("B2", StoneColor.Black), testGame.History));
        }

        [TestMethod]
        public void PlaceStoneOnStone()
        {
            Assert.AreEqual(MoveResult.Legal,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("B2", StoneColor.Black), testGame.History));
            testGame.Place("B2", StoneColor.Black);
            Assert.AreEqual(MoveResult.OccupiedPosition,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("B2", StoneColor.Black), testGame.History));
        }
        [TestMethod]
        public void CornerSuicide() {
            testGame
                .Place("A2", StoneColor.Black)
                .Place("B1", StoneColor.Black)
                ;
            Assert.AreEqual(MoveResult.SelfCapture,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("A1", StoneColor.White), testGame.History));
        }
        [TestMethod]
        public void CornerNonSuicide()
        {
            testGame
                .Place("A2", StoneColor.Black)
                .Place("B1", StoneColor.Black)
                .Place("B2", StoneColor.White)
                .Place("C1", StoneColor.White)
                ;
            Assert.AreEqual(MoveResult.Legal,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("A1", StoneColor.White), testGame.History));
        }
        [TestMethod]
        public void OutsideTheBoard()
        {
            Assert.AreEqual(MoveResult.OutsideTheBoard,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("D1", StoneColor.Black), testGame.History));
            Assert.AreEqual(MoveResult.OutsideTheBoard,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("C4", StoneColor.Black), testGame.History));
            Assert.AreEqual(MoveResult.OutsideTheBoard,
                ruleset.IsLegalMove(testGame.CurrentBoard, testGame.Move("Z25",
                StoneColor.White), testGame.History));
        }

    }
}
