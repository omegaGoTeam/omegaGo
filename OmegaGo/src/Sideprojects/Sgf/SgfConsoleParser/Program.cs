using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.Core.Game;
using OmegaGo.Core.Game.Markup;
using OmegaGo.Core.Game.GameTreeConversion;

namespace SgfConsoleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            SgfParser sgfParser = new SgfParser();
            SgfCollection collection = sgfParser.Parse(File.ReadAllText("S:\\DELETEME\\markup.sgf"));

            SgfToGameTreeConverter gameTreeConverter = new SgfToGameTreeConverter(collection.First());
            GameTreeNode rootNode = gameTreeConverter.Convert().GameTree.GameTreeRoot;
            //SgfParser.Deserialize(File.ReadAllText("C:\\Users\\Martin\\Downloads\\ff4_ex.sgf"));

            WriteMarkup(rootNode);
        }

        private static void WriteMarkup(GameTreeNode rootNode)
        {
            foreach (GameTreeNode item in rootNode.Branches)
            {
                WriteMarkup(item);
            }

            foreach (var markup in rootNode.Markups.GetMarkups<Cross>())
            {
                Console.WriteLine("Cross");
            }
        }
    }
}
