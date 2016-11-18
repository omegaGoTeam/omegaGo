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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OmegaGo.UI.WindowsUniversal.UserControls
{
    public sealed partial class TimelineControl : UserControl
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
                        new PropertyMetadata(32, TimelineChanged));
        
        public static readonly DependencyProperty TimelineNodeSpacingProperty =
                DependencyProperty.Register(
                        "TimelineNodeSpacing",
                        typeof(int),
                        typeof(TimelineControl),
                        new PropertyMetadata(8, TimelineChanged));

        private static readonly DependencyProperty TimelineDepthProperty =
                DependencyProperty.Register(
                        "TimelineDepth",
                        typeof(int),
                        typeof(TimelineControl),
                        new PropertyMetadata(0));

        private static void TimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineControl timeline = d as TimelineControl;

            if (timeline != null)
            {
                if (e.Property == ViewModelProperty)
                {
                    TimelineViewModel oldViewModel;
                    TimelineViewModel newViewModel;

                    if (e.OldValue != null)
                    {
                        oldViewModel = (TimelineViewModel)e.OldValue;
                        oldViewModel.TimelineRedrawRequsted -= timeline.TimelineRedrawRequsted;
                    }

                    newViewModel = (TimelineViewModel)e.NewValue;
                    newViewModel.TimelineRedrawRequsted += timeline.TimelineRedrawRequsted;
                }

                timeline.Draw();
            }
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

        public TimelineControl()
        {
            this.InitializeComponent();
        }
        
        private void Draw()
        {
            TimelineDepth = 0;
            canvas.Children.Clear();
            canvas.Width = 0;
            canvas.Height = 0;

            if (ViewModel?.GameTree != null && ViewModel?.GameTree?.GameTreeRoot != null)
            {
                int requiredHeight = DrawNode(ViewModel.GameTree.GameTreeRoot, 0, 0, 0) + 1;

                canvas.Height =
                    requiredHeight * TimelineNodeSize +
                    requiredHeight * TimelineNodeSpacing;

                canvas.Width =
                    (TimelineDepth + 1) * TimelineNodeSize +
                    (TimelineDepth + 1) * TimelineNodeSpacing;
            }
        }

        private int DrawNode(GameTreeNode node, int depth, int offset, int parentOffset)
        {
            int nodeOffset = offset;
            TimelineDepth = Math.Max(TimelineDepth, depth);

            Ellipse nodeVisual = new Ellipse();
            nodeVisual.Width = TimelineNodeSize;
            nodeVisual.Height = TimelineNodeSize;
            
            nodeVisual.StrokeThickness = 2;
            nodeVisual.Tag = node;

            if (node.Move.WhoMoves == StoneColor.Black)
                nodeVisual.Fill = new SolidColorBrush(Colors.Black);
            else if (node.Move.WhoMoves == StoneColor.White)
                nodeVisual.Fill = new SolidColorBrush(Colors.White);

            nodeVisual.PointerEntered += (s, e) => nodeVisual.Stroke = new SolidColorBrush(Colors.Yellow);
            nodeVisual.PointerExited += (s, e) => nodeVisual.Stroke = null;

            Canvas.SetTop(nodeVisual, offset * TimelineNodeSize + offset * TimelineNodeSpacing);
            Canvas.SetLeft(nodeVisual, depth * TimelineNodeSize + depth * TimelineNodeSpacing);

            Line nodeParentLine = new Line();
            nodeParentLine.Stroke = new SolidColorBrush(Colors.Gray);
            nodeParentLine.StrokeThickness = 1;
            nodeParentLine.X1 = (depth) * TimelineNodeSize + (depth - 1) * TimelineNodeSpacing;
            nodeParentLine.Y1 = (parentOffset) * TimelineNodeSize + parentOffset * TimelineNodeSpacing + TimelineNodeSize * 0.5;
            nodeParentLine.X2 = depth * TimelineNodeSize + depth * TimelineNodeSpacing;
            nodeParentLine.Y2 = offset * TimelineNodeSize + offset * TimelineNodeSpacing + TimelineNodeSize * 0.5;

            canvas.Children.Add(nodeParentLine);
            canvas.Children.Add(nodeVisual);
            
            foreach(GameTreeNode childNode in node.Branches)
            {
                offset = DrawNode(childNode, depth + 1, offset, nodeOffset);
                offset++;
            }

            if (node.Branches.Count > 0)
                offset--;

            return offset;
        }
        
        private void TimelineRedrawRequsted(object sender, EventArgs e)
        {
            Draw();
        }
    }
}
