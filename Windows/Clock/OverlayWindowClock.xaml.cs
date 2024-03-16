using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Overlay.Windows.Clock
{
    /// <summary>
    /// Interaction logic for OverlayWindowClock.xaml
    /// </summary>
    public partial class OverlayWindowClock : OverlayWindowBase
    {
        #region Variables

        #endregion

        #region OnLoad

        public OverlayWindowClock(OverlayWindowSettings settings) : base (settings)
        {
            InitializeComponent();
        }

        private void OverlayWindowClock_Loaded(object sender, RoutedEventArgs e)
        {
            TimerUpdate();
        }

        #endregion

        #region Methods

        private void TimerUpdate()
        {
            // Update the timer.
            textBlockTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        public override void Timer_Tick(object sender, EventArgs e)
        {
            // Call the base method.
            base.Timer_Tick(sender, e);

            // Update the timer.
            TimerUpdate();
        }

        #endregion
    }
}
