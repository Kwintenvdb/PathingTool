using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PathingTool.Models;

namespace PathingTool.Commands
{
    class AddBezierCommand : IUndoableCommand
    {
        private PathContainer _container;
        private PointGrid _grid;
        private Point _snappedPt;

        public AddBezierCommand(PathContainer container, PointGrid grid)
        {
            _container = container;
            _grid = grid;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _snappedPt = _grid.FindClosestGridPoint(Mouse.GetPosition(parameter as IInputElement));
            _container.AddBezierSegment(_snappedPt);

            UndoRedoStack.GetInstance().AddToStack(Copy());
        }

        public event EventHandler CanExecuteChanged;

        public void Undo()
        {
            _container.RemoveLastSegment();
        }

        public void Redo()
        {
            _container.AddLineSegment(_snappedPt);
        }

        public IUndoableCommand Copy()
        {
            var cmd = new AddBezierCommand(_container, _grid);
            cmd._snappedPt = _snappedPt;
            return cmd;
        }

        public override string ToString()
        {
            return "add bezier point";
        }
    }
}
