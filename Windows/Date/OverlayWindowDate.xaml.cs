using System;
using System.Collections.Generic;
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

namespace Overlay.Windows.Date
{
    /// <summary>
    /// Interaction logic for OverlayWindowDate.xaml
    /// </summary>
    public partial class OverlayWindowDate : OverlayWindowBase
    {
        #region Variables

        #endregion

        #region OnLoad

        public OverlayWindowDate(OverlayWindowSettings settings) : base(settings)
        {
            InitializeComponent();
        }

        private void OverlayWindowDate_Loaded(object sender, RoutedEventArgs e)
        {
            TimerUpdate();
        }

        #endregion

        #region Methods

        private void TimerUpdate()
        {
            // Update the timer.
            textblockDate.Text = DateTime.Now.ToString("dddd, dd MMMMM yyyy");
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
