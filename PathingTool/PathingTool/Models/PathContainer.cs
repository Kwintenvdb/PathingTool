using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Command;
using PathingTool.Annotations;

namespace PathingTool.Models
{
    public class PathContainer : INotifyPropertyChanged
    {
        private System.Windows.Shapes.Path _path;
        public System.Windows.Shapes.Path Path 
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public GeometryGroup GroupGeom { get; set; }
        public GeometryGroup OldGeometryGroup { get; set; }

        private PathGeometry _pathGeom;
        public PathGeometry PathGeom
        {
            get { return _pathGeom; }
            set
            {
                _pathGeom = value;
                OnPropertyChanged();
            }
        }

        private bool _isNotEmpty;
        public bool IsNotEmpty
        {
            get { return _isNotEmpty; }
            set
            {
                _isNotEmpty = value;
                OnPropertyChanged();
            }
        }

        private bool _hasSegments;
        public bool HasSegments
        {
            get { return _hasSegments; }
            set
            {
                _hasSegments = value;
                OnPropertyChanged();
            }
        }

        private PathSegment _selectedSegment;
        private SegmentContainer _container;
        public SegmentContainer SegContainer
        {
            get { return _container; }          
        }

        public int SegCount
        {
            get { return _container.CurrentFigure.Segments.Count; }
        }

        public PathContainer()
        {
            IsNotEmpty = false;
            HasSegments = false;

            GroupGeom = new GeometryGroup();
            _container = new SegmentContainer(GroupGeom);

            PathGeom = new PathGeometry {FillRule = FillRule.Nonzero, Figures = _container.Figures};         
            GroupGeom.Children.Add(PathGeom);

            Path = new System.Windows.Shapes.Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Data = GroupGeom,
            };
        }

        public PathContainer Copy()
        {
            var p = new PathContainer();
            p.PathGeom.Figures = _container.Figures.Clone();
            return p;
        }

        /// <summary>
        /// Checks if a PathFigure has already been made, then adds a LineSegment to the currently selected figure.
        /// </summary>
        /// <param name="pt"></param>
        public void AddLineSegment(Point pt)
        {
            IsNotEmpty = true;
            if (_container.AddLineSegment(pt)) HasSegments = true;
        }

        public void AddFigure(Point pt)
        {
            _container.AddPathFigure(pt);
        }

        public void AddBezierSegment(Point pt)
        {
            IsNotEmpty = true;
            if (_container.AddBezierSegment(pt)) HasSegments = true;
        }

        public void ChangeBezierSegment(Point pt)
        {
            _container.ChangeBezierSegment(pt);
        }

        public void RemoveLastSegment()
        {
            _container.RemoveLastSegment();
            if (_container.Figures.Count <= 0) IsNotEmpty = false;
            if (_container.CurrentFigure.Segments.Count <= 0) HasSegments = false;
        }

        public void ClearPaths()
        {
            _container.ClearAll();
            IsNotEmpty = false;
            HasSegments = false;
        }

        public void ResetPaths()
        {
            _container.Reset();
            IsNotEmpty = true;
            if (_container.CurrentFigure.Segments.Count > 0) HasSegments = true;
        }

        public void LoadData(Geometry group)
        {
            _container.ClearAll();
            GroupGeom = group as GeometryGroup;
            Path.Data = GroupGeom;
            foreach (var geom in GroupGeom.Children)
            {
                if (geom is PathGeometry) PathGeom = geom as PathGeometry;
                if (geom is EllipseGeometry) _container.Ellipses.Add(geom as EllipseGeometry);
            }
            _container.GeomGroup = GroupGeom;
            _container.Figures = PathGeom.Figures;
            _container.CurrentFigure = _container.Figures.FirstOrDefault();

            IsNotEmpty = true;
            if (_container.CurrentFigure.Segments.Count > 0) HasSegments = true;
        }

        public void HighlightPath()
        {
            Path.StrokeThickness = 5;
        }

        /// <summary>
        /// Check if the clicked point in the UserControl equals the endpoint of any segment.
        /// Used for changing the positions of previously placed points.
        /// </summary>
        /// <param name="pt">The point to check.</param>
        /// <returns>true if the point is used by the path.</returns>
        public bool IsPointUsed(Point pt)
        {
            return _container.IsPointUsed(pt);
        }

        public void ChangeSelectedPoint(Point pt)
        {
            _container.ChangeSelectedPoint(pt);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
