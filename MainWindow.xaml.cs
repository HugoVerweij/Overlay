using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Overlay.Windows;
using Overlay.Models;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.ComponentModel;
using Control = System.Windows.Forms.Control;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using Panel = System.Windows.Controls.Panel;

namespace Overlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        // Public
        public BlurHelper Blur;
        public KeyHookHelper Logger;
        public DispatcherTimer Update = new DispatcherTimer(DispatcherPriority.Render);
        public readonly List<OverlayWindowBase> Widgets = new List<OverlayWindowBase>();

        // Private
        private readonly Storyboard up = new Storyboard();
        private readonly Storyboard down = new Storyboard();
        private bool trayAnimating;
        private bool mouseInHover;
        private bool mouseInTray;

        #endregion

        #region OnLoad

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle window specific variables.
            Topmost = true;

            #region Helpers

            // Define the blur helper.
            Blur = new BlurHelper();

            // Enable the blur.
            Blur.ToggleBlur(this, BlurHelper.AccentState.ACCENT_ENABLE_BLURBEHIND);

            // Define the keyhook helper.
            Logger = new KeyHookHelper(this);

            // Start the logger, and subscribe to the keydown event.
            Logger.Start();
            Logger.OnKeyDown += KeyHook_OnKeyDown;

            #endregion

            #region Load Widgets
            
            // Load the tray animations.
            LoadAnimations();

            // Load the images.
            await ImageHelper.LoadImages();

            // Load the widgets.
            LoadAssemblyWidgetsAsync();

            #endregion

            #region Timer

            // Define a new system wide timer.
            Update.Interval = TimeSpan.FromSeconds(1);
            Update.Start();
            
            // Subscribe to the tick event.
            Update.Tick += (ss, ee) =>
            {
                // Loop through every active widgt.
                foreach (OverlayWindowBase widget in Widgets.Where(x => x.Timer != null && !x.Timer.IsEnabled))
                    // Start their timer on the same tick.
                    widget.Timer.Start();
            };

            #endregion
        }

        #endregion

        #region Methods

        private void CreateWidget(OverlayWindowBase widget)
        {
            // Subscribe to the correct events.
            widget.OnExit += Widget_OnExit;
            widget.OnClick += Widget_OnClick;

            // Set the initial zindex.
            int zindex = widget.ZIndex > 0 ? (int)widget.ZIndex : WidgetArea.Children.Count + 1;

            // Update the chosen zindex.
            widget.ZIndex = zindex;

            // Set the actual zindex.
            Panel.SetZIndex(widget, zindex);

            // Add the widget to the lists.
            Widgets.Add(widget);
            WidgetArea.Children.Add(widget);
        }

        private void LoadAnimations()
        {
            // Define the up/down animations.
            GridLengthAnimation animation1 = new GridLengthAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                From = new GridLength(0, GridUnitType.Star),
                To = new GridLength(0.3, GridUnitType.Star),
                DecelerationRatio = 1
            };
            GridLengthAnimation animation2 = new GridLengthAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                From = new GridLength(0.3, GridUnitType.Star),
                To = new GridLength(0, GridUnitType.Star),
                AccelerationRatio = 1
            };

            // Define the fadein/fadeout animations.
            DoubleAnimation fade1 = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                From = 0.0,
                To = 1.0,
                DecelerationRatio = 1

            };
            DoubleAnimation fade2 = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                From = 1.0,
                To = 0.0,
                AccelerationRatio = 1
            };

            // Set the targets.
            Storyboard.SetTarget(fade1, kek);
            Storyboard.SetTargetProperty(fade1, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animation1, rowOverlayTray);
            Storyboard.SetTargetProperty(animation1, new PropertyPath("Height"));

            // Set the targets.
            Storyboard.SetTarget(fade2, kek);
            Storyboard.SetTargetProperty(fade2, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animation2, rowOverlayTray);
            Storyboard.SetTargetProperty(animation2, new PropertyPath("Height"));

            // Add the animations.
            up.Children.Add(animation1);
            up.Children.Add(fade1);
            down.Children.Add(fade2);
            down.Children.Add(animation2);

            // Subscribe to the completed events.
            up.Completed += (ss, ee) =>
            {
                // Turn tray animating off.
                trayAnimating = false;

                // Check if the mouse is no longer in the tray.
                if (!mouseInTray)
                {
                    // Turn animating on.
                    trayAnimating = true;
                    // Force the down animation.
                    down.Begin();
                }
            };

            // Subscribe to the completed events.
            down.Completed += (ss, ee) =>
            {
                // Turn tray animating off.
                trayAnimating = false;

                // Check the mouse is in the hover area.
                if (mouseInHover)
                {
                    // Turn animating on.
                    trayAnimating = true;
                    // Force the up animation.
                    up.Begin();
                }
            };
        }

        #endregion

        #region Tasks

        private async void LoadAssemblyWidgetsAsync()
        {
            // Start a new task.
            await Task.Run(() =>
            {
                // Invoke on the new thread.
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    // Load the states form the xml file.
                    List<OverlayWindowState> saved = await LoadHelper.LoadWidgetsAsync();

                    // Crawl through the assembly and fetch every assignable class from the base class.
                    foreach (Type type in Extentions.GetTypesInAssembly(Assembly.GetExecutingAssembly(), typeof(OverlayWindowBase)))
                    {
                        #region Icon

                        // Fetch the image set.
                        ImageSet set = await ImageHelper.GetImageSet(type.Name.Replace("OverlayWindow", "Widget"));

                        // Fetch the default image.
                        BitmapSource iconParsed = set?.Highlight;

                        // Define an icon.
                        ModelIcon icon = new ModelIcon(type, iconParsed)
                        {
                            Width = 64,
                            Height = 64,
                            Margin = new Thickness(2.5)
                        };

                        // Subscribe to the onclick event.
                        icon.OnClick += (ss, ee) =>
                        {
                            // Create a new settings struct.
                            OverlayWindowSettings settings = new OverlayWindowSettings()
                            {
                                Parent = this,
                                Style = StyleTypes.Null,
                                StartPos = new Point(Width / 2 - 200, Height / 2 - 100),
                                Width = 400,
                                Height = 200,
                            };

                            // Make sure the widget can't be relaunched twice.
                            if (Widgets.All(x => x.GetType() != (Type)ss))
                            {
                                // Create an instance of the widget.
                                OverlayWindowBase widget = (OverlayWindowBase)Activator.CreateInstance((Type)ss, settings);

                                // Create the widget on the overlay.
                                CreateWidget(widget);
                            }
                        };

                        // Add the icon to the content area.
                        contentArea.Children.Add(icon);

                        #endregion

                        #region Saved

                        // Check if the saved widgets contains the current widget.
                        if (saved.Any(x => x.Key.Equals(type.Name)))
                        {
                            // Fetch the state before the program was closed.
                            OverlayWindowState state = saved.Where(x => x.Key.Equals(type.Name)).FirstOrDefault();

                            // Create a new settings struct.
                            OverlayWindowSettings settings = new OverlayWindowSettings()
                            {
                                Parent = this,
                                Style = state.Style,
                                StartPos = state.Position,
                                ZIndex = state.ZIndex,
                                Width = state.Width,
                                Height = state.Height,
                                Locked = state.Locked
                            };

                            // Create an instance of the widget.
                            OverlayWindowBase widget = (OverlayWindowBase)Activator.CreateInstance(type, settings);

                            // Create the widget on the overlay.
                            CreateWidget(widget);
                        }

                        #endregion
                    }
                }), DispatcherPriority.ContextIdle);
            });
        }

        #endregion

        #region Events

        #region Widget

        private void Widget_OnClick(object sender, EventArgsBase e)
        {
            // Cast the object sender to the actual widget class.
            OverlayWindowBase widget = sender as OverlayWindowBase;

            // Return if the widget is already at the top.
            if (widget.ZIndex >= WidgetArea.Children.Count) return;

            // Select every control that has a bigger zindex than the sender.
            foreach(OverlayWindowBase w in Widgets.Where(x => x.ZIndex > widget.ZIndex))
            {
                // Subtract the zindex.
                Widgets[Widgets.IndexOf(w)].ZIndex--;

                // Set the actual zindex.
                Panel.SetZIndex(Widgets[Widgets.IndexOf(w)], (int)Widgets[Widgets.IndexOf(w)].ZIndex);
            }

            // Set the sender's zindex to the max.
            Widgets[Widgets.IndexOf(widget)].ZIndex = WidgetArea.Children.Count;

            // Set the actual zindex.
            Panel.SetZIndex(widget, (int)Widgets[Widgets.IndexOf(widget)].ZIndex);
        }

        private void Widget_OnExit(object sender, EventArgsBase e)
        {
            // Cast the object sender to the actual widget class.
            OverlayWindowBase widget = sender as OverlayWindowBase;

            // Remove the widget from the global list and content area.
            Widgets.Remove(widget);
            WidgetArea.Children.Remove(widget);
        }

        #endregion

        private void KeyHook_OnKeyDown(object sender, Keys code)
        {
            // Check if the correct combination of keys has been pressed.
            if (code == Keys.Tab && Control.ModifierKeys == Keys.Control)
            {
                // Swap the visibility.
                Visibility = Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

                // Focus.
                Focus();
            }
        }

        #region Window

        private void Window_Deactivated(object sender, EventArgs e)
        {
            // Hide the window on lost focus.
            Visibility = Visibility.Hidden;
        }

        #endregion

        private void Hover_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set the mouse in hover to true.
            mouseInHover = true;

            // Return if the tray is already animating.
            if (trayAnimating) return;

            // Set animating to true.
            trayAnimating = true;

            // Start the animation.
            up.Begin();
        }

        private void Hover_MouseLeave(object sender, MouseEventArgs e)
        {
            // Set the mouse in hover to false.
            mouseInHover = false;
        }

        private void Tray_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set the mouse in tray to true.
            mouseInTray = true;
        }

        private void Tray_MouseLeave(object sender, MouseEventArgs e)
        {
            // Set the mouse in tray to false.
            mouseInTray = false;

            // Return if the tray is already animating.
            if (trayAnimating) return;

            // Set the animating to true.
            trayAnimating = true;

            // Start the animation.
            down.Begin();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Define a states list.
            List<OverlayWindowState> states = new List<OverlayWindowState>();

            // Loop through every active widget.
            foreach (OverlayWindowBase widget in Widgets)
            {
                // Create a new state for said widget.
                OverlayWindowState state = new OverlayWindowState()
                {
                    Key = widget.GetType().Name,
                    Style = widget.StyleType,
                    Position = widget.Position,
                    ZIndex = widget.ZIndex,
                    Width = widget.Width,
                    Height = widget.Height,
                    Locked = widget.Locked
                };

                // Add the state.
                states.Add(state);
            }

            // Save the states.
            SaveHelper.SaveTypeof(typeof(List<OverlayWindowState>), states, Paths.Widgets);

            // Close out.
            Environment.Exit(0);
        }

        #endregion
    }
}
