using System.Linq;
using OmegaGo.Core;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.UI.Services.Tsumego
{
    public class TsumegoProblem
    {
        private static IRuleset tsumegoRuleset = Ruleset.Create(RulesetType.Chinese, new GameBoardSize(19, 19));
        public string Name { get; }
        public StoneColor PlayerToPlay { get; }
        private SgfGameTree SgfGameTree { get; }

        public GameTreeNode SpawnThisProblem()
        {
            var tree = GameTreeConverter.FromSgfGameTree(SgfGameTree);
            tree.ForAllDescendants((node) => node.Branches, node =>
            {
                if (node.Comment != null)
                {
                    if (node.Comment.StartsWith("Correct."))
                    {
                        node.TsumegoCorrect = true;
                    }
                    else if (node.Comment.StartsWith("Wrong."))
                    {
                        node.TsumegoWrong = true;
                    }
                }
                node.TsumegoExpected = true;
                if (node.Parent == null)
                {
                    node.FillBoardStateOfRoot(new GameBoardSize(19), tsumegoRuleset);
                }
                else
                {
                    node.FillBoardState(tsumegoRuleset);
                }
            });
            return tree;
        }

        private TsumegoProblem(string name, SgfGameTree tree, StoneColor playerToPlay)
        {
            this.Name = name;
            this.SgfGameTree = tree;
            this.PlayerToPlay = playerToPlay;
        }
        public override string ToString()
        {
            return Name;
        }

        public static TsumegoProblem CreateFromSgfText(string data)
        {
            SgfParser parser = new SgfParser();
            var collection =  parser.Parse(data);
            SgfGameTree sgfTree = collection.GameTrees.First();
            string problemName = "";
            StoneColor playerToPlay = StoneColor.None;
            foreach(var node in sgfTree.Sequence)
            {
                if (node["GN"] != null)
                {
                    problemName = node["GN"].Value<string>();
                }
                if (node["PL"] != null)
                {
                    SgfColor sgfColor = node["PL"].Value<SgfColor>();
                    switch (sgfColor)
                    {
                        case SgfColor.Black:
                            playerToPlay = StoneColor.Black;
                            break;
                        case SgfColor.White:
                            playerToPlay = StoneColor.White;
                            break;
                    }
                }
            }
            var tree = GameTreeConverter.FromSgfGameTree(sgfTree);
            return new TsumegoProblem(problemName, collection.GameTrees.First(), playerToPlay);
        }
    }
}