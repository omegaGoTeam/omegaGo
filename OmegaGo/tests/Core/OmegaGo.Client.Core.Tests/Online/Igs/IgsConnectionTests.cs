using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Client.Core.Tests.Online.Igs
{
    [TestClass]
    public class IgsConnectionTests
    {
        [TestMethod]        
        public void LoginThrowsWhenUserNameIsNull()
        {
            IgsConnection connection = new IgsConnection();
            var task = connection.Login(null, "1234");
            Assert.IsTrue(task.Exception.InnerException is ArgumentNullException);
        }

        [TestMethod]        
        public void LoginThrowsWhenPasswordIsNull()
        {
            IgsConnection connection = new IgsConnection();
            var task = connection.Login("User", null);
            Assert.IsTrue(task.Exception.InnerException is ArgumentNullException);
        }
    }
}
