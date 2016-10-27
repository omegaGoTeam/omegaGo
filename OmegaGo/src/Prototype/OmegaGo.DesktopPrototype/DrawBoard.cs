using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OmegaGo.DesktopPrototype
{
    public class DrawBoard : FrameworkElement
    {
        public static DependencyProperty BoardSizeXProperty =
                DependencyProperty.Register(
                        "BoardSizeX",
                        typeof(int),
                        typeof(DrawBoard),
                        new FrameworkPropertyMetadata(
                                19,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        public static DependencyProperty BoardSizeYProperty =
                DependencyProperty.Register(
                        "BoardSizeY",
                        typeof(int),
                        typeof(DrawBoard),
                        new FrameworkPropertyMetadata(
                                19,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        public static DependencyProperty CellSizeProperty =
                DependencyProperty.Register(
                        "CellSize",
                        typeof(int),
                        typeof(DrawBoard),
                        new FrameworkPropertyMetadata(
                                16,
                                FrameworkPropertyMetadataOptions.AffectsRender));


        public static DependencyProperty StonesPositionsProperty = 
                DependencyProperty.Register(
                        "StonesPositions", 
                        typeof(char[,]), 
                        typeof(DrawBoard), 
                        new FrameworkPropertyMetadata(
                                null, 
                                FrameworkPropertyMetadataOptions.AffectsRender));

        public int BoardSizeX
        {
            get { return (int)GetValue(BoardSizeXProperty); }
            set { SetValue(BoardSizeXProperty, value); }
        }

        public int BoardSizeY
        {
            get { return (int)GetValue(BoardSizeYProperty); }
            set { SetValue(BoardSizeYProperty, value); }
        }

        public int CellSize
        {
            get { return (int)GetValue(CellSizeProperty); }
            set { SetValue(CellSizeProperty, value); }
        }

        public char[,] StonesPositions
        {
            get { return (char[,])GetValue(StonesPositionsProperty); }
            set { SetValue(StonesPositionsProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double gridHeight = CellSize * BoardSizeY;
            double gridWidth = CellSize * BoardSizeX;

            return new Size(gridWidth, gridWidth);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double stoneRadius = (CellSize * 0.5f) - 2;

            Pen pen = new Pen(Brushes.Black, 1);
            double cellSizeHalf = CellSize * 0.5;

            double gridHeight = CellSize * BoardSizeY;
            double gridWidth = CellSize * BoardSizeX;
            
            for (int x = 0; x < BoardSizeX; x++)
            {
                // Draw row line
                double rowOffset = x * CellSize + cellSizeHalf;
                drawingContext.DrawLine(pen, new Point(0, rowOffset), new Point(gridWidth, rowOffset));
            }

            for (int y = 0; y < BoardSizeY; y++)
            {
                // Draw column line
                double columnOffset = y * CellSize + cellSizeHalf;
                drawingContext.DrawLine(pen, new Point(columnOffset, 0), new Point(columnOffset, gridHeight));
            }

            if (StonesPositions != null)
            {
                for (int x = 0; x < BoardSizeX; x++)
                {
                    double rowOffset = x * CellSize + cellSizeHalf;
                    
                    for (int y = 0; y < BoardSizeY; y++)
                    {
                        // Draw stone if exists
                        double columnOffset = y * CellSize + cellSizeHalf;

                        Brush brush = null;
                        if (StonesPositions[x, y] == 'x')
                        {
                            brush = Brushes.Black;
                        }
                        else if (StonesPositions[x, y] == 'o')
                        {
                            brush = Brushes.White;
                        }
                        if (brush != null)
                        {
                            var r = new Rect(x * CellSize + 2, y * CellSize + 2, CellSize, CellSize);
                            drawingContext.DrawEllipse(brush, null, new Point(rowOffset, columnOffset), stoneRadius, stoneRadius);
                            //drawingContext.DrawRectangle(brush, null, r);
                        }
                    }
                }
            }
        }
    }
}
