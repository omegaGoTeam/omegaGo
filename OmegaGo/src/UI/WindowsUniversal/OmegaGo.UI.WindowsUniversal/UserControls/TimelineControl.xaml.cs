using OmegaGo.Core;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using OmegaGo.Core.Game;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System.Numerics;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class TimelineControl : UserControlBase
    {
        public static readonly DependencyProperty ViewModelProperty =
                DependencyProperty.Register(
                        "ViewModel",
                        typeof(TimelineViewModel),
                        typeof(TimelineControl),
                        new PropertyMetadata(null, TimelineChanged));

        public static readonly DependencyProperty TimelineNodeSizeProperty = 
                DependencyProperty.Register(
                        "TimelineNodeSize", 
                        typeof(int), 
                        typeof(TimelineControl), 
                        new PropertyMetadata(32));
        
        public static readonly DependencyProperty TimelineNodeSpacingProperty =
                DependencyProperty.Register(
                        "TimelineNodeSpacing",
                        typeof(int),
                        typeof(TimelineControl),
                        new PropertyMetadata(8));

        private static readonly DependencyProperty TimelineDepthProperty =
                DependencyProperty.Register(
                        "TimelineDepth",
                        typeof(int),
                        typeof(TimelineControl),
                        new PropertyMetadata(0));

        private static void TimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineControl timelineControl= d as TimelineControl;

            if (timelineControl != null)
            {
                TimelineViewModel viewModel = (TimelineViewModel)e.NewValue;
                viewModel.TimelineRedrawRequsted += timelineControl.TimelineRedrawRequsted;

                timelineControl.canvas.Draw += timelineControl.Canvas_Draw;
                timelineControl.canvas.PointerReleased += timelineControl.Canvas_PointerReleased;
                timelineControl.canvas.Invalidate();
            }
        }
        
        public TimelineControl()
        {
            this.InitializeComponent();
        }

        public TimelineViewModel ViewModel
        {
            get { return (TimelineViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public int TimelineNodeSize
        {
            get { return (int)GetValue(TimelineNodeSizeProperty); }
            set { SetValue(TimelineNodeSizeProperty, value); }
        }

        public int TimelineNodeSpacing
        {
            get { return (int)GetValue(TimelineNodeSpacingProperty); }
            set { SetValue(TimelineNodeSpacingProperty, value); }
        }

        public int TimelineDepth
        {
            get { return (int)GetValue(TimelineDepthProperty); }
            private set { SetValue(TimelineDepthProperty, value); }
        }

        private void TimelineRedrawRequsted(object sender, EventArgs e)
        {
            canvas.Invalidate();
            // canvas.InvalidateMeasure();
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            TimelineDepth = 0;
            canvas.Width = 0;
            canvas.Height = 0;

            if (ViewModel?.GameTree != null && ViewModel?.GameTree?.GameTreeRoot != null)
            {
                int requiredHeight = DrawNode(args.DrawingSession, ViewModel.GameTree.GameTreeRoot, 0, 0, 0) + 1;

                canvas.Height =
                    requiredHeight * TimelineNodeSize +
                    requiredHeight * TimelineNodeSpacing;

                canvas.Width =
                    (TimelineDepth + 1) * TimelineNodeSize +
                    (TimelineDepth + 1) * TimelineNodeSpacing;
            }
        }

        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            GameTreeNode pressedNode;
            GetNodeAtPoint(e.GetCurrentPoint(canvas).Position, ViewModel.GameTree.GameTreeRoot, 0, 0, out pressedNode);

            if (pressedNode != null)
                ViewModel.SelectedTimelineNode = pressedNode;
        }

        private int DrawNode(CanvasDrawingSession drawingSession, GameTreeNode node, int depth, int offset, int parentOffset)
        {
            int nodeOffset = offset;
            TimelineDepth = Math.Max(TimelineDepth, depth);

            Vector2 stoneCenter = new Vector2(
                depth * TimelineNodeSize + depth * TimelineNodeSpacing,
                offset * TimelineNodeSize + offset * TimelineNodeSpacing);

            stoneCenter.X += TimelineNodeSize * 0.5f;
            stoneCenter.Y += TimelineNodeSize * 0.5f;



            //Ellipse nodeVisual = new Ellipse();
            //nodeVisual.Width = TimelineNodeSize;
            //nodeVisual.Height = TimelineNodeSize;

            //nodeVisual.StrokeThickness = 2;
            //nodeVisual.Tag = node;

            if (node.Move.WhoMoves == StoneColor.Black)
                drawingSession.FillEllipse(stoneCenter, TimelineNodeSize * 0.5f, TimelineNodeSize * 0.5f, Colors.Black);
            else if (node.Move.WhoMoves == StoneColor.White)
                drawingSession.FillEllipse(stoneCenter, TimelineNodeSize * 0.5f, TimelineNodeSize * 0.5f, Colors.White);
            else
                drawingSession.FillEllipse(stoneCenter, TimelineNodeSize * 0.5f, TimelineNodeSize * 0.5f, Colors.MediumPurple);

            //nodeVisual.PointerEntered += (s, e) => nodeVisual.Stroke = new SolidColorBrush(Colors.Yellow);
            //nodeVisual.PointerExited += (s, e) => nodeVisual.Stroke = null;
            //nodeVisual.PointerReleased += (s, e) => ViewModel.SelectedTimelineNode = (GameTreeNode)((Ellipse)s).Tag;

            //Canvas.SetTop(nodeVisual, offset * TimelineNodeSize + offset * TimelineNodeSpacing);
            //Canvas.SetLeft(nodeVisual, depth * TimelineNodeSize + depth * TimelineNodeSpacing);

            Vector2 lineStart = new Vector2(
                (depth) * TimelineNodeSize + (depth - 1) * TimelineNodeSpacing, 
                (parentOffset) * TimelineNodeSize + parentOffset * TimelineNodeSpacing + TimelineNodeSize * 0.5f);
            Vector2 lineEnd = new Vector2(
                depth * TimelineNodeSize + depth * TimelineNodeSpacing,
                offset * TimelineNodeSize + offset * TimelineNodeSpacing + TimelineNodeSize * 0.5f);

            drawingSession.DrawLine(lineStart, lineEnd, Colors.Gray);

            //Line nodeParentLine = new Line();
            //nodeParentLine.Stroke = new SolidColorBrush(Colors.Gray);
            //nodeParentLine.StrokeThickness = 1;
            //nodeParentLine.X1 = (depth) * TimelineNodeSize + (depth - 1) * TimelineNodeSpacing;
            //nodeParentLine.Y1 = (parentOffset) * TimelineNodeSize + parentOffset * TimelineNodeSpacing + TimelineNodeSize * 0.5;
            //nodeParentLine.X2 = depth * TimelineNodeSize + depth * TimelineNodeSpacing;
            //nodeParentLine.Y2 = offset * TimelineNodeSize + offset * TimelineNodeSpacing + TimelineNodeSize * 0.5;

            //canvas.Children.Add(nodeParentLine);
            //canvas.Children.Add(nodeVisual);
            
            foreach(GameTreeNode childNode in node.Branches)
            {
                offset = DrawNode(drawingSession, childNode, depth + 1, offset, nodeOffset);
                offset++;
            }

            if (node.Branches.Count > 0)
                offset--;

            return offset;
        }

        private int GetNodeAtPoint(Point point, GameTreeNode node, int depth, int offset, out GameTreeNode resultNode)
        {
            // We draw elipse but hit test rectangle as this is easier.
            Rect nodeRect = new Rect(
                depth * TimelineNodeSize + depth * TimelineNodeSpacing,
                offset * TimelineNodeSize + offset * TimelineNodeSpacing,
                TimelineNodeSize, TimelineNodeSize);

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
    }
}
