using System;
using System.Windows;
using System.Windows.Input;
using PathingTool.Models;

namespace PathingTool.Commands
{
    class SelectPointCommand : IUndoableCommand
    {
        private PathContainer _container;

        public SelectPointCommand(PathContainer container)
        {
            _container = container;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //var pt = (Point) parameter;
            //MessageBox.Show("point found: " + pt);
            //UndoRedoStack.GetInstance().AddToStack(Copy()); 
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
            var cmd = new SelectPointCommand(_container);
            return cmd;
        }
    }
}
