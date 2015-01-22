using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PathingTool.Models;

namespace PathingTool.Commands
{
    public class ClearPathsCommand : IUndoableCommand
    {
        public event EventHandler CanExecuteChanged;
        private PathContainer _container;

        public ClearPathsCommand(PathContainer container)
        {
            _container = container;
        }

        public bool CanExecute(object parameter)
        {
            return _container.IsNotEmpty;
        }

        public void Execute(object parameter)
        {
            _container.ClearPaths();
            UndoRedoStack.GetInstance().AddToStack(Copy());
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, null);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public IUndoableCommand Copy()
        {
            var cmd = new ClearPathsCommand(_container);
            return cmd;
        }

        public void Undo()
        {
            _container.ResetPaths();
        }

        public void Redo()
        {
            _container.ClearPaths();
        }

        public override string ToString()
        {
            return "clear paths";
        }
    }
}