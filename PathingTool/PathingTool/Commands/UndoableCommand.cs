using System;
using System.Windows.Input;

namespace PathingTool.Commands
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
        void Redo();
        IUndoableCommand Copy();
    }
}