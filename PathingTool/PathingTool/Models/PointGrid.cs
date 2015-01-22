using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PathingTool.Models
{
    public class PointGrid
    {
        private readonly int _gridPointWidth;
        private readonly int _gridPointHeight;

        public PointGrid(int w, int h)
        {
            _gridPointWidth = w;
            _gridPointHeight = h;
        }

        public void DrawGrid(DrawingContext context, UserControl control)
        {
            var cWidth = control.ActualWidth;
            var cHeight = control.ActualHeight;

            // amount of iterations for x and y axis
            var itH = cHeight/_gridPointHeight;
            var itW = cWidth/_gridPointWidth;

            var brush = new SolidColorBrush(Color.FromRgb(125, 125, 125));

            for (var i = 0; i < itH; ++i)
            {
                var height = i*_gridPointHeight;
                if (i > 0)
                {
                    context.DrawLine(
                        new Pen(brush, 0.5),
                        new Point(0, height),
                        new Point(cWidth, height));
                }
            }

            for (var i = 0; i < itW; ++i)
            {
                var width = i*_gridPointWidth;
                if (i > 0)
                {
                    context.DrawLine(
                        new Pen(brush, 0.5),
                        new Point(width, 0),
                        new Point(width, cHeight));
                }
            }
        }

        /// <summary>
        /// Finds the point on the grid closest to the mouse position (snapping to gridpoints).
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns>The snapped position.</returns>
        public Point FindClosestGridPoint(Point mousePos)
        {
            var x = Math.Round(mousePos.X/_gridPointWidth)*_gridPointWidth;
            var y = Math.Round(mousePos.Y/_gridPointHeight)*_gridPointHeight;
            var roundedPoint = new Point(x, y);
            return roundedPoint;
        }
    }
}
