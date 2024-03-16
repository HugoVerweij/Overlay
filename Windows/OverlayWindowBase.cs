using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Windows.Point;

namespace Overlay.Windows
{
    public abstract class OverlayWindowBase : UserControl
    {
        #region Variables

        // Public.
        public MainWindow ParentWindow;
        public readonly DispatcherTimer Timer;
        public StyleTypes StyleType;
        public Point Position;
        public double ZIndex;
        public bool Locked;

        // Events.
        public delegate void OverlayWindowEventHandler(object sender, EventArgsBase e);
        public event PropertyChangedEventHandler PropertyChanged;
        public event OverlayWindowEventHandler OnMinimize;
        public event OverlayWindowEventHandler OnMaxamize;
        public event OverlayWindowEventHandler OnExit;
        public event OverlayWindowEventHandler OnClick;

        // Private.
        private Point mousePosition;

        // Binding
        #region Binding

        #region Public/Protected

        public string WindowTitle
        {
            get => Title();
            set => OnPropertyChanged();
        }

        public double? WindowOpacity
        {
            get => Opacity();
            set => OnPropertyChanged();
        }

        #endregion

        #region Abstract/Virtual

        protected virtual string Title()
        {
            return null;
        }

        protected virtual new double? Opacity()
        {
            return null;
        }

        #endregion

        #endregion

        #endregion

        #region OnLoad

        public OverlayWindowBase(OverlayWindowSettings settings)
        {
            // Set the variables.
            ParentWindow = settings.Parent;
            StyleType = settings.Style;
            Position = settings.StartPos != null ? settings.StartPos : new Point(0, 0);
            ZIndex = settings.ZIndex;
            MinWidth = 200;
            MinHeight = 100;
            Width = settings.Width;
            Height = settings.Height;
            Locked = settings.Locked;

            // Set the relevant binding context.
            DataContext = this;

            // Set the style.
            ChangeStyle(settings.Style);

            // Set the transform.
            RenderTransform = new TranslateTransform();
            (RenderTransform as TranslateTransform).X = settings.StartPos.X;
            (RenderTransform as TranslateTransform).Y = settings.StartPos.Y;

            // Set the timer.
            Timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            Timer.Tick += Timer_Tick;

            // Subscribe to parentwindow events.
            ParentWindow.Logger.OnKeyDown += Logger_OnKeyDown;
            ParentWindow.Closing += ParentWindow_Closing;
        }

        #endregion

        #region Methods

        public void ChangeStyle(StyleTypes style)
        {
            // Change the style.
            SetResourceReference(StyleProperty, Enum.GetName(typeof(StyleTypes), style));
        }

        #endregion

        #region Events

        // General
        public void OnControlClick(object sender, MouseButtonEventArgs e)
        {
            // Invoke if not null.
            OnClick?.Invoke(this, new EventArgsBase(null));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Buttons
        public virtual void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            // Invoke if not null.
            OnMinimize?.Invoke(this, new EventArgsBase(null));
        }

        public virtual void OnMaximizeClick(object sender, RoutedEventArgs e)
        {
            // Invoke if not null.
            OnMaxamize?.Invoke(this, new EventArgsBase(null));
        }

        public virtual void OnExitClick(object sender, RoutedEventArgs e)
        {
            // Invoke if not null.
            OnExit?.Invoke(this, new EventArgsBase(null));
        }

        // Dragging
        public void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Return if locked.
            if (Locked) return;

            // Set the titlebar.
            Grid title = sender as Grid;

            // Remember where the user grabbed the control from.
            mousePosition = e.GetPosition(this);

            // Capture the mouse.
            title.CaptureMouse();
        }

        public void OnTitleBarMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Return if locked.
            if (Locked) return;

            // Set the titlebar.
            Grid title = sender as Grid;

            // Check if the mouse is captured.
            if (title.IsMouseCaptured)
            {
                // Release the mouse.
                title.ReleaseMouseCapture();
            }
        }

        public void OnTitleBarDrag(object sender, MouseEventArgs e)
        {
            // Return if locked.
            if (Locked) return;

            // Set the titlebar.
            Grid grid = sender as Grid;

            // Calculate the difference.
            Vector diff = e.GetPosition(ParentWindow as Window) - mousePosition;

            // Check if the mouse is captured.
            if (grid.IsMouseCaptured)
            {
                // Reset the x and y pos.
                (RenderTransform as TranslateTransform).X = diff.X;
                (RenderTransform as TranslateTransform).Y = diff.Y;

                // Update the position.
                Position.X = diff.X;
                Position.Y = diff.Y;
            }
        }

        // Resizing
        public void OnResizeDragStarted(object sender, DragStartedEventArgs e)
        {
            // Return if locked.
            if (Locked) return;

        }

        public void OnResizeDragCompleted(object sender, DragCompletedEventArgs e)
        {
            // Return if locked.
            if (Locked) return;

        }

        public void OnResizeDragDelta(object sender, DragDeltaEventArgs e)
        {
            // Return if locked.
            if (Locked) return;

            // Set the content.
            OverlayWindowBase content = sender as OverlayWindowBase;

            // Get the x and y.
            double yAdjust = content.Height + e.VerticalChange;
            double xAdjust = content.Width + e.HorizontalChange;

            // Make sure not to resize to negative width or heigth .           
            xAdjust = (content.ActualWidth + xAdjust) > content.MinWidth ? xAdjust : content.MinWidth;
            yAdjust = (content.ActualHeight + yAdjust) > content.MinHeight ? yAdjust : content.MinHeight;

            // Set the x and y.
            content.Width = xAdjust;
            content.Height = yAdjust;
        }

        // Timer
        public virtual void Timer_Tick(object sender, EventArgs e)
        {
        }

        // ParentWindow
        public virtual void Logger_OnKeyDown(object sender, System.Windows.Forms.Keys code)
        {
        }

        public virtual void ParentWindow_Closing(object sender, CancelEventArgs e)
        {
        }

        #endregion
    }
}
