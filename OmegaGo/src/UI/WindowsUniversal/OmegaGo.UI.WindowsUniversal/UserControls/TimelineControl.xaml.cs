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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class TimelineControl : UserControlBase
    {
        private const int NODESIZE = 24;
        private const int NODESPACING = 4;
        private const int NODEHIGHLIGHTSTROKE = 2;
        private const int NODEFONTSIZE = 10;

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
            if (ViewModel?.GameTree == null || ViewModel?.GameTree?.GameTreeRoot.Branches.Count == 0)
                return base.MeasureOverride(availableSize);

            int requiredHeight = CalculateDesiredSize(ViewModel.GameTree.GameTreeRoot.Branches[0], 0, 0) + 1;
            
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
            if (ViewModel.GameTree != null && ViewModel.GameTree.GameTreeRoot.Branches.Count != 0)
            {
                args.DrawingSession.Transform = Matrix3x2.CreateTranslation(NODEHIGHLIGHTSTROKE, NODEHIGHLIGHTSTROKE);
                int requiredHeight = DrawNode(args.DrawingSession, ViewModel.GameTree.GameTreeRoot.Branches[0], 0, 0, 0) + 1;
            }
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            GameTreeNode pressedNode;
            Point pointerPosition = e.GetCurrentPoint(canvas).Position;

            // Compensate position for transformation
            pointerPosition.X -= NODEHIGHLIGHTSTROKE;
            pointerPosition.Y -= NODEHIGHLIGHTSTROKE;

            GetNodeAtPoint(pointerPosition, ViewModel.GameTree.GameTreeRoot.Branches[0], 0, 0, out pressedNode);

            if (pressedNode != null && pressedNode != ViewModel.SelectedTimelineNode)
            {
                ViewModel.SetSelectedNode(pressedNode);
            }
        }

        private int DrawNode(CanvasDrawingSession drawingSession, GameTreeNode node, int depth, int offset, int parentOffset)
        {
            int nodeOffset = offset;

            Vector2 stoneTopLeft = new Vector2(
                depth * NODESIZE + depth * NODESPACING,
                offset * NODESIZE + offset * NODESPACING);

            Vector2 stoneCenter = stoneTopLeft;
            stoneCenter.X += NODESIZE * 0.5f;
            stoneCenter.Y += NODESIZE * 0.5f;

            string nodeText="";
            if (node.Move.Kind == MoveKind.PlaceStone)
                nodeText = node.Move.Coordinates.ToIgsCoordinates();
            else if (node.Move.Kind == MoveKind.Pass)
                nodeText = "P";

            CanvasTextLayout textLayout = GetTextLayoutForString(drawingSession.Device, nodeText);
            if (node.Move.WhoMoves == StoneColor.Black)
            {
                drawingSession.FillEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, Colors.Black);
                drawingSession.DrawTextLayout(textLayout, stoneTopLeft, Colors.White);
            }
            else if (node.Move.WhoMoves == StoneColor.White)
            {
                drawingSession.FillEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, Colors.White);
                drawingSession.DrawTextLayout(textLayout, stoneTopLeft, Colors.Black);
            }
            else
            {
                drawingSession.FillEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, Colors.MediumPurple);
            }

            if (node == ViewModel.SelectedTimelineNode)
            {
                drawingSession.DrawEllipse(stoneCenter, NODESIZE * 0.5f, NODESIZE * 0.5f, Colors.Tomato, NODEHIGHLIGHTSTROKE);
            }

            Vector2 lineStart = new Vector2(
                (depth) * NODESIZE + (depth - 1) * NODESPACING, 
                (parentOffset) * NODESIZE + parentOffset * NODESPACING + NODESIZE * 0.5f);
            Vector2 lineEnd = new Vector2(
                depth * NODESIZE + depth * NODESPACING,
                offset * NODESIZE + offset * NODESPACING + NODESIZE * 0.5f);

            drawingSession.DrawLine(lineStart, lineEnd, Colors.Gray);
            
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
    }
}
