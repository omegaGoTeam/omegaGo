using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using OmegaGo.Core.Game;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.System;
using System.Threading.Tasks;
using Windows.UI.Core;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class TimelineControl : UserControlBase
    {
        private const int NODESIZE = 24;
        private const int NODESPACING = 4;
        private const int NODEHIGHLIGHTSTROKE = 2;
        private const int NODEFONTSIZE = 10;

        private static readonly Color WhiteNodeColor = Colors.White;
        private static readonly Color BlackNodeColor = Colors.Black;
        private static readonly Color EmptyNodeColor = Colors.Maroon;
        private static readonly Color HighlightNodeColor = Colors.Tomato;
        private static readonly Color LineColor = Colors.Gray;

        public static readonly DependencyProperty ViewModelProperty =
                DependencyProperty.Register(
                        "ViewModel",
                        typeof(TimelineViewModel),
                        typeof(TimelineControl),
                        new PropertyMetadata(null, TimelineChanged));
        
        private int _timelineDepth;

        private Dictionary<string, CanvasTextLayout> _textLayoutCache;
        private CanvasTextFormat _textFormat;

        private static void TimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineControl timelineControl = d as TimelineControl;

            if (timelineControl != null)
            {
                TimelineViewModel viewModel = (TimelineViewModel)e.NewValue;
                viewModel.TimelineRedrawRequested += timelineControl.TimelineRedrawRequsted;
                
                timelineControl.PointerReleased += timelineControl.TimelineControl_PointerReleased;
                timelineControl.KeyUp += timelineControl.TimelineControl_KeyUp;
                timelineControl.canvas.Draw += timelineControl.Canvas_Draw;
                timelineControl.canvas.PointerReleased += timelineControl.Canvas_PointerReleased;
                timelineControl.canvas.Invalidate();
            }
        }
        
        public TimelineControl()
        {
            _textLayoutCache = new Dictionary<string, CanvasTextLayout>();
            _textFormat = new CanvasTextFormat()
            {
                FontSize = NODEFONTSIZE,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };

            this.InitializeComponent();
        }

        public TimelineViewModel ViewModel
        {
            get { return (TimelineViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ViewModel?.GameTree == null || ViewModel?.GameTree?.GameTreeRoot == null)
                return base.MeasureOverride(availableSize);

            int requiredHeight = CalculateDesiredSize(ViewModel.GameTree.GameTreeRoot, 0, 0) + 1;
            
            Size desiredSize = new Size();
            desiredSize.Height =
                    requiredHeight * NODESIZE +
                    (requiredHeight - 1) * NODESPACING +
                    2 * NODEHIGHLIGHTSTROKE;                // Add node elliptical stroke for top and bottom

            desiredSize.Width =
                (_timelineDepth + 1) * NODESIZE +
                (_timelineDepth + 1) * NODESPACING +
                2 * NODEHIGHLIGHTSTROKE;                // Add node elliptical stroke for left and right
            
            canvas.Width = desiredSize.Width;
            canvas.Height = desiredSize.Height;

            return base.MeasureOverride(availableSize); 
        }
        
        private void TimelineRedrawRequsted(object sender, EventArgs e)
        {
            // New node could could had been added, in which case the neccessary space could change
            this.InvalidateMeasure();

            // If the desired size had not changed - issue redraw request anyway
            canvas.Invalidate();

            // This will not result in two draws.
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (ViewModel.GameTree != null && ViewModel.GameTree.GameTreeRoot != null)
            {
                args.DrawingSession.Transform = Matrix3x2.CreateTranslation(NODEHIGHLIGHTSTROKE, NODEHIGHLIGHTSTROKE);
                int requiredHeight = DrawNode(args.DrawingSession, ViewModel.GameTree.GameTreeRoot, 0, 0, 0) + 1;
            }
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            GameTreeNode pressedNode;
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            // Compensate position for transformation
            pointerPosition.X -= NODEHIGHLIGHTSTROKE;
            pointerPosition.Y -= NODEHIGHLIGHTSTROKE;

            // First node is empty, ignor it
            if (ViewModel.GameTree.GameTreeRoot == null)
                return;

            GameTreeNode gameTreeRootNode = ViewModel.GameTree.GameTreeRoot;
            GetNodeAtPoint(pointerPosition, gameTreeRootNode, 0, 0, out pressedNode);

            if (pressedNode != null && pressedNode != ViewModel.SelectedTimelineNode)
            {
                ViewModel.SetSelectedNode(pressedNode);
            }
        }

        private int DrawNode(CanvasDrawingSession drawingSession, GameTreeNode node, int depth, int offset, int parentOffset)
        {
            int nodeOffset = offset;

            // Calculate node position
            Vector2 stoneTopLeft = new Vector2(
                depth * NODESIZE + depth * NODESPACING,
                offset * NODESIZE + offset * NODESPACING);

            Vector2 stoneCenter = stoneTopLeft;
            stoneCenter.X += NODESIZE * 0.5f;
            stoneCenter.Y += NODESIZE * 0.5f;

            string nodeText = "";

            // Get text for node
            if (node.Move.Kind == MoveKind.PlaceStone)
                nodeText = node.Move.Coordinates.ToIgsCoordinates();
            else if (node.Move.Kind == MoveKind.Pass)
                nodeText = "P";

            CanvasTextLayout textLayout = GetTextLayoutForString(drawingSession.Device, nodeText);
            // Draw node according to the color of player whose move that was
            if (node.Move.WhoMoves == StoneColor.Black)
            {
                // Black move
                drawingSession.FillEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, BlackNodeColor);
                drawingSession.DrawTextLayout(textLayout, stoneTopLeft, Colors.White);
            }
            else if (node.Move.WhoMoves == StoneColor.White)
            {
                // White move
                drawingSession.FillEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, WhiteNodeColor);
                drawingSession.DrawTextLayout(textLayout, stoneTopLeft, Colors.Black);
            }
            else
            {
                // Empty node, no move
                drawingSession.FillEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, EmptyNodeColor);
            }

            // If the current node is the selected node, draw highlight
            if (node == ViewModel.SelectedTimelineNode)
            {
                drawingSession.DrawEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, HighlightNodeColor, NODEHIGHLIGHTSTROKE);
            }

            // Calculate starting and end points for line connecting two nodes
            Vector2 lineStart = new Vector2(
                (depth) * NODESIZE + (depth - 1) * NODESPACING, 
                (parentOffset) * NODESIZE + parentOffset * NODESPACING + NODESIZE * 0.5f);
            Vector2 lineEnd = new Vector2(
                depth * NODESIZE + depth * NODESPACING,
                offset * NODESIZE + offset * NODESPACING + NODESIZE * 0.5f);

            // Draw line between new node and its parent
            drawingSession.DrawLine(lineStart, lineEnd, LineColor);
            
            // Draw children nodes
            foreach(GameTreeNode childNode in node.Branches)
            {
                offset = DrawNode(drawingSession, childNode, depth + 1, offset, nodeOffset);
                offset++;
            }

            if (node.Branches.Count > 0)
                offset--;

            return offset;
        }

        // Calculating measure
        private int CalculateDesiredSize(GameTreeNode node, int depth, int offset)
        {
            _timelineDepth = Math.Max(_timelineDepth, depth);

            foreach (GameTreeNode childNode in node.Branches)
            {
                offset = CalculateDesiredSize(childNode, depth + 1, offset);
                offset++;
            }

            if (node.Branches.Count > 0)
                offset--;

            return offset;
        }

        // Calculating pointer over node
        private int GetNodeAtPoint(Point point, GameTreeNode node, int depth, int offset, out GameTreeNode resultNode)
        {
            // We draw elipse but hit test rectangle as this is easier.
            Rect nodeRect = new Rect(
                depth * NODESIZE + depth * NODESPACING,
                offset * NODESIZE + offset * NODESPACING,
                NODESIZE, NODESIZE);

            if (nodeRect.Contains(point))
            {
                resultNode = node;
                return offset;
            }
            
            foreach (GameTreeNode childNode in node.Branches)
            {
                GameTreeNode nodeAtPoint;
                offset = GetNodeAtPoint(point, childNode, depth + 1, offset, out nodeAtPoint);
                
                if (nodeAtPoint != null)
                {
                    resultNode = nodeAtPoint;
                    return offset;
                }
                
                offset++;
            }

            if (node.Branches.Count > 0)
                offset--;

            resultNode = null;
            return offset;
        }

        private CanvasTextLayout GetTextLayoutForString(CanvasDevice device, string text)
        {
            if (_textLayoutCache.ContainsKey(text))
                return _textLayoutCache[text];

            CanvasTextLayout textLayout = new CanvasTextLayout(device, text, _textFormat, NODESIZE, NODESIZE);
            _textLayoutCache[text] = textLayout;

            return textLayout;
        }

        //////
        // Keyboard arrows handling
        //////

        private void TimelineControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Hack around UWP XAML Focus WTFiness
            Task.Run(
                async () =>
                {
                    await Task.Delay(100);
                    var task = this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => this.Focus(FocusState.Programmatic));
                });
        }

        private void TimelineControl_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    SwitchToParentNode();
                    break;
                case VirtualKey.Right:
                    SwitchToFirstChildNode();
                    break;
                case VirtualKey.Up:
                    SwitchToPreviousMoveNumberNode();
                    break;
                case VirtualKey.Down:
                    SwitchToNextMoveNumberNode();
                    break;
            }
        }
        
        private void SwitchToPreviousMoveNumberNode()
        {
            GameTreeNode node = ViewModel.SelectedTimelineNode;
            GameTreeNode nodeParent = ViewModel.SelectedTimelineNode.Parent;
            int targetMoveNumber = node.MoveNumber;
            int searchMoveNumber = targetMoveNumber;
            
            while (nodeParent != null)
            {
                int nodeBranchIndex = nodeParent.Branches.IndexOf(node);

                // Search previous siblings
                for (int i = nodeBranchIndex - 1; i >= 0; i--)
                {
                    GameTreeNode searchNode = SwitchToPreviousMoveNumberNodeDFS(nodeParent.Branches[i], searchMoveNumber, targetMoveNumber);

                    if(searchNode != null)
                    {
                        ViewModel.SetSelectedNode(searchNode);
                        return;
                    }
                }

                node = nodeParent;
                nodeParent = nodeParent.Parent;
                searchMoveNumber--;
            }
        }

        private GameTreeNode SwitchToPreviousMoveNumberNodeDFS(GameTreeNode node, int currentMoveIndex, int targetMoveIndex)
        {
            if (currentMoveIndex == targetMoveIndex)
                return node;

            int nodeChildrenCount = node.Branches.Count;

            for (int i = nodeChildrenCount - 1; i >= 0; i--)
            {
                GameTreeNode searchNode = SwitchToPreviousMoveNumberNodeDFS(node.Branches[i], currentMoveIndex + 1, targetMoveIndex);

                if (searchNode != null)
                    return searchNode;
            }

            return null;
        }

        private void SwitchToNextMoveNumberNode()
        {
            GameTreeNode node = ViewModel.SelectedTimelineNode;
            GameTreeNode nodeParent = ViewModel.SelectedTimelineNode.Parent;
            int targetMoveNumber = node.MoveNumber;
            int searchMoveNumber = targetMoveNumber;

            while (nodeParent != null)
            {
                int nodeBranchIndex = nodeParent.Branches.IndexOf(node);

                // Search previous siblings
                for (int i = nodeBranchIndex + 1; i < nodeParent.Branches.Count; i++)
                {
                    GameTreeNode searchNode = SwitchToNextMoveNumberNodeDFS(nodeParent.Branches[i], searchMoveNumber, targetMoveNumber);

                    if (searchNode != null)
                    {
                        ViewModel.SetSelectedNode(searchNode);
                        return;
                    }
                }

                node = nodeParent;
                nodeParent = nodeParent.Parent;
                searchMoveNumber--;
            }
        }

        private GameTreeNode SwitchToNextMoveNumberNodeDFS(GameTreeNode node, int currentMoveIndex, int targetMoveIndex)
        {
            if (currentMoveIndex == targetMoveIndex)
                return node;

            int nodeChildrenCount = node.Branches.Count;

            for (int i = 0; i < nodeChildrenCount; i++)
            {
                GameTreeNode searchNode = SwitchToNextMoveNumberNodeDFS(node.Branches[i], currentMoveIndex + 1, targetMoveIndex);

                if (searchNode != null)
                    return searchNode;
            }

            return null;
        }

        private void SwitchToFirstChildNode()
        {
            GameTreeNode node = ViewModel.SelectedTimelineNode;

            if (node.NextNode != null)
                ViewModel.SetSelectedNode(node.NextNode);
        }

        private void SwitchToParentNode()
        {
            GameTreeNode node = ViewModel.SelectedTimelineNode;

            if (node.Parent != null)
                ViewModel.SetSelectedNode(node.Parent);
        }
    }
}
