using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PathingTool.Helpers;
using PathingTool.Models;

namespace PathingTool.Commands
{
    public class ExportCommand : ICommand
    {
        private PathContainer _container;

        public ExportCommand(PathContainer container)
        {
            _container = container;
        }

        public bool CanExecute(object parameter)
        {
            return _container.IsNotEmpty;
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, null);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public void Execute(object parameter)
        {
            ScriptExporter.ClearExporter();
            var path = _container.PathGeom;

            // Loop over all figures and segments inside those figures.
            foreach (var fig in path.Figures)
            {
                var lastPt = fig.StartPoint;

                foreach (var seg in fig.Segments)
                {
                    if (seg is BezierSegment)
                    {
                        var bez = seg as BezierSegment;
                        ScriptExporter.AddSegment(new Bezier(bez.Point1, bez.Point2, bez.Point3));
                        lastPt = bez.Point3;
                    }
                    else if (seg is LineSegment)
                    {
                        var line = seg as LineSegment;
                        var midX = (line.Point.X + lastPt.X) / 2;
                        var midY = (line.Point.Y + lastPt.Y) / 2;

                        ScriptExporter.AddSegment(new Bezier(lastPt, new Point(midX, midY), line.Point));
                        lastPt = line.Point;
                    }
                }
            }

            ScriptExporter.ExportScript();
        }

        public event EventHandler CanExecuteChanged;
    }
}