using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Tests.Sgf
{
    [TestClass]
    public class SgfNodeTests
    {
        [TestMethod]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void SgfNodeWithNullPropertiesThrowsException()
        {
            new SgfNode( null );
        }

        [TestMethod]
        public void SgfNodeWithEmptyPropertiesCanBeCreatedSuccessfully()
        {
            var node = new SgfNode( new SgfProperty[ 0 ] );
            Assert.AreEqual( 0, node.Count() );
        }

        [TestMethod]
        [ExpectedException( typeof( ArgumentException ) )]
        public void SgfNodeWithRepeatingPropertiesThrowsException()
        {
            var node = new SgfNode( new SgfProperty[]
            {
                new SgfProperty( "FF", new []{SgfNumberValue.Parse("4"), } ),
                new SgfProperty( "FF", new []{SgfNumberValue.Parse("4")} ),
            } );
        }

        [TestMethod]
        public void ValidSgfNodeWithPropertiesCanBeCreated()
        {
            var node = new SgfNode( new []
            {
                new SgfProperty( "FF", new [] { SgfNumberValue.Parse("4")} ), 
                new SgfProperty( "C", new [] { SgfTextValue.Parse("Test")} )
            }  );
            Assert.AreEqual( 2, node.Count() );
            Assert.AreEqual( "FF", node["FF"].Identifier );
            Assert.AreEqual( 4, node["FF"].Value<int>() );
            Assert.AreEqual( "C", node["C"].Identifier );
            Assert.AreEqual( "Test", node[ "C" ].Value<string>() );
        }
    }
}
