using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PathingTool.Annotations;
using PathingTool.Commands;
using PathingTool.Views;
using Timer = System.Timers.Timer;

namespace PathingTool
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private const int FRAMERATE = 10;
        private DateTime _prevDrawTime;

        private string _undoRedoText;
        public string UndoRedoText
        {
            get { return _undoRedoText; }
            set
            {
                _undoRedoText = value;
                OnPropertyChanged();
            }
        }

        // Used for binding the animation duration
        private Duration _animDuration;
        public Duration AnimDuration
        {
            get { return _animDuration; }
            set
            {
                _animDuration = value;
                OnPropertyChanged();
            }
        }

        // Static property accessed by ScriptExporter
        public static double Duration { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            AnimDuration = new Duration(TimeSpan.FromSeconds(2));
            Duration = 2;

            SetCircleEmpty();

            // Render loop
            _prevDrawTime = DateTime.Now;

            var paintTimer = new Timer()
            {
                AutoReset = true,
                Interval = 1000.0/FRAMERATE
            };
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // Update stuff
            var now = DateTime.Now;
            var diff = now - _prevDrawTime;
            _prevDrawTime = now;
            var dT = diff.TotalSeconds;

            drawingContext.DrawRectangle(
                new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                null,
                new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            base.OnRender(drawingContext);
        }

        private void SetUndoText()
        {
            UndoRedoText = UndoRedoStack.GetInstance().UndoText;
        }

        private void SetRedoText()
        {
            UndoRedoText = UndoRedoStack.GetInstance().RedoText;
        }

        private void BtnUndo_MouseEnter(object sender, MouseEventArgs e)
        {
            SetUndoText();
        }

        /// <summary>
        /// Command is not databound to button, otherwise it executes the click event before the command.
        /// The command is stored in the button's Tag property, and is executed first before the text is updated.
        /// </summary>
        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var cmd = btn.Tag as ICommand;
                if (cmd != null)
                    cmd.Execute(null);

                SetUndoText();
            }    
        }

        private void BtnRedo_MouseEnter(object sender, MouseEventArgs e)
        {
            SetRedoText();
        }

        private void BtnRedo_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                var cmd = btn.Tag as ICommand;
                if (cmd != null)
                    cmd.Execute(null);

                SetRedoText();
            }    
        }

        private void TxtAnimLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            var str = TxtAnimLength.Text;
            double number;
            var result = double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
            if (result)
            {
                AnimDuration = TimeSpan.FromSeconds(number);
                Duration = number;
                TxtInvalid.Text = string.Empty;
                return;
            }
            TxtInvalid.Text = "Invalid";
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            var window = new HelpWindow();
            window.Show();
        }

        private void BtnPlayAnim_Click(object sender, RoutedEventArgs e)
        {
            CirclePath.Fill = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        }

        private void SetCircleEmpty()
        {
            CirclePath.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void BtnClearLines_Click(object sender, RoutedEventArgs e)
        {
            SetCircleEmpty();
        }
    }
}