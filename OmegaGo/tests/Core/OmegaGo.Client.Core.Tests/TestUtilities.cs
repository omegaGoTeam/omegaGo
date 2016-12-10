using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OmegaGo.Core.Tests
{
    public static class TestUtilities
    {
        public static void AssertStringEqualityByCharacters( string expected, string actual )
        {
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
