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
    class AddFigureCommand : IUndoableCommand
    {
        private PathContainer _container;
        private PointGrid _grid;
        private Point _snappedPt;

        public AddFigureCommand(PathContainer container, PointGrid grid)
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
            _container.AddFigure(_snappedPt);

            UndoRedoStack.GetInstance().AddToStack(Copy());
        }

        public event EventHandler CanExecuteChanged;
        public void Undo()
        {
            throw new NotImplementedException();
        }

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public IUndoableCommand Copy()
        {
            var cmd = new AddFigureCommand(_container, _grid);
            cmd._snappedPt = _snappedPt;
            return cmd;
        }
    }
}
