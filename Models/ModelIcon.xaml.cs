using Overlay.Windows;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Overlay.Models
{
    /// <summary>
    /// Interaction logic for ModelIcon.xaml
    /// </summary>
    public partial class ModelIcon : UserControl
    {
        // Public.
        public Type Widget;

        // Event.
        public delegate void ModelIconEventHandler(object sender, EventArgs e);
        public event ModelIconEventHandler OnClick;

        public ModelIcon(Type widget, BitmapSource icon)
        {
            // Initialize the normal components.
            InitializeComponent();

            // Set the widget.
            Widget = widget;

            // Set the icon image.
            imageIcon.Source = icon;

            // Set the icon text.
            textblockName.Text = widget.Name.Replace("OverlayWindow", "");
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            textblockName.Foreground = System.Windows.Media.Brushes.SeaGreen;
            OnClick?.Invoke(Widget, new EventArgs());
        }
    }
}
