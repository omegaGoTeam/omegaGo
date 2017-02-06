using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Game;
using OmegaGo.Core.Tests.Game.Mocks;

namespace OmegaGo.Core.Tests.Game
{
    [TestClass]
    public class GameTreeNodeTests
    {
        [TestMethod]
        public void TsumegoIsAlwaysNonNull()
        {
            GameTreeNode node = new GameTreeNode(Move.NoneMove);
            Assert.IsNotNull(node.Tsumego);
        }

        [TestMethod]
        public void CustomNodeInfoCanBeCreatedAutomatically()
        {
            GameTreeNode node = new GameTreeNode(Move.NoneMove);
            var info = node.GetOrCreateNodeInfo<TestGameTreeNodeInfo>();
            Assert.IsNotNull(info);
            Assert.AreEqual("hello", info.TestInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void CustomNodeInfoGetterThrowsWhenNotAvailable()
        {
            GameTreeNode node = new GameTreeNode();
            var info = node.GetNodeInfo<TestGameTreeNodeInfo>();
        }

        [TestMethod]
        public void CustomNodeInfoCanBeCreatedWithFunctor()
        {
            GameTreeNode node = new GameTreeNode();
            var info = node.GetOrCreateNodeInfo(() => new TestGameTreeNodeInfo("test"));
            Assert.IsNotNull(info);
            Assert.AreEqual("test", info.TestInfo);
        }

        [TestMethod]
        public void ModificationsOfNodeInfoOutsideArePreserved()
        {
            GameTreeNode node = new GameTreeNode();
            var info = node.GetOrCreateNodeInfo<TestGameTreeNodeInfo>();
            info.TestInfo = "new value";
            var newInfo = node.GetOrCreateNodeInfo<TestGameTreeNodeInfo>();
            Assert.AreEqual("new value", newInfo.TestInfo);
        }

        [TestMethod]
        public void NodeInfoCanBeStored()
        {
            GameTreeNode node = new GameTreeNode();
            var info = new TestGameTreeNodeInfo("custom");
            node.SetNodeInfo(info);
            var retrievedInfo = node.GetNodeInfo<TestGameTreeNodeInfo>();
            Assert.IsNotNull(retrievedInfo);
            Assert.AreEqual("custom", retrievedInfo.TestInfo);
        }

        [TestMethod]
        public void NodeInfoIsNotRecreated()
        {
            GameTreeNode node = new GameTreeNode();
            var info = new TestGameTreeNodeInfo("custom");
            node.SetNodeInfo(info);
            var retrievedInfo = node.GetOrCreateNodeInfo<TestGameTreeNodeInfo>();
            Assert.IsNotNull(retrievedInfo);
            Assert.AreEqual("custom", retrievedInfo.TestInfo);
        }

        [TestMethod]
        public void NodeInfoIsNotRecreatedWithFunctor()
        {
            GameTreeNode node = new GameTreeNode();
            var info = new TestGameTreeNodeInfo("custom");
            node.SetNodeInfo(info);
            var retrievedInfo = node.GetOrCreateNodeInfo(() => new TestGameTreeNodeInfo("different"));
            Assert.IsNotNull(retrievedInfo);
            Assert.AreEqual("custom", retrievedInfo.TestInfo);
        }

        [TestMethod]
        public void NewerSetOverwritesPreviousNodeInfo()
        {
            GameTreeNode node = new GameTreeNode();
            var originalInfo = new TestGameTreeNodeInfo("original");
            var updatedInfo = new TestGameTreeNodeInfo("new");
            node.SetNodeInfo(originalInfo);
            node.SetNodeInfo(updatedInfo);
            var retrievedInfo = node.GetNodeInfo<TestGameTreeNodeInfo>();
            Assert.IsNotNull(retrievedInfo);
            Assert.AreEqual("new", retrievedInfo.TestInfo);
        }
    }
}
