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
        private const int NODEFONTSIZE = 10;

        private const double POINTERMOVETOLERANCE = 2.5d;

        private static readonly Color WhiteNodeColor = Colors.White;
        private static readonly Color BlackNodeColor = Colors.Black;
        private static readonly Color EmptyNodeColor = Colors.Maroon;
        private static readonly Color HighlightNodeColor = Colors.Tomato;
        private static readonly Color LineColor = Colors.Gray;

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
        
        private static void GameTreeVMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GameTreeControl GameTreeControl = d as GameTreeControl;

            if (GameTreeControl != null)
            {
                GameTreeViewModel viewModel = (GameTreeViewModel)e.NewValue;
                viewModel.GameTreeRedrawRequested += GameTreeControl.GameTreeRedrawRequsted;

                // GameTree scrolling
                GameTreeControl.PointerEntered += GameTreeControl.GameTreeControl_PointerEntered;
                GameTreeControl.PointerExited += GameTreeControl.GameTreeControl_PointerExited;
                GameTreeControl.PointerWheelChanged += GameTreeControl.GameTreeControl_PointerWheelChanged;
                // Arrows
                GameTreeControl.PointerReleased += GameTreeControl.GameTreeControl_FocusHack;
                GameTreeControl.KeyUp += GameTreeControl.GameTreeControl_KeyUp;

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

        private int _GameTreeDepth;

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
                int requiredHeight = DrawNode(args.DrawingSession, ViewModel.GameTree.GameTreeRoot, 0, 0, 0) + 1;
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

            // Make sure we are not behind bounds, this method checks that
            if (isHorizontal)
                SetScrollOffset(GameTreeHorizontalOffset + wheelDelta, GameTreeVerticalOffset);
            else
                SetScrollOffset(GameTreeHorizontalOffset, GameTreeVerticalOffset - wheelDelta);
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
            GetNodeAtPoint(pointerPosition, gameTreeRootNode, 0, 0, out pressedNode);

            if (pressedNode != null && pressedNode != ViewModel.SelectedGameTreeNode)
            {
                ViewModel.SetSelectedNode(pressedNode);
            }
        }

        //////
        // GameTree drawing
        //////

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
            if (node == ViewModel.SelectedGameTreeNode)
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
            _GameTreeDepth = Math.Max(_GameTreeDepth, depth);

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

        private void GameTreeControl_FocusHack(object sender, PointerRoutedEventArgs e)
        {
            // Hack around UWP XAML Focus WTFiness
            Task.Run(
                async () =>
                {
                    await Task.Delay(100);
                    var task = this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => this.Focus(FocusState.Programmatic));
                });
        }

        private void GameTreeControl_KeyUp(object sender, KeyRoutedEventArgs e)
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
            GameTreeNode node = ViewModel.SelectedGameTreeNode;

            if (node.NextNode != null)
                ViewModel.SetSelectedNode(node.NextNode);
        }

        private void SwitchToParentNode()
        {
            GameTreeNode node = ViewModel.SelectedGameTreeNode;

            if (node.Parent != null)
                ViewModel.SetSelectedNode(node.Parent);
        }

        //////
        // Custom scrolling implementation
        //////

        private void UpdateGameTreeSize()
        {
            if (ViewModel?.GameTree == null || ViewModel?.GameTree?.GameTreeRoot == null)
                return;

            int requiredHeight = CalculateDesiredSize(ViewModel.GameTree.GameTreeRoot, 0, 0) + 1;

            double height = requiredHeight * NODESIZE +
                            (requiredHeight - 1) * NODESPACING +
                            2 * NODEHIGHLIGHTSTROKE;    // Add node elliptical stroke for top and bottom

            double width = (_GameTreeDepth + 1) * NODESIZE +
                            (_GameTreeDepth + 1) * NODESPACING +
                            2 * NODEHIGHLIGHTSTROKE;    // Add node elliptical stroke for left and right

            GameTreeHeight = height - canvas.ActualHeight;  // Subtract from the entire required height what we can display
            GameTreeWidth = width - canvas.ActualWidth;     // Subtract from the entire required width what we can display

            // Make sure we are not behing bounds (could happen when branch get deleted)
            SetScrollOffset(
                GameTreeHorizontalOffset,
                GameTreeVerticalOffset);
        }
                
        private void layoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
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
    }
}
