using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using PathingTool.Commands;
using PathingTool.Helpers;
using PathingTool.Models;

namespace PathingTool.ViewModels
{
    class GridViewModel : INotifyPropertyChanged
    {
        private EditType _editType;
        private EditType _oldEditType;
        public EditType EdType
        {
            get { return _editType; }
            set
            {
                _editType = value; 
                OnPropertyChanged();
            }
        }

        private PathContainer _container;
        public PathContainer Container
        {
            get { return _container; }
            set
            {
                _container = value;
                OnPropertyChanged();
            }
        }

        private PointGrid _grid;
        public PointGrid Grid
        {
            get { return _grid; }
            set
            {
                _grid = value;
                OnPropertyChanged();
            }
        }


        ////////////////
        /// COMMANDS ///
        ////////////////

        private static RelayCommand _undoCommand;
        public static RelayCommand UndoCommand
        {
            get { return _undoCommand ?? (_undoCommand = new RelayCommand(UndoRedoStack.GetInstance().Undo, () => true)); }
        }

        private static RelayCommand _redoCommand;
        public static RelayCommand RedoCommand
        {
            get { return _redoCommand ?? (_redoCommand = new RelayCommand(UndoRedoStack.GetInstance().Redo)); }
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(_service.Save)); }
        }

        private RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get { return _loadCommand ?? (_loadCommand = new RelayCommand(_service.Load)); }
        }

        private AddLineCommand _addLineCmd;
        public AddLineCommand AddLineCmd
        {
            get { return _addLineCmd ?? (_addLineCmd = new AddLineCommand(Container, Grid)); }
        }

        private AddBezierCommand _addBezierCmd;
        public AddBezierCommand AddBezierCmd
        {
            get { return _addBezierCmd ?? (_addBezierCmd = new AddBezierCommand(_container, _grid)); }
        }

        private ChangeBezierCommand _changeBezierCmd;
        public ChangeBezierCommand ChangeBezierCmd
        {
            get { return _changeBezierCmd ?? (_changeBezierCmd = new ChangeBezierCommand(_container)); }
        }

        private AddFigureCommand _addFigCmd;
        public AddFigureCommand AddFigCmd
        {
            get { return _addFigCmd ?? (_addFigCmd = new AddFigureCommand(_container, _grid)); }
        }

        private SelectPointCommand _selectCmd;
        public SelectPointCommand SelectCmd
        {
            get { return _selectCmd ?? (_selectCmd = new SelectPointCommand(Container)); }
        }

        private MovePointCommand _moveCmd;
        public MovePointCommand MoveCmd
        {
            get { return _moveCmd ?? (_moveCmd = new MovePointCommand(Container)); }
        }

        private ClearPathsCommand _clearCmd;
        public ClearPathsCommand ClearCmd
        {
            get { return _clearCmd ?? (_clearCmd = new ClearPathsCommand(_container)); }
        }

        private RelayCommand<object> _multiCmd;
        public RelayCommand<object> MultiCmd
        {
            get { return _multiCmd ?? (_multiCmd = new RelayCommand<object>(MultiCommandExecute)); }
        }

        private ExportCommand _exportCmd;
        public ExportCommand ExportCmd
        {
            get { return _exportCmd ?? (_exportCmd = new ExportCommand(_container)); }
        }

        /// <summary>
        /// Highlights the hovered-over path.
        /// </summary>
        private RelayCommand _hightlightPathCmd;
        public RelayCommand HightlightPathCmd
        {
            get { return _hightlightPathCmd ?? (_hightlightPathCmd = new RelayCommand(Container.HighlightPath)); }
        }

        // Saving and loading service.
        private XamlService _service;

        public UndoRedoStack Stack { get { return UndoRedoStack.GetInstance(); } }

        public GridViewModel()
        {
            Container = new PathContainer();
            Grid = new PointGrid(20, 20);

            _service = new XamlService(Container);
        }

        /// <summary>
        /// Allows switching between multiple commands, by binding one to a MouseClick.
        /// </summary>
        /// <param name="param">Command parameter</param>
        public void MultiCommandExecute(object param)
        {
            var pt = _grid.FindClosestGridPoint(Mouse.GetPosition(param as IInputElement));
            if (Container.IsPointUsed(pt))
            {
                SelectCmd.Execute(pt);
                EdType = EditType.ChangePoint;
            }
            else
            {
                switch (EdType)
                {
                    case EditType.Line:
                        AddLineCmd.Execute(param);
                        _oldEditType = EditType.Line;
                        break;

                    case EditType.Bezier:
                        AddBezierCmd.Execute(param);                    
                        if (_container.SegCount > 0)
                        {
                            EdType = EditType.ChangeBezier;
                            _oldEditType = EditType.Bezier;
                        }
                        break;

                    case EditType.ChangeBezier:
                        ChangeBezierCmd.Execute(param);
                        EdType = _oldEditType;
                        break;

                    case EditType.AddFigure:
                        AddFigCmd.Execute(param);
                        EdType = _oldEditType;
                        break;

                    case EditType.ChangePoint:
                        MoveCmd.Execute(pt);
                        EdType = _oldEditType;
                        break;
                }          
            }

            ClearCmd.RaiseCanExecuteChanged();
            ExportCmd.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}