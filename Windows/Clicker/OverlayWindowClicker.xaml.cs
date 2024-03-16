using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Overlay.Windows.Clicker
{
    /// <summary>
    /// Interaction logic for OverlayWindowClicker.xaml
    /// </summary>
    public partial class OverlayWindowClicker : OverlayWindowBase
    {
        #region Variables

        private bool clicking = false;

        #endregion

        #region OnLoad

        public OverlayWindowClicker(OverlayWindowSettings settings) : base(settings)
        {
            InitializeComponent();
        }

        private void OverlayWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region Tasks

        private async Task AutoClick(int cps, TimeSpan duration)
        {
            if (clicking) return;
            clicking = true;

            Stopwatch time = new Stopwatch();
            time.Start();

            while (time.ElapsedMilliseconds < duration.TotalMilliseconds)
            {
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                await Task.Delay(1000 / cps);
            }

            clicking = false;
        }

        #endregion

        #region Events

        public override async void Logger_OnKeyDown(object sender, Keys code)
        {
            if (code == Keys.D4) await AutoClick(13, TimeSpan.FromSeconds(20));
        }

        #endregion
    }
}
