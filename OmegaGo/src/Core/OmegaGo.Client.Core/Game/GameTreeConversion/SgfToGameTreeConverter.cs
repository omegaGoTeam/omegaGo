using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game.Markup;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Game.GameTreeConversion
{
    /// <summary>
    /// Converts input SGF Collection to GameTree + GameInfo
    /// </summary>
    public class SgfToGameTreeConverter
    {
        /// <summary>
        /// Input SGF collection
        /// </summary>
        private readonly SgfGameTree _inputTree;

        /// <summary>
        /// Converter for a game tree
        /// </summary>
        /// <param name="gameTree">SGF Game Tree</param>
        public SgfToGameTreeConverter(SgfGameTree gameTree)
        {
            _inputTree = gameTree;
        }

        /// <summary>
        /// Converts the input collection to GameTrees with their respective GameInfos
        /// </summary>
        /// <returns>Converted trees</returns>
        public SgfToGameTreeConversionResult Convert()
        {
            var gameInfo = FindGameInfo(_inputTree);
            var applicationInfo = FindApplicationInfo();
            var gameTree = ConvertTree(_inputTree);
            //set board size
            gameInfo.BoardSize = gameTree.BoardSize;
            return new SgfToGameTreeConversionResult(applicationInfo, gameInfo, gameTree);
        }

        /// <summary>
        /// Finds application info
        /// </summary>
        /// <returns>Application info</returns>
        private ApplicationInfo FindApplicationInfo()
        {
            var values =
                _inputTree.GetPropertyInSequence("AP")
                    ?.PropertyValues.First() as SgfComposePropertyValue<string, string>;
            string name = values?.LeftValue ?? "";
            string version = values?.RightValue ?? "";
            return new ApplicationInfo(name, version);
        }

        /// <summary>
        /// Finds all available game info in the SGF game tree
        /// </summary>
        /// <param name="tree">Tree</param>
        /// <returns>Game info</returns>
        private GameInfo FindGameInfo(SgfGameTree tree)
        {
            var gameInfoSearcher = new SgfGameInfoSearcher(tree);
            var sgfGameInfo = gameInfoSearcher.GetGameInfo();
            return sgfGameInfo.ToGameInfo();
        }

        /// <summary>
        /// Converts a game tree
        /// </summary>
        /// <param name="tree">SGF game tree</param>
        /// <returns>Game tree root</returns>
        private GameTree ConvertTree(SgfGameTree tree)
        {
            var boardSizeInt = tree.GetPropertyInSequence("SZ")?.Value<int>() ?? 19;
            if (boardSizeInt == 0) boardSizeInt = 19;
            GameBoardSize boardSize = new GameBoardSize(boardSizeInt);
            var converted = ConvertBranch(tree, boardSize);
            var ruleset = new ChineseRuleset(boardSize);
            // Post-processing 
            converted.ForAllDescendants((node) => node.Branches, node =>
            {
                if (node.Parent == null)
                {
                    node.FillBoardStateOfRoot(boardSize, ruleset);
                }
                else
                {
                    node.FillBoardState(ruleset);
                }
            });
            var gameTree = new GameTree(new ChineseRuleset(boardSize), boardSize, converted);
            gameTree.LastNode = gameTree.GameTreeRoot;
            return gameTree;
        }

        /// <summary>
        /// Converts a SGF tree branch to GameTreeNode
        /// </summary>
        /// <param name="branch">Branch</param>
        /// <returns>Game tree node</returns>
        private GameTreeNode ConvertBranch(SgfGameTree branch, GameBoardSize boardSize)
        {
            GameTreeNode root = null;
            GameTreeNode current = null;
            foreach (var node in branch.Sequence)
            {
                GameTreeNode newNode = null;
                if (node["W"] != null)
                {
                    SgfPoint point = node["W"].Value<SgfPoint>();
                    //add white move
                    newNode = new GameTreeNode(Move.PlaceStone(StoneColor.White, Position.FromSgfPoint(point, boardSize)));
                }
                else if (node["B"] != null)
                {
                    SgfPoint point = node["B"].Value<SgfPoint>();
                    //add black move
                    newNode = new GameTreeNode(Move.PlaceStone(StoneColor.Black, Position.FromSgfPoint(point, boardSize)));
                }
                else
                {
                    //add non-move
                    newNode = new GameTreeNode(Move.NoneMove);
                }
                if (node["AW"] != null)
                {
                    //add white moves
                    var property = node["AW"];
                    var pointRectangles = property.SimpleValues<SgfPointRectangle>();
                    newNode.AddWhite.AddRange(GetPositionsFromPointRectangles(pointRectangles, boardSize));
                }
                if (node["AB"] != null)
                {
                    var property = node["AB"];
                    var pointRectangles = property.SimpleValues<SgfPointRectangle>();
                    newNode.AddBlack.AddRange(GetPositionsFromPointRectangles(pointRectangles, boardSize));
                }
                if (node["C"] != null)
                {
                    var property = node["C"];
                    var comment = property.Value<string>();
                    newNode.Comment = comment;
                }
                // Fill in all markup properties
                ParseAndFillMarkupProperties(node, newNode, boardSize);
                if (current == null)
                {
                    root = newNode;
                }
                else
                {
                    current.Branches.AddNode(newNode);
                }
                current = newNode;
            }
            //create root if none were found
            if (root == null)
            {
                root = new GameTreeNode(Move.NoneMove);
                current = root;
            }
            //add branches
            foreach (var child in branch.Children)
            {
                var rootOfBranch = ConvertBranch(child, boardSize);
                current.Branches.AddNode(rootOfBranch);
            }
            return root;
        }

        /// <summary>
        /// Parses and fills markup properties
        /// </summary>
        /// <param name="sourceNode">Source SGF node</param>
        /// <param name="targetNode">Target game tree node</param>
        /// <param name="boardSize">Board size</param>
        private void ParseAndFillMarkupProperties(SgfNode sourceNode, GameTreeNode targetNode, GameBoardSize boardSize)
        {
            // Needs to handle: 
            string arrow = "AR"; // AR - Arrow
            string circle = "CR"; // CR - Circle
            string dimPoint = "DD"; // DD - Dim Point
            string label = "LB"; // LB - Label
            string line = "LN"; // LN - Line
            string cross = "MA"; // MA - Mark With X
            string selected = "SL"; // SL - Selected
            string square = "SQ"; // SQ - Square
            string triangle = "TR"; // TR - Triangle

            if (sourceNode[arrow] != null)
            {
                // Add arrow
                var property = sourceNode[arrow];
                var arrowDefinitions = property.ComposeValues<SgfPoint, SgfPoint>();

                foreach (var arrowDefinition in arrowDefinitions)
                {
                    var fromPoint = Position.FromSgfPoint(arrowDefinition.Left, boardSize);
                    var toPoint = Position.FromSgfPoint(arrowDefinition.Right, boardSize);
                    targetNode.Markups.AddMarkup(new Arrow(fromPoint, toPoint));
                }
            }
            if (sourceNode[circle] != null)
            {
                // Add circle
                var property = sourceNode[circle];
                var pointRectangles = property.SimpleValues<SgfPointRectangle>();

                var positions = GetPositionsFromPointRectangles(pointRectangles, boardSize);
                foreach (var position in positions)
                {
                    targetNode.Markups.AddMarkup(
                            new Circle(position));
                }
            }
            if (sourceNode[dimPoint] != null)
            {
                // Add dim point
                var property = sourceNode[dimPoint];
                var pointRectangles = property.SimpleValues<SgfPointRectangle>();
                foreach (var rectangle in pointRectangles)
                {
                    var topLeft = Position.FromSgfPoint(rectangle.UpperLeft, boardSize);
                    var bottomRight = Position.FromSgfPoint(rectangle.LowerRight, boardSize);
                    targetNode.Markups.AddMarkup(new AreaDim(topLeft, bottomRight));
                }
            }
            if (sourceNode[label] != null)
            {
                // Add label
                var property = sourceNode[label];
                var labelDefinitions = property.ComposeValues<SgfPoint, string>();
                foreach (var labelDefinition in labelDefinitions)
                {
                    targetNode.Markups.AddMarkup(new Label(Position.FromSgfPoint(labelDefinition.Left, boardSize), labelDefinition.Right));
                }
            }
            if (sourceNode[line] != null)
            {
                // Add line
                var property = sourceNode[line];
                var lineDefinitions = property.ComposeValues<SgfPoint, SgfPoint>();

                foreach (var lineDefinition in lineDefinitions)
                {
                    var startPoint = Position.FromSgfPoint(lineDefinition.Left, boardSize);
                    var endPoint = Position.FromSgfPoint(lineDefinition.Right, boardSize);
                    targetNode.Markups.AddMarkup(new Line(startPoint, endPoint));
                }
            }
            if (sourceNode[cross] != null)
            {
                // Add cross
                var property = sourceNode[cross];
                var pointRectangles = property.SimpleValues<SgfPointRectangle>();

                var positions = GetPositionsFromPointRectangles(pointRectangles, boardSize);
                foreach (var position in positions)
                {
                    targetNode.Markups.AddMarkup(
                            new Cross(position));
                }
            }
            if (sourceNode[selected] != null)
            {
                // Add selected     
                // TODO  (future work) Vita : implement
            }
            if (sourceNode[square] != null)
            {
                // Add square
                var property = sourceNode[square];
                var pointRectangles = property.SimpleValues<SgfPointRectangle>();

                var positions = GetPositionsFromPointRectangles(pointRectangles, boardSize);
                foreach (var position in positions)
                {
                    targetNode.Markups.AddMarkup(
                            new Square(position));
                }
            }
            if (sourceNode[triangle] != null)
            {
                // Add triangle
                var property = sourceNode[triangle];
                var pointRectangles = property.SimpleValues<SgfPointRectangle>();

                var positions = GetPositionsFromPointRectangles(pointRectangles, boardSize);
                foreach (var position in positions)
                {
                    targetNode.Markups.AddMarkup(
                        new Triangle(position));
                }
            }
        }

        /// <summary>
        /// Gets all positions from point rectangles
        /// </summary>
        /// <param name="pointRectangles">Point rectangles</param>
        /// <param name="boardSize">Board size</param>
        /// <returns>Positions</returns>
        private IEnumerable<Position> GetPositionsFromPointRectangles(IEnumerable<SgfPointRectangle> pointRectangles, GameBoardSize boardSize)
        {
            foreach (var pointRectangle in pointRectangles)
            {
                foreach (var point in pointRectangle)
                {
                    yield return Position.FromSgfPoint(point, boardSize);
                }
            }
        }
    }
}
