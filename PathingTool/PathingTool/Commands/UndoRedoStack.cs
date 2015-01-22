using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PathingTool.Annotations;
using PathingTool.ViewModels;

namespace PathingTool.Commands
{
    /// <summary>
    /// Singleton class: static properties and WPF binding are not good friends.
    /// </summary>
    public class UndoRedoStack : INotifyPropertyChanged
    {
        private static readonly UndoRedoStack _instance = new UndoRedoStack();

        private bool _canUndo;     
        public bool CanUndo
        {
            get { return _canUndo; }
            set
            {
                _canUndo = value;
                OnPropertyChanged();
            }
        }

        private bool _canRedo;
        public bool CanRedo
        {
            get { return _canRedo; }
            set
            {
                _canRedo = value;
                OnPropertyChanged();
            }
        }

        private Stack<IUndoableCommand> _undoStack = new Stack<IUndoableCommand>();
        private Stack<IUndoableCommand> _redoStack = new Stack<IUndoableCommand>();

        // Private constructor: only use singleton
        private UndoRedoStack() { }

        public static UndoRedoStack GetInstance()
        {
            return _instance;
        }

        public string UndoText
        {
            get { return _undoStack.Count > 0 ? "Undo " + _undoStack.Peek().ToString() : string.Empty; }
        }
        public string RedoText
        {
            get { return _redoStack.Count > 0 ? "Redo " + _redoStack.Peek().ToString() : string.Empty; }
        }

        // Add a command to the undo-stack
        public void AddToStack(IUndoableCommand command)
        {
            _undoStack.Push(command);
            _redoStack.Clear();

            CanUndo = true;
            CanRedo = false;
        }

        public void Undo()
        {
            CommandManager.InvalidateRequerySuggested();
            if (_undoStack.Count <= 0) return;
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);

            if (_undoStack.Count == 0) CanUndo = false;
            CanRedo = true;
        }

        public void Redo()
        {
            if (_redoStack.Count <= 0) return;
            var command = _redoStack.Pop();
            command.Redo();
            _undoStack.Push(command);

            if (_redoStack.Count == 0) CanRedo = false;
            CanUndo = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
