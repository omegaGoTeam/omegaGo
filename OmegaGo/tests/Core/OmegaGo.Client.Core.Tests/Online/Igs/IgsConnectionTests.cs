using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Tests.Online.Igs
{
    [TestClass]
    public class IgsConnectionTests
    {
        [TestMethod]        
        public void LoginThrowsWhenUserNameIsNull()
        {
            IgsConnection connection = new IgsConnection();
            var task = connection.LoginAsync(null, "1234");
            Assert.IsTrue(task.Exception.InnerException is ArgumentNullException);
        }

        [TestMethod]        
        public void LoginThrowsWhenPasswordIsNull()
        {
            IgsConnection connection = new IgsConnection();
            var task = connection.LoginAsync("User", null);
            Assert.IsTrue(task.Exception.InnerException is ArgumentNullException);
        }
    }
}
