using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties;

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
                new SgfProperty( "FF", new []{"4"} ),
                new SgfProperty( "FF", new []{"4"} ),
            } );
        }

        [TestMethod]
        public void ValidSgfNodeWithPropertiesCanBeCreated()
        {
            var node = new SgfNode( new []
            {
                new SgfProperty( "FF", new [] {"4"} ), 
                new SgfProperty( "C", new [] {"Test"} )
            }  );
            Assert.AreEqual( 2, node.Count() );
            Assert.AreEqual( "FF", node["FF"].Identifier );
            Assert.AreEqual( "4", node["FF"].Values.First().Serialize() );
            Assert.AreEqual( "C", node["C"].Identifier );
            Assert.AreEqual( "Test", node[ "C" ].Values.First().Serialize() );
        }
    }
}
