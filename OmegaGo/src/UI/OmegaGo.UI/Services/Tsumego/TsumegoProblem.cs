using System.Linq;
using OmegaGo.Core;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.UI.Services.Tsumego
{
    /// <summary>
    /// Represents the definition of a tsumego problem, including its name, problem statement and the game tree.
    /// </summary>
    public class TsumegoProblem
    {
        /// <summary>
        /// Gets the ruleset that is used for tsumego problems (i.e. Chinese 19x19).
        /// </summary>
        public static IRuleset TsumegoRuleset { get; } = Ruleset.Create(RulesetType.Chinese,
            new GameBoardSize(19, 19));

        /// <summary>
        /// Gets the name of the problem.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the color of stones the player will place.
        /// </summary>
        public StoneColor ColorToPlay { get; }

        private SgfGameTree SgfGameTree { get; }

        /// <summary>
        /// Creates a new game tree from the definition of this problem. The returned node is the root of this tree.
        /// </summary>
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
                    node.FillBoardStateOfRoot(new GameBoardSize(19), TsumegoProblem.TsumegoRuleset);
                }
                else
                {
                    if (node.Parent.TsumegoCorrect) node.TsumegoCorrect = true;
                    if (node.Parent.TsumegoWrong) node.TsumegoWrong = true;
                    node.FillBoardState(TsumegoProblem.TsumegoRuleset);
                }
            });
            return tree;
        }

        private TsumegoProblem(string name, SgfGameTree tree, StoneColor colorToPlay)
        {
            this.Name = name;
            this.SgfGameTree = tree;
            this.ColorToPlay = colorToPlay;
        }
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Creates a tsumego problem from the contents of an SGF file downloaded from online-go.com using
        /// the Ruby downloader.
        /// </summary>
        /// <param name="data">The contents of an SGF file.</param>
        /// <returns></returns>
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
            return new TsumegoProblem(problemName, sgfTree, playerToPlay);
        }
    }
}