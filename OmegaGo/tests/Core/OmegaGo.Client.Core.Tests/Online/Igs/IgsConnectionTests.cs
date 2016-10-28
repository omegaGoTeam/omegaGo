using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Client.Core.Tests.Online.Igs
{
    [TestClass]
    public class IgsConnectionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoginThrowsWhenUserNameIsNull()
        {
            IgsConnection connection = new IgsConnection();
            connection.Login(null, "1234");
        }

        [TestMethod]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void LoginThrowsWhenPasswordIsNull()
        {
            IgsConnection connection = new IgsConnection();
            connection.Login( "User", null );
        }
    }
}
