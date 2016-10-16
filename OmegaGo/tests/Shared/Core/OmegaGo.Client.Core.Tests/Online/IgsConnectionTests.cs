using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Online;

namespace OmegaGo.Client.Core.Tests.Online
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
