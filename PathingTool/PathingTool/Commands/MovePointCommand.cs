using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PathingTool.Models;

namespace PathingTool.Commands
{
    public class MovePointCommand : IUndoableCommand
    {
        private PathContainer _container;
        private Point _lastPt;
        private Point _newPt;

        public MovePointCommand(PathContainer container)
        {
            _container = container;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _newPt = (Point) parameter;
            _lastPt = _container.SegContainer.CurrentSelectedPoint;
            _container.ChangeSelectedPoint(_newPt);
            UndoRedoStack.GetInstance().AddToStack(Copy());
        }

        public event EventHandler CanExecuteChanged;

        public void Undo()
        {
            _container.IsPointUsed(_newPt);
            _container.ChangeSelectedPoint(_lastPt);
        }

        public void Redo()
        {
            _container.IsPointUsed(_lastPt);
            _container.ChangeSelectedPoint(_newPt);
        }

        public IUndoableCommand Copy()
        {
            var cmd = new MovePointCommand(_container);
            cmd._lastPt = _lastPt;
            cmd._newPt = _newPt;
            return cmd;
        }

        public override string ToString()
        {
            return "move point " + _newPt;
        }
    }
}