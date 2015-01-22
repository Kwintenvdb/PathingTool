using System;
using System.Windows;
using System.Windows.Input;
using PathingTool.Models;

namespace PathingTool.Commands
{
    public class AddLineCommand : IUndoableCommand
    {
        private PathContainer _container;
        private PointGrid _grid;
        private Point _snappedPt;

        public AddLineCommand(PathContainer container, PointGrid grid)
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
            _container.AddLineSegment(_snappedPt);

            // Add a copy of this command to the stack
            UndoRedoStack.GetInstance().AddToStack(Copy());
        }

        public event EventHandler CanExecuteChanged;

        public void Redo()
        {
            _container.AddLineSegment(_snappedPt);
        }

        public void Undo()
        {
            _container.RemoveLastSegment();
        }

        public IUndoableCommand Copy()
        {
            var cmd = new AddLineCommand(_container, _grid);
            // Point is a value type, gets copied
            cmd._snappedPt = _snappedPt;
            return cmd;
        }

        public override string ToString()
        {
            return "add point " + _snappedPt;
        }
    }
}