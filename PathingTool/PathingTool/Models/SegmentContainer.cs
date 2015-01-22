using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PathingTool.Models
{
    /// <summary>
    /// Manages the data of the PathContainer class (to keep it clean).
    /// </summary>
    public class SegmentContainer
    {
        private PathFigure _oldCurrentFigure;
        public PathFigure CurrentFigure { get; set; }

        private List<EllipseGeometry> _oldEllipses;
        public List<EllipseGeometry> Ellipses { get; set; }

        // Hold segments and corresponding circles
        private Dictionary<PathSegment, EllipseGeometry> _oldPairs;
        public Dictionary<PathSegment, EllipseGeometry> Pairs { get; private set; }

        public GeometryGroup GeomGroup { get; set; }

        private BezierSegment _currentBezier;
        public Point CurrentBezierPoint2
        {
            get { return _currentBezier.Point2; }
        }

        private PathFigureCollection _oldFigures;
        public PathFigureCollection Figures { get; set; }

        // Currently selected segment for moving points
        public PathSegment CurrentSegment { get; private set; }
        public Point CurrentSelectedPoint { get; private set; }
        public int CurrentPointIndex;

        public SegmentContainer(GeometryGroup group)
        {
            _oldFigures = new PathFigureCollection();
            Figures = new PathFigureCollection();
            CurrentFigure = Figures.FirstOrDefault();

            _oldEllipses = new List<EllipseGeometry>();
            Ellipses = new List<EllipseGeometry>();
            GeomGroup = group;

            Pairs = new Dictionary<PathSegment, EllipseGeometry>();
            _oldPairs = new Dictionary<PathSegment, EllipseGeometry>();
        }

        // Regular line segments
        public bool AddLineSegment(Point pt)
        {
            if (Figures.Count == 0)
            {
                AddPathFigure(pt);
                return false;
            }

            var segment = new LineSegment(pt, true);
            AddPair(segment, pt);
            return true;
        }

        // Bezier segment: define endpoint first, then add bezier curve
        public bool AddBezierSegment(Point pt)
        {
            if (Figures.Count == 0)
            {
                AddPathFigure(pt);
                return false;
            }         

            // Retrieve the last point of the figure
            // This is the startpoint of the bezier segment
            var lastPt = new Point();
            if (CurrentFigure.Segments.Count > 0)
            {
                var lastSegment = CurrentFigure.Segments[CurrentFigure.Segments.Count - 1];

                if (lastSegment is LineSegment) lastPt = (lastSegment as LineSegment).Point;
                else if (lastSegment is BezierSegment) lastPt = (lastSegment as BezierSegment).Point3;
            }
            else lastPt = CurrentFigure.StartPoint;

            // Temporarily set the curve point
            var midX = (pt.X + lastPt.X)/2;
            var midY = (pt.Y + lastPt.Y)/2;

            var segment = new BezierSegment(lastPt, new Point(midX, midY), pt, true);
            _currentBezier = segment;
            AddPair(segment, pt);
            return true;
        }

        public void ChangeBezierSegment(Point pt)
        {
            if (_currentBezier == null) return;

            _currentBezier.Point2 = pt;
        }

        // Add segment to the figure
        // Adds segment and ellipse to the dictionary
        private void AddPair(PathSegment segment, Point pt)
        {
            CurrentFigure.Segments.Add(segment);
            var el = AddEllipse(pt);

            Pairs.Add(segment, el);
        }

        // Create a new path figure
        public void AddPathFigure(Point pt)
        {
            var figure = new PathFigure {StartPoint = pt, IsClosed = false};
            Figures.Add(figure);
            CurrentFigure = figure;
            AddEllipse(pt);
        }

        private EllipseGeometry AddEllipse(Point pt)
        {
            var ellipse = new EllipseGeometry(pt, 4, 4);
            Ellipses.Add(ellipse);
            GeomGroup.Children.Add(ellipse);
            return ellipse;
        }

        public void RemoveLastSegment()
        {
            var segments = CurrentFigure.Segments;
            var count = segments.Count;
            if (count > 0)
            {
                CurrentFigure.Segments.RemoveAt(count - 1);
            }
            else if (CurrentFigure != null) Figures.Remove(CurrentFigure);

            var ellipse = Ellipses[Ellipses.Count - 1];
            GeomGroup.Children.Remove(ellipse);
            Ellipses.Remove(ellipse);
        }

        public void ClearAll()
        {
            _oldFigures = Figures.CloneCurrentValue();
            _oldCurrentFigure = CurrentFigure.CloneCurrentValue();
            foreach (var pair in Pairs)
            {
                _oldPairs.Add(pair.Key.CloneCurrentValue(), pair.Value.CloneCurrentValue());
            }
            Figures.Clear();
            ClearEllipses();
            Pairs.Clear();
        }

        public void Reset()
        {
            foreach (var pair in _oldPairs)
            {
                Pairs.Add(pair.Key, pair.Value);
            }
            _oldPairs.Clear();

            foreach (var el in _oldEllipses)
            {
                Ellipses.Add(el);
                GeomGroup.Children.Add(el);
            }
            _oldEllipses.Clear();

            foreach (var fig in _oldFigures)
            {
                Figures.Add(fig);
            }
            _oldFigures.Clear();
            CurrentFigure = Figures.FirstOrDefault();
        }

        private void ClearEllipses()
        {
            foreach (var el in Ellipses.Where(el => GeomGroup.Children.Contains(el)))
            {
                _oldEllipses.Add(el.CloneCurrentValue());
                GeomGroup.Children.Remove(el);              
            }
            Ellipses.Clear();
        }

        /// <summary>
        /// Loop over all (end) points used by the segments, and start points of all figures.
        /// </summary>
        /// <param name="pt">Point to check against.</param>
        /// <returns>True if the point is found.</returns>
        public bool IsPointUsed(Point pt)
        {
            foreach (var fig in Figures)
            {
                if (fig.StartPoint.Equals(pt))
                {
                    CurrentSelectedPoint = pt;
                    return true;
                }
                foreach (var seg in fig.Segments)
                {
                    if (seg is LineSegment)
                    {
                        if ((seg as LineSegment).Point.Equals(pt))
                        {
                            CurrentSegment = seg;
                            CurrentSelectedPoint = pt;
                            return true;
                        }
                    }
                    else if (seg is BezierSegment)
                    {
                        if ((seg as BezierSegment).Point3.Equals(pt))
                        {
                            CurrentSegment = seg;
                            CurrentSelectedPoint = pt;
                            return true;
                        }
                    }
                    else if (seg is PolyLineSegment)
                    {
                        var poly = seg as PolyLineSegment;
                        var len = poly.Points.Count;
                        for (var i = 0; i < len; ++i)
                        {
                            if (!poly.Points[i].Equals(pt)) continue;
                            CurrentSegment = seg;
                            CurrentPointIndex = i;
                            CurrentSelectedPoint = pt;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void ChangeSelectedPoint(Point pt)
        {
            // This means the start point of a figure was selected
            if (CurrentSegment == null)
            {
                CurrentFigure.StartPoint = pt;
                return;
            }

            // Set the segment point
            if (CurrentSegment is LineSegment)
            {
                (CurrentSegment as LineSegment).Point = pt;
                
            }
            else if (CurrentSegment is BezierSegment)
            {
                (CurrentSegment as BezierSegment).Point3 = pt;
            }
            else if (CurrentSegment is PolyLineSegment)
            {
                (CurrentSegment as PolyLineSegment).Points[CurrentPointIndex] = pt;
            }

            // Set the ellipse point
            if (Pairs.ContainsKey(CurrentSegment))
                Pairs[CurrentSegment].Center = pt;
            CurrentSegment = null;
        }
    }
}