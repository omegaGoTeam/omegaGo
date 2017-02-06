using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Tests.Game.Mocks
{
    public class TestGameTreeNodeInfo
    {
        public TestGameTreeNodeInfo()
        {
            
        }

        public TestGameTreeNodeInfo(string testInfo)
        {
            TestInfo = testInfo;
        }

        public string TestInfo { get; set; } = "hello";
    }
}
