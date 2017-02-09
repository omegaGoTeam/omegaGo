using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Local;

namespace OmegaGo.Core.Tests.Modes.LiveGame.Players
{
    [TestClass]
    public class PlayerPairTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PlayerPairThrowsForNullFirstPlayer()
        {
            var players = new PlayerPair(null, new HumanPlayerBuilder(StoneColor.Black).Build());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PlayerPairThrowsForNullSecondPlayer()
        {
            var players = new PlayerPair(null, new HumanPlayerBuilder(StoneColor.Black).Build());
        }

        [TestMethod]        
        public void PlayersAreSetProperlyToTheRightColors()
        {
            var black = new HumanPlayerBuilder(StoneColor.Black).Build();
            var white = new HumanPlayerBuilder(StoneColor.White).Build();
            var firstOrderPlayers = new PlayerPair(black, white);
            var secondOrderPlayers = new PlayerPair(white, black);
            Assert.AreEqual(black, firstOrderPlayers.Black);
            Assert.AreEqual(black, secondOrderPlayers.Black);
            Assert.AreEqual(white, firstOrderPlayers.White);
            Assert.AreEqual(white, secondOrderPlayers.White);
        }

        [TestMethod]        
        public void IndexerReturnsCorrectPlayers()
        {
            var black = new HumanPlayerBuilder(StoneColor.Black).Build();
            var white = new HumanPlayerBuilder(StoneColor.White).Build();
            var players = new PlayerPair(black, white);
            Assert.AreEqual(black, players[StoneColor.Black]);            
            Assert.AreEqual(white, players[StoneColor.White]);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PlayerPairThrowsForSameColorPlayers()
        {
            var black = new HumanPlayerBuilder(StoneColor.Black).Build();
            var secondBlack = new HumanPlayerBuilder(StoneColor.Black).Build();
            var players = new PlayerPair(black, secondBlack);
        }
    }
}
