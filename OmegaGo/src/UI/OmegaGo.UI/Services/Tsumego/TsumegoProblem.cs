using System;
using System.Linq;
using MvvmCross.Platform;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Utility;

namespace OmegaGo.UI.Services.Tsumego
{
    /// <summary>
    /// Represents the definition of a tsumego problem, including its name, problem statement and the game tree.
    /// </summary>
    public class TsumegoProblem
    {
        // This field causes TsumegoProblem to not work at design time, but who cares.
        protected virtual IGameSettings _settings { get; } = Mvx.Resolve<IGameSettings>();

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

        public GameTreeNode InitialTree { get; }

        public GameBoard InitialBoard => InitialTree.BoardState;
        
        public virtual bool Solved => _settings?.Tsumego.SolvedProblems.Contains(this.Name) ?? false;

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
                        node.Tsumego.Correct = true;
                    }
                    else if (node.Comment.StartsWith("Wrong."))
                    {
                        node.Tsumego.Wrong = true;
                    }
                }
                node.Tsumego.Expected = true;
                if (node.Parent == null)
                {
                    node.FillBoardStateOfRoot(new GameBoardSize(19), TsumegoProblem.TsumegoRuleset);
                }
                else
                {
                    if (node.Parent.Tsumego.Correct) node.Tsumego.Correct = true;
                    if (node.Parent.Tsumego.Wrong) node.Tsumego.Wrong = true;
                    node.FillBoardState(TsumegoProblem.TsumegoRuleset);
                }
                foreach(GameTreeNode continuation in node.Branches)
                {
                    node.Tsumego.MarkedPositions.Add(continuation.Move.Coordinates);
                }
            });
            return tree;
        }

        protected TsumegoProblem(string name, SgfGameTree tree, StoneColor colorToPlay)
        {
            this.Name = name;
            this.SgfGameTree = tree;
            this.ColorToPlay = colorToPlay;
            this.InitialTree = SpawnThisProblem();
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

        public Rectangle GetBoundingBoard()
        {
            var board = InitialBoard;
            int x1 = InitialBoard.Size.Width;
            int y1 = InitialBoard.Size.Height;
            int x2 = 0;
            int y2 = 0;
            for (int x = 0; x < board.Size.Width; x++)
            {
                for (int y= 0;y< board.Size.Height; y++)
                {
                    bool filled = board[x, y] != StoneColor.None;
                    if (filled)
                    {
                        if (x < x1) x1 = x;
                        if (y < y1) y1 = y;
                        if (x > x2) x2 = x;
                        if (y > y2) y2 = y;
                    }
                }
            }
            int safeSpace = 3;
            // Correction
            x1 = Math.Max(0, x1 - safeSpace);
            y1 = Math.Max(0, y1 - safeSpace);
            x2 = Math.Min(InitialBoard.Size.Width - 1, x2 + safeSpace);
            y2 = Math.Min(InitialBoard.Size.Height - 1, y2 + safeSpace);
            int w = x2 - x1 + 1;
            int h = y2 - y1 + 1;
            return new Utility.Rectangle(
                x1,
                y1,
                w,
                h)
            ;
        }
    }
}