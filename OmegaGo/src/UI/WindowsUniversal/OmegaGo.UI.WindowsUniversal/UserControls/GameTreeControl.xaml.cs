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
using Windows.Devices.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class GameTreeControl : UserControlBase
    {
        private const int NODESIZE = 24;
        private const int NODESPACING = 4;
        private const int NODEHIGHLIGHTSTROKE = 2;
        private const int NODECOMBINEDSIZE = NODESIZE + NODESPACING + NODEHIGHLIGHTSTROKE;
        private const int NODEFONTSIZE = 10;
        private const string NODEPASSTEXT = "P";
        
        private const double POINTERMOVETOLERANCE = 2.5d;

        private static readonly Color WhiteNodeColor = Colors.White;
        private static readonly Color BlackNodeColor = Colors.Black;
        private static readonly Color EmptyNodeColor = Colors.Maroon;
        private static readonly Color HighlightNodeColor = Colors.Tomato;
        private static readonly Color LineColor = Colors.Gray;
        
        private int _gameTreeVerticalSize;

        private Dictionary<string, CanvasTextLayout> _textLayoutCache;
        private CanvasTextFormat _textFormat;

        private bool _isPointerDown;
        private double _pointerMoveDifference;
        private Point _pointerCurrentPosition = new Point();
        
        public GameTreeControl()
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

        public static readonly DependencyProperty ViewModelProperty =
                DependencyProperty.Register(
                        "ViewModel",
                        typeof(GameTreeViewModel),
                        typeof(GameTreeControl),
                        new PropertyMetadata(null, GameTreeVMChanged));

        public static readonly DependencyProperty GameTreeWidthProperty =
                DependencyProperty.Register(
                        "GameTreeWidth",
                        typeof(double),
                        typeof(GameTreeControl),
                        new PropertyMetadata(0d, GameTreeRenderPropertyChanged));

        public static readonly DependencyProperty GameTreeHeightProperty =
                DependencyProperty.Register(
                        "GameTreeHeight",
                        typeof(double),
                        typeof(GameTreeControl),
                        new PropertyMetadata(0d, GameTreeRenderPropertyChanged));

        public static readonly DependencyProperty GameTreeVerticalOffsetProperty =
                DependencyProperty.Register(
                        "GameTreeVerticalOffset",
                        typeof(double),
                        typeof(GameTreeControl),
                        new PropertyMetadata(0d, GameTreeRenderPropertyChanged));

        public static readonly DependencyProperty GameTreeHorizontalOffsetProperty =
                DependencyProperty.Register(
                        "GameTreeHorizontalOffset",
                        typeof(double),
                        typeof(GameTreeControl),
                        new PropertyMetadata(0d, GameTreeRenderPropertyChanged));

        public GameTreeViewModel ViewModel
        {
            get { return (GameTreeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public double GameTreeWidth
        {
            get { return (double)GetValue(GameTreeWidthProperty); }
            set { SetValue(GameTreeWidthProperty, value); }
        }

        public double GameTreeHeight
        {
            get { return (double)GetValue(GameTreeHeightProperty); }
            set { SetValue(GameTreeHeightProperty, value); }
        }

        public double GameTreeVerticalOffset
        {
            get { return (double)GetValue(GameTreeVerticalOffsetProperty); }
            set { SetValue(GameTreeVerticalOffsetProperty, value); }
        }

        public double GameTreeHorizontalOffset
        {
            get { return (double)GetValue(GameTreeHorizontalOffsetProperty); }
            set { SetValue(GameTreeHorizontalOffsetProperty, value); }
        }

        private static void GameTreeVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GameTreeControl GameTreeControl = d as GameTreeControl;

            if (GameTreeControl != null)
            {
                GameTreeViewModel viewModel = (GameTreeViewModel)e.NewValue;
                viewModel.GameTreeRedrawRequested += GameTreeControl.GameTreeRedrawRequsted;

                // GameTree scrolling
                GameTreeControl.gameTreeRoot.PointerEntered += GameTreeControl.GameTreeControl_PointerEntered;
                GameTreeControl.gameTreeRoot.PointerExited += GameTreeControl.GameTreeControl_PointerExited;
                GameTreeControl.gameTreeRoot.PointerWheelChanged += GameTreeControl.GameTreeControl_PointerWheelChanged;

                // Arrows handling
                // This control has public methods to navigate using arrow keys.
                // These methods are being called by owning view.

                // Drawing
                GameTreeControl.canvas.Draw += GameTreeControl.Canvas_Draw;
                // GameTree scrolling and node highlighting
                GameTreeControl.canvas.PointerReleased += GameTreeControl.Canvas_PointerReleased;
                GameTreeControl.canvas.PointerMoved += GameTreeControl.Canvas_PointerMoved;
                GameTreeControl.canvas.PointerPressed += GameTreeControl.Canvas_PointerPressed;

                // Relaculate desired GameTree size and redraw
                GameTreeControl.UpdateGameTreeSize();
                GameTreeControl.canvas.Invalidate();
            }
        }

        //////
        // Public methods for manipulating selected node
        //////

        /// <summary>
        /// Switches selected node in the game tree to the node at the same move index in previous branch.
        /// </summary>
        public void GoToPreviousLevelNode()
        {
            SwitchToPreviousLevelNode();
        }

        /// <summary>
        /// Switches selected node in the game tree to the node at the same move index in next branch.
        /// </summary>
        public void GoToNextLevelNode()
        {
            SwitchToNextLevelNode();
        }

        /// <summary>
        /// Switches selected node in the game tree to the first child node of the current node.
        /// </summary>
        public void GoToFirstChildNode()
        {
            SwitchToFirstChildNode();
        }

        /// <summary>
        /// Switches selected node in the game tree to the parent node of the current node.
        /// </summary>
        public void GoToParentNode()
        {
            SwitchToParentNode();
        }

        //////
        // handlers
        //////

        private void GameTreeControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isPointerDown = false;

            horizontalBar.IndicatorMode = ScrollingIndicatorMode.None;
            verticalBar.IndicatorMode = ScrollingIndicatorMode.None;
        }

        private void GameTreeControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ScrollingIndicatorMode indicatorMode = ScrollingIndicatorMode.None;

            switch (e.Pointer.PointerDeviceType)
            {
                case PointerDeviceType.Touch:
                    indicatorMode = ScrollingIndicatorMode.TouchIndicator;
                    break;
                case PointerDeviceType.Pen:
                case PointerDeviceType.Mouse:
                    indicatorMode = ScrollingIndicatorMode.MouseIndicator;
                    break;
            }

            horizontalBar.IndicatorMode = indicatorMode;
            verticalBar.IndicatorMode = indicatorMode;
        }

        private static void GameTreeRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GameTreeControl GameTreeControl = d as GameTreeControl;

            if (GameTreeControl != null)
            {
                GameTreeControl.canvas.Invalidate();
            }
        }

        private void GameTreeRedrawRequsted(object sender, EventArgs e)
        {
            // New node could could had been added, in which case the neccessary space could change
            UpdateGameTreeSize();

            // Issue redraw request
            canvas.Invalidate();
        }
        
        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (ViewModel.GameTree != null && ViewModel.GameTree.GameTreeRoot != null)
            {
                args.DrawingSession.Transform = 
                    Matrix3x2.CreateTranslation(
                        NODEHIGHLIGHTSTROKE - (float)GameTreeHorizontalOffset, 
                        NODEHIGHLIGHTSTROKE - (float)GameTreeVerticalOffset);

                DrawGameTree(args.DrawingSession, ViewModel.GameTree.GameTreeRoot);
            }
        }

        //////
        // GameTree Pointer input
        //////

        private void GameTreeControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pointerPoint = e.GetCurrentPoint(this);

            bool isHorizontal = pointerPoint.Properties.IsHorizontalMouseWheel;
            int wheelDelta = pointerPoint.Properties.MouseWheelDelta;
            
            bool hasOffsetChanged = false;

            // Make sure we are not behind bounds, this method checks that
            if (isHorizontal)
            {
                double oldOffset = GameTreeHorizontalOffset;
                SetScrollOffset(GameTreeHorizontalOffset + wheelDelta, GameTreeVerticalOffset);

                hasOffsetChanged = (oldOffset != GameTreeHorizontalOffset);
            }
            else
            {
                double oldOffset = GameTreeVerticalOffset;
                SetScrollOffset(GameTreeHorizontalOffset, GameTreeVerticalOffset - wheelDelta);

                hasOffsetChanged = (oldOffset != GameTreeVerticalOffset);
            }

            // If the offset changed, mark the event as handled so that any parent scrollviewer wont scroll.
            // (We want to perform only one scrolling per wheel changed)
            e.Handled = hasOffsetChanged;
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // If the pointer is of touch type, then allow dragging the GameTree
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                HandleTouchPointerDown(e.GetCurrentPoint(canvas).Position);
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // If the pointer is of touch type, then allow dragging the GameTree
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                HandleTouchPointerMove(e.GetCurrentPoint(canvas).Position);
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            // If the pointer is of touch type, then check the move ammount
            // If the move ammout tolerance is within bounds then highlight node at touch position
            // If the pointer is mouse / pen then just highlight node position as no dragging is allowed
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                HandleTouchPointerUp(pointerPosition);
            else
                HighlightNodeAtPointerPosition(pointerPosition);
        }

        private void HighlightNodeAtPointerPosition(Point pointerPosition)
        {
            GameTreeNode pressedNode;

            // Compensate position for transformation
            pointerPosition.X = pointerPosition.X - NODEHIGHLIGHTSTROKE + GameTreeHorizontalOffset;
            pointerPosition.Y = pointerPosition.Y - NODEHIGHLIGHTSTROKE + GameTreeVerticalOffset;

            // First node is empty, ignor it
            if (ViewModel.GameTree.GameTreeRoot == null)
                return;

            GameTreeNode gameTreeRootNode = ViewModel.GameTree.GameTreeRoot;
            pressedNode = GetNodeAtPoint(pointerPosition, gameTreeRootNode);

            if (pressedNode != null && pressedNode != ViewModel.SelectedGameTreeNode)
            {
                ViewModel.SetSelectedNode(pressedNode);
            }
        }

        //////
        // GameTree drawing
        //////

        private void DrawGameTree(CanvasDrawingSession drawingSession, GameTreeNode gameTreeRoot)
        {
            int resultVerticalOffset = 0;
            WalkGameTree(gameTreeRoot, 0, ref resultVerticalOffset, 0,
                (node, horizontalOffset, verticalOffset, parentVerticalOffset) =>
                {
                    // Calculate node position
                    Vector2 stoneTopLeft = new Vector2(
                        horizontalOffset * NODESIZE + horizontalOffset * NODESPACING,
                        verticalOffset * NODESIZE + verticalOffset * NODESPACING);

                    Vector2 stoneCenter = stoneTopLeft;
                    stoneCenter.X += NODESIZE * 0.5f;
                    stoneCenter.Y += NODESIZE * 0.5f;

                    string nodeText = "";

                    // Get text for node
                    if (node.Move.Kind == MoveKind.PlaceStone)
                        nodeText = node.Move.Coordinates.ToIgsCoordinates();
                    else if (node.Move.Kind == MoveKind.Pass)
                        nodeText = NODEPASSTEXT;

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
                    if (node == ViewModel.SelectedGameTreeNode)
                    {
                        drawingSession.DrawEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, HighlightNodeColor, NODEHIGHLIGHTSTROKE);
                    }

                    // Calculate starting and end points for line connecting two nodes
                    Vector2 lineStart = new Vector2(
                        (horizontalOffset) * NODESIZE + (horizontalOffset - 1) * NODESPACING,
                        (parentVerticalOffset) * NODESIZE + parentVerticalOffset * NODESPACING + NODESIZE * 0.5f);
                    Vector2 lineEnd = new Vector2(
                        horizontalOffset * NODESIZE + horizontalOffset * NODESPACING,
                        verticalOffset * NODESIZE + verticalOffset * NODESPACING + NODESIZE * 0.5f);

                    // Draw line between new node and its parent
                    drawingSession.DrawLine(lineStart, lineEnd, LineColor);

                    return GameTreeNodeCallbackResultBehavior.Continue;
                });
        }

        // Calculating measure
        private int CalculateDesiredSize(GameTreeNode node)
        {
            int resultVerticalOffset = 0;
            WalkGameTree(node, 0, ref resultVerticalOffset, 0, 
                (currentNode, horizontalOffset, verticalOffset, parentVerticalOffset) => 
                {
                    _gameTreeVerticalSize = Math.Max(_gameTreeVerticalSize, horizontalOffset);
                    return GameTreeNodeCallbackResultBehavior.Continue;
                });

            return resultVerticalOffset;
        }

        // Calculating pointer over node
        private GameTreeNode GetNodeAtPoint(Point point, GameTreeNode node)
        {
            GameTreeNode resultNode = null;

            int resultVerticalOffset = 0;
            WalkGameTree(node, 0, ref resultVerticalOffset, 0,
                (currentNode, horizontalOffset, verticalOffset, parentVerticalOffset) =>
                {
                    Rect nodeRect = new Rect(
                        horizontalOffset * NODESIZE + horizontalOffset * NODESPACING,
                        verticalOffset * NODESIZE + verticalOffset * NODESPACING,
                        NODESIZE, NODESIZE);

                    if (nodeRect.Contains(point))
                    {
                        resultNode = currentNode;
                        return GameTreeNodeCallbackResultBehavior.Stop;
                    }

                    return GameTreeNodeCallbackResultBehavior.Continue;
                });
            
            return resultNode;
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
        
        private void SwitchToPreviousLevelNode()
        {
            GameTreeNode node = ViewModel.SelectedGameTreeNode;
            GameTreeNode nodeParent = ViewModel.SelectedGameTreeNode.Parent;
            int targetMoveNumber = node.MoveNumber;
            int searchMoveNumber = targetMoveNumber;
            
            while (nodeParent != null)
            {
                int nodeBranchIndex = nodeParent.Branches.IndexOf(node);

                // Search previous siblings
                for (int i = nodeBranchIndex - 1; i >= 0; i--)
                {
                    GameTreeNode searchNode = SwitchToPreviousLevelNodeDFS(nodeParent.Branches[i], searchMoveNumber, targetMoveNumber);

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

        private GameTreeNode SwitchToPreviousLevelNodeDFS(GameTreeNode node, int currentMoveIndex, int targetMoveIndex)
        {
            if (currentMoveIndex == targetMoveIndex)
                return node;

            int nodeChildrenCount = node.Branches.Count;

            for (int i = nodeChildrenCount - 1; i >= 0; i--)
            {
                GameTreeNode searchNode = SwitchToPreviousLevelNodeDFS(node.Branches[i], currentMoveIndex + 1, targetMoveIndex);

                if (searchNode != null)
                    return searchNode;
            }

            return null;
        }

        private void SwitchToNextLevelNode()
        {
            GameTreeNode node = ViewModel.SelectedGameTreeNode;
            GameTreeNode nodeParent = ViewModel.SelectedGameTreeNode.Parent;
            int targetMoveNumber = node.MoveNumber;
            int searchMoveNumber = targetMoveNumber;

            while (nodeParent != null)
            {
                int nodeBranchIndex = nodeParent.Branches.IndexOf(node);

                // Search previous siblings
                for (int i = nodeBranchIndex + 1; i < nodeParent.Branches.Count; i++)
                {
                    GameTreeNode searchNode = SwitchToNextLevelNodeDFS(nodeParent.Branches[i], searchMoveNumber, targetMoveNumber);

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

        private GameTreeNode SwitchToNextLevelNodeDFS(GameTreeNode node, int currentMoveIndex, int targetMoveIndex)
        {
            if (currentMoveIndex == targetMoveIndex)
                return node;

            int nodeChildrenCount = node.Branches.Count;

            for (int i = 0; i < nodeChildrenCount; i++)
            {
                GameTreeNode searchNode = SwitchToNextLevelNodeDFS(node.Branches[i], currentMoveIndex + 1, targetMoveIndex);

                if (searchNode != null)
                    return searchNode;
            }

            return null;
        }

        private void SwitchToFirstChildNode()
        {
            GameTreeNode node = ViewModel.SelectedGameTreeNode;

            if (node.NextNode != null)
            {
                ViewModel.SetSelectedNode(node.NextNode);
            }
        }

        private void SwitchToParentNode()
        {
            GameTreeNode node = ViewModel.SelectedGameTreeNode;

            if (node.Parent != null)
            {
                ViewModel.SetSelectedNode(node.Parent);
            }
        }

        //////
        // Custom scrolling implementation
        //////

        private void UpdateGameTreeSize()
        {
            if (ViewModel?.GameTree == null || ViewModel?.GameTree?.GameTreeRoot == null)
                return;

            int requiredHeight = CalculateDesiredSize(ViewModel.GameTree.GameTreeRoot) + 1;

            double height = requiredHeight * NODESIZE +
                            (requiredHeight - 1) * NODESPACING +
                            2 * NODEHIGHLIGHTSTROKE;        // Add node elliptical stroke for top and bottom

            double width = (_gameTreeVerticalSize + 1) * NODESIZE +
                            (_gameTreeVerticalSize + 1) * NODESPACING +
                            2 * NODEHIGHLIGHTSTROKE;        // Add node elliptical stroke for left and right

            GameTreeHeight = height - canvas.ActualHeight;  // Subtract from the entire required height what we can display
            GameTreeWidth = width - canvas.ActualWidth;     // Subtract from the entire required width what we can display

            // Make sure we are not behing bounds (could happen when branch get deleted)
            SetScrollOffset(
                GameTreeHorizontalOffset,
                GameTreeVerticalOffset);
            
            BringNodeIntoView(ViewModel.GameTree.GameTreeRoot);
        }
                
        private void gameTreeRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGameTreeSize();
            verticalBar.ViewportSize = e.NewSize.Height;
            horizontalBar.ViewportSize = e.NewSize.Width;
        }

        private void HandleTouchPointerDown(Point pointerPosition)
        {
            _isPointerDown = true;
            _pointerMoveDifference = 0;
            _pointerCurrentPosition = pointerPosition;
        }

        private void HandleTouchPointerMove(Point pointerPosition)
        {
            if (!_isPointerDown)
                return;

            double horizontalDiff = pointerPosition.X - _pointerCurrentPosition.X;
            double verticalDiff = pointerPosition.Y - _pointerCurrentPosition.Y;

            // Calculate lenght the pointer gone since last sampling (by applying standard 2D Vector Length)
            _pointerMoveDifference += Math.Sqrt(horizontalDiff * horizontalDiff + verticalDiff * verticalDiff);

            // Make sure we are not behind bounds
            SetScrollOffset(
                GameTreeHorizontalOffset - horizontalDiff, 
                GameTreeVerticalOffset - verticalDiff);

            _pointerCurrentPosition = pointerPosition;
        }

        private void HandleTouchPointerUp(Point pointerPosition)
        {
            _isPointerDown = false;
            
            if (_pointerMoveDifference <= POINTERMOVETOLERANCE)
                HighlightNodeAtPointerPosition(pointerPosition);
        }

        private void HandleMouseUp(Point pointerPosition)
        {
            HighlightNodeAtPointerPosition(pointerPosition);
        }

        private void SetScrollOffset(double horizontalOffset, double verticalOffset)
        {
            // Make sure we are not behind bounds
            GameTreeHorizontalOffset = Math.Min(GameTreeWidth, horizontalOffset);
            GameTreeVerticalOffset = Math.Min(GameTreeHeight, verticalOffset);
        }
        
        private void BringNodeIntoView(GameTreeNode node)
        {
            int tmpVerticalOffset = 0;

            WalkGameTree(
                node, 0, ref tmpVerticalOffset, 0,
                (currentNode, horizontalOffset, verticalOffset, parentVerticalOffset) =>
                {
                    if (currentNode == ViewModel.SelectedGameTreeNode)
                    {
                        double nodeVerticalOffset =
                            verticalOffset * NODESIZE +
                            (verticalOffset - 1) * NODESPACING +
                            2 * NODEHIGHLIGHTSTROKE;    // Add node elliptical stroke for top and bottom

                        double nodeHorizontalOffset =
                            horizontalOffset * NODESIZE +
                            horizontalOffset * NODESPACING;    // Add node elliptical stroke for left and right

                        double horizontalDiff = nodeHorizontalOffset;
                        double verticalDiff = nodeVerticalOffset;

                        if (horizontalDiff >= GameTreeHorizontalOffset && horizontalDiff < (GameTreeHorizontalOffset + canvas.ActualWidth - NODECOMBINEDSIZE))
                            // We are located inside current Viewport
                            horizontalDiff = GameTreeHorizontalOffset;
                        else if (horizontalDiff > GameTreeHorizontalOffset)
                            // We are located on the right side of Viewport
                            horizontalDiff = GameTreeHorizontalOffset + (horizontalDiff - GameTreeHorizontalOffset - canvas.ActualWidth + NODECOMBINEDSIZE);

                        if (verticalDiff >= GameTreeVerticalOffset && verticalDiff < (GameTreeVerticalOffset + canvas.ActualHeight - NODECOMBINEDSIZE))
                            // We are located inside current Viewport
                            verticalDiff = GameTreeVerticalOffset;
                        else if (verticalDiff > GameTreeVerticalOffset)
                            // We are located on the right side of Viewport
                            verticalDiff = GameTreeVerticalOffset + (verticalDiff - GameTreeVerticalOffset - canvas.ActualHeight + NODECOMBINEDSIZE);

                        // Make sure we are not behing bounds (could happen when branch get deleted)
                        SetScrollOffset(
                            horizontalDiff,
                            verticalDiff);

                        return GameTreeNodeCallbackResultBehavior.Stop;
                    }

                    return GameTreeNodeCallbackResultBehavior.Continue;
                });
        }

        //////
        // Game Tree Walker Infrastructure
        //////

        private delegate GameTreeNodeCallbackResultBehavior GameTreeNodeCallback(GameTreeNode node, int horizontalOffset, int verticalOffset, int parentVerticalOffset);

        private enum GameTreeNodeCallbackResultBehavior
        {
            Continue,
            Stop
        }

        private GameTreeNodeCallbackResultBehavior WalkGameTree(GameTreeNode node, int horizontalOffset, ref int verticalOffset, int parentVerticalOffset, GameTreeNodeCallback nodeResultCallback)
        {
            var resultBehavior = nodeResultCallback(node, horizontalOffset, verticalOffset, parentVerticalOffset);

            if (resultBehavior == GameTreeNodeCallbackResultBehavior.Stop)
                return resultBehavior;

            int nodeOffset = verticalOffset;

            foreach (GameTreeNode childNode in node.Branches)
            {
                resultBehavior = WalkGameTree(childNode, horizontalOffset + 1, ref verticalOffset, nodeOffset, nodeResultCallback);

                if (resultBehavior == GameTreeNodeCallbackResultBehavior.Stop)
                {
                    if (node.Branches.Count > 0)
                        verticalOffset--;

                    return resultBehavior;
                }

                verticalOffset++;
            }

            if (node.Branches.Count > 0)
                verticalOffset--;

            return GameTreeNodeCallbackResultBehavior.Continue;
        }
    }
}
