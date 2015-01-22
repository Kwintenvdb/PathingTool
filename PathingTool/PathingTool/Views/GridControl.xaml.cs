using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using PathingTool.Helpers;
using PathingTool.Models;
using PathingTool.ViewModels;

namespace PathingTool.Views
{
    public partial class GridControl : UserControl
    {
        private const int FRAMERATE = 60;
        private DateTime _prevDrawTime;

        private PointGrid _grid;

        private GridViewModel _gvm;

        public GridControl()
        {
            InitializeComponent();

            _grid = new PointGrid(20, 20);
            _gvm = DataContext as GridViewModel;

            // Render loop
            _prevDrawTime = DateTime.Now;

            var paintTimer = new Timer()
            {
                AutoReset = true,
                Interval = 1000.0 / FRAMERATE
            };

            paintTimer.Elapsed +=
                (o, e) => Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)InvalidateVisual);
            paintTimer.Start();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Update stuff
            var now = DateTime.Now;
            var diff = now - _prevDrawTime;
            _prevDrawTime = now;
            var dT = diff.TotalSeconds;

            //_mousePos = Mouse.GetPosition(this);

            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(215, 215, 215)), null,
                new Rect(0, 0, ActualWidth, ActualHeight));

            _grid.DrawGrid(drawingContext, this);

            switch (_gvm.EdType)
            {
                case EditType.ChangeBezier:
                    Circle.Fill = Brushes.DeepPink;
                    break;

                default:
                    Circle.Fill = Brushes.Black;
                    break;
            }

            var pos = _grid.FindClosestGridPoint(Mouse.GetPosition(this));
            Canvas.SetTop(Circle, pos.Y - Circle.Height / 2);
            Canvas.SetLeft(Circle, pos.X - Circle.Width / 2);

            //if (_gvm.Container.Figures.FirstOrDefault() != null)
            //{
            //    foreach (var seg in _gvm.Container.Segments)
            //    {
            //        drawingContext.DrawEllipse(
            //            Brushes.Black,
            //            null,
            //            (seg as LineSegment).Point,
            //            3, 3);
            //    }
            //}

            //_path.Update(drawingContext, _mousePos);
        }
    }
}
