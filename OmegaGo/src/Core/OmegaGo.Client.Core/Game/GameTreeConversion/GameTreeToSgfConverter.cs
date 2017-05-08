using OmegaGo.Core.Sgf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game.Markup;
using OmegaGo.Core.Sgf.Properties;
using OmegaGo.Core.Sgf.Properties.Known;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Game.GameTreeConversion
{
    /// <summary>
    /// Converts GameTree to SGF game tree
    /// </summary>
    public class GameTreeToSgfConverter
    {
        private readonly ApplicationInfo _applicationInfo;
        private readonly GameInfo _gameInfo;
        private readonly GameTree _gameTree;

        /// <summary>
        /// Creates a converter for given game info and game tree
        /// </summary>
        /// <param name="applicationInfo">Application info</param>
        /// <param name="gameInfo">Game info</param>
        /// <param name="gameTree">Game tree</param>
        public GameTreeToSgfConverter(ApplicationInfo applicationInfo, GameInfo gameInfo, GameTree gameTree)
        {
            _applicationInfo = applicationInfo;
            _gameInfo = gameInfo;
            _gameTree = gameTree;
        }

        /// <summary>
        /// Performs the conversion
        /// </summary>
        /// <returns>SGF game tree</returns>
        public SgfGameTree Convert()
        {
            var rootGameTree = ProcessNode(_gameTree.GameTreeRoot);
            var rootProperties = GetRootProperties();
            var gameInfoProperties = GetGameInfoProperties();

            //modify the root node
            var firstNode = rootGameTree.Sequence.First();
            var newSequenceNodeList = new List<SgfNode>();

            //check if the first node has a move (mixing move and root properties is not recommended)
            if (firstNode["B"] != null || firstNode["W"] != null)
            {
                //create a new first node
                var newNode = new SgfNode(rootProperties.Union(gameInfoProperties));
                newSequenceNodeList.Add(newNode);
                newSequenceNodeList.AddRange(rootGameTree.Sequence);
            }
            else
            {
                //modify first node
                var newNode = new SgfNode(rootProperties.Union(gameInfoProperties).Union(firstNode.Properties.Values));
                newSequenceNodeList.Add(newNode);
                for (int i = 1; i < rootGameTree.Sequence.Count(); i++)
                {
                    newSequenceNodeList.Add(rootGameTree.Sequence.Nodes[i]);
                }
            }

            return new SgfGameTree(new SgfSequence(newSequenceNodeList), rootGameTree.Children);
        }

        /// <summary>
        /// Processes a game tree node into SGF Game tree
        /// </summary>
        /// <param name="node">Game tree node</param>
        /// <returns>SGF Game tree</returns>
        private SgfGameTree ProcessNode(GameTreeNode node)
        {
            List<SgfNode> currentSequence = new List<SgfNode>();
            var currentNode = node;
            while (currentNode.Branches.Count == 1)
            {
                //continues sequence
                currentSequence.Add(ConvertToSgfNode(currentNode));
                currentNode = currentNode.Branches[0];
            }
            currentSequence.Add(ConvertToSgfNode(currentNode));
            //convert branches
            List<SgfGameTree> subtrees = new List<SgfGameTree>();
            foreach (var branch in currentNode.Branches)
            {
                subtrees.Add(ProcessNode(branch));
            }
            return new SgfGameTree(new SgfSequence(currentSequence), subtrees);
        }

        /// <summary>
        /// Converts game tree node in a SGF node
        /// </summary>
        /// <param name="node">Game tree node</param>
        /// <returns>SGF node</returns>
        private SgfNode ConvertToSgfNode(GameTreeNode node)
        {
            List<SgfProperty> properties = new List<SgfProperty>();

            //parse move property                
            properties.AddIfNotNull(TryGetMoveProperty(node));

            //add comment
            if (!string.IsNullOrEmpty(node.Comment))
            {
                properties.Add(new SgfCommentProperty(node.Comment));
            }

            //setup properties
            properties.AddRange(GetSetupProperties(node));

            //markup properties
            properties.AddRange(GetMarkupProperties(node));

            return new SgfNode(properties);
        }

        /// <summary>
        /// Gets move from the node if available
        /// </summary>
        /// <param name="node">Node to retrieve move from</param>
        /// <returns>Move property or null</returns>
        private SgfProperty TryGetMoveProperty(GameTreeNode node)
        {
            if (node.Move != null)
            {
                switch (node.Move.WhoMoves)
                {
                    case StoneColor.Black:
                        return new SgfProperty("B", new SgfPointValue(node.Move.Coordinates.ToSgfPoint(_gameInfo.BoardSize)));
                    case StoneColor.White:
                        return new SgfProperty("W", new SgfPointValue(node.Move.Coordinates.ToSgfPoint(_gameInfo.BoardSize)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets setup properties from node
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Setup properties</returns>
        private IEnumerable<SgfProperty> GetSetupProperties(GameTreeNode node)
        {
            List<SgfProperty> properties = new List<SgfProperty>();
            if (node.AddBlack.Count > 0)
            {
                //add black
                properties.Add(new SgfAddBlackProperty(ConvertPositionsToPointRectangles(node.AddBlack)));
            }
            if (node.AddWhite.Count > 0)
            {
                //add white
                properties.Add(new SgfAddWhiteProperty(ConvertPositionsToPointRectangles(node.AddWhite)));
            }
            return properties;
        }

        /// <summary>
        /// Gets markup properties from node
        /// </summary>
        /// <param name="node">Node</param>
        /// <returns>Markup properties</returns>
        private IEnumerable<SgfProperty> GetMarkupProperties(GameTreeNode node)
        {
            List<SgfProperty> properties = new List<SgfProperty>();

            var arrows = node.Markups.GetMarkups<Arrow>()
                .Select(ar => new SgfComposeValue<SgfPoint, SgfPoint>(
                    ar.From.ToSgfPoint(_gameInfo.BoardSize),
                    ar.To.ToSgfPoint(_gameInfo.BoardSize))).ToArray();
            if (arrows.Any())
            {
                properties.Add(new SgfArrowProperty(arrows));
            }

            var circles = node.Markups.GetMarkups<Circle>().
                Select(c => c.Position).ToArray();
            properties.AddIfNotNull(ConvertSgfPointRectangleProperty("CR", circles));

            var dimPoints = node.Markups.GetMarkups<AreaDim>().
                Select(dp => new SgfPointRectangle(dp.From.ToSgfPoint(_gameInfo.BoardSize), dp.To.ToSgfPoint(_gameInfo.BoardSize))).
                ToArray();

            if (dimPoints.Any())
            {
                properties.Add(new SgfDimPointProperty(dimPoints));
            }

            var labels = node.Markups.GetMarkups<Label>().
                Select(l => new SgfComposeValue<SgfPoint, string>(l.Position.ToSgfPoint(_gameInfo.BoardSize), l.Text)).
                ToArray();
            if (labels.Any())
            {
                properties.Add(new SgfLabelProperty(labels));
            }

            var lines = node.Markups.GetMarkups<Line>().
                Select(l => new SgfComposeValue<SgfPoint, SgfPoint>(l.From.ToSgfPoint(_gameInfo.BoardSize), l.To.ToSgfPoint(_gameInfo.BoardSize))).
                ToArray();

            if (lines.Any())
            {
                properties.Add(new SgfLineProperty(lines));
            }

            var crosses = node.Markups.GetMarkups<Cross>().
                Select(c => c.Position).ToArray();
            properties.AddIfNotNull(ConvertSgfPointRectangleProperty("MA", crosses));

            var squares = node.Markups.GetMarkups<Square>().
                Select(s => s.Position).ToArray();
            properties.AddIfNotNull(ConvertSgfPointRectangleProperty("SQ", squares));

            var triangles = node.Markups.GetMarkups<Triangle>().
                 Select(s => s.Position).ToArray();
            properties.AddIfNotNull(ConvertSgfPointRectangleProperty("TR", triangles));

            return properties;
        }

        /// <summary>
        /// Retrieves root properties
        /// </summary>
        /// <returns>SGF root properties</returns>
        private IEnumerable<SgfProperty> GetRootProperties()
        {
            List<SgfProperty> properties = new List<SgfProperty>();
            properties.Add(new SgfGameProperty());
            properties.Add(new SgfFileFormatProperty(4));
            properties.Add(new SgfApplicationProperty(_applicationInfo.Name, _applicationInfo.Version));
            properties.Add(new SgfCharsetProperty());
            properties.Add(new SgfStyleProperty(0));
            properties.Add(new SgfSizeProperty(_gameInfo.BoardSize.Width, _gameInfo.BoardSize.Height));
            return properties;
        }

        /// <summary>
        /// Retrieves game info properties
        /// </summary>
        /// <returns>SGF game info properties</returns>
        private IEnumerable<SgfProperty> GetGameInfoProperties()
        {
            List<SgfProperty> properties = new List<SgfProperty>();
            AddPropertyIfNotNullNorEmpty(_gameInfo.Name, "GN", properties);
            AddPropertyIfNotNullNorEmpty(_gameInfo.Date, "DT", properties);
            AddPropertyIfNotNullNorEmpty(_gameInfo.Comment, "GC", properties, true);            
            AddPropertyIfNotNullNorEmpty(_gameInfo.Black.Name, "PB", properties);
            AddPropertyIfNotNullNorEmpty(_gameInfo.Black.Rank, "BR", properties);            
            AddPropertyIfNotNullNorEmpty(_gameInfo.White.Name, "PW", properties);
            AddPropertyIfNotNullNorEmpty(_gameInfo.White.Rank, "WR", properties);
            
            properties.Add(new SgfProperty("HA", new SgfNumberValue(_gameInfo.NumberOfHandicapStones)));
            properties.Add(new SgfProperty("KM", new SgfRealValue((decimal)_gameInfo.Komi)));
            return properties;
        }

        /// <summary>
        /// Adds a game-info property to the list if the string value is not null or empty
        /// </summary>
        private void AddPropertyIfNotNullNorEmpty(string value, string identifier, IList<SgfProperty> target, bool isTextValue = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (!isTextValue)
                {
                    target.Add(new SgfProperty(identifier, new SgfSimpleTextValue(value)));
                }
                else
                {
                    target.Add(new SgfProperty(identifier, new SgfTextValue(value)));
                }
            }
        }

        /// <summary>
        /// Converts a SGF Point Rectangle-based property.
        /// If no positions are provided, the method returns null
        /// </summary>
        /// <param name="identifier">Identifier of the final property</param>
        /// <param name="positions">Positions</param>
        /// <returns>Converted property</returns>
        private SgfProperty ConvertSgfPointRectangleProperty(string identifier, Position[] positions)
        {
            if (positions == null || positions.Length == 0) return null;

            var pointRectangles = ConvertPositionsToPointRectangles(positions);
            return new SgfProperty(identifier,
                pointRectangles.Select(pr => new SgfPointRectangleValue(pr)).ToArray<ISgfPropertyValue>());
        }

        private SgfPointRectangle[] ConvertPositionsToPointRectangles(IEnumerable<Position> positions)
        {
            return SgfPointRectangle.CompressPoints(positions.Select(p => p.ToSgfPoint(_gameInfo.BoardSize))
                .ToArray());
        }
    }
}