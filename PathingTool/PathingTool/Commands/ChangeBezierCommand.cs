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
    public class ChangeBezierCommand : IUndoableCommand
    {
        private PathContainer _container;
        private Point _mousePos;
        private Point _lastPoint2;

        public ChangeBezierCommand(PathContainer container)
        {
            _container = container;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _mousePos = Mouse.GetPosition(parameter as IInputElement);
            _lastPoint2 = _container.SegContainer.CurrentBezierPoint2;
            _container.ChangeBezierSegment(_mousePos);
            UndoRedoStack.GetInstance().AddToStack(Copy());
        }

        public event EventHandler CanExecuteChanged;

        public void Undo()
        {
            _container.ChangeBezierSegment(_lastPoint2);
        }

        public void Redo()
        {
            _container.ChangeBezierSegment(_mousePos);
        }

        public IUndoableCommand Copy()
        {
            var cmd = new ChangeBezierCommand(_container);
            cmd._mousePos = _mousePos;
            cmd._lastPoint2 = _lastPoint2;
            return cmd;
        }

        public override string ToString()
        {
            return "change bezier curve";
        }
    }
}