using Overlay.Windows;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace Overlay.Styles
{
    public partial class UserControls
    {
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            // Fetch the overlay window base.
            Button button = sender as Button;
            OverlayWindowBase parent = button.Tag as OverlayWindowBase;

            // Invoke the click event.
            parent.OnMinimizeClick(sender, e);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Fetch the overlay window base.
            Button button = sender as Button;
            OverlayWindowBase parent = button.Tag as OverlayWindowBase;

            // Invoke the click event.
            parent.OnExitClick(sender, e);
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            // Fetch the overlay window base.
            Grid title = sender as Grid;
            OverlayWindowBase parent = title.Tag as OverlayWindowBase;

            // Invoke the drag event.
            parent.OnTitleBarDrag(sender, e);
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Fetch the overlay window base.
            Grid title = sender as Grid;
            OverlayWindowBase parent = title.Tag as OverlayWindowBase;

            // Invoke the down event.
            parent.OnTitleBarMouseDown(sender, e);
        }

        private void TitleBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Fetch the overlay window base.
            Grid title = sender as Grid;
            OverlayWindowBase parent = title.Tag as OverlayWindowBase;

            // Invoke the up event.
            parent.OnTitleBarMouseUp(sender, e);
        }

        private void OnResizeThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            OverlayWindowBase parent = thumb.Tag as OverlayWindowBase;

            // Invoke the up event.
            parent.OnResizeDragStarted(parent, e);
        }

        private void OnResizeThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            // Fetch the overlay window base.
            Thumb thumb = sender as Thumb;
            OverlayWindowBase parent = thumb.Tag as OverlayWindowBase;

            // Invoke the up event.
            parent.OnResizeDragCompleted(parent, e);
        }

        private void OnResizeThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            // Fetch the overlay window base.
            Thumb thumb = sender as Thumb;
            OverlayWindowBase parent = thumb.Tag as OverlayWindowBase;

            // Invoke the up event.
            parent.OnResizeDragDelta(parent, e);
        }

        private async void Lock_Loaded(object sender, RoutedEventArgs e)
        {
            // Fetch the overlay window base.
            Image image = sender as Image;
            OverlayWindowBase parent = image.Tag as OverlayWindowBase;

            // Fetch the image set.
            ImageSet set = await ImageHelper.GetImageSet("Lock");
            image.Source = parent.Locked ? set.Highlight : set.Default;
        }

        private async void Lock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Fetch the overlay window base.
            Image image = sender as Image;
            OverlayWindowBase parent = image.Tag as OverlayWindowBase;

            // Swap the locked state.
            parent.Locked = !parent.Locked;

            // Fetch the image set.
            ImageSet set = await ImageHelper.GetImageSet("Lock");
            image.Source = parent.Locked ? set.Highlight : set.Default;
        }

        private async void Exit_Loaded(object sender, RoutedEventArgs e)
        {
            // Fetch the image.
            Image image = sender as Image;

            // Fetch the image set.
            ImageSet set = await ImageHelper.GetImageSet("Cross");
            image.Source = set.Default;
        }

        private void Exit_Click(object sender, MouseButtonEventArgs e)
        {
            // Fetch the overlay window base.
            Image image = sender as Image;
            OverlayWindowBase parent = image.Tag as OverlayWindowBase;

            // Invoke the exit event.
            parent.OnExitClick(sender, e);
        }

        private async void Exit_MouseEnter(object sender, MouseEventArgs e)
        {
            // Fetch the image.
            Image image = sender as Image;

            // Fetch the image set.
            ImageSet set = await ImageHelper.GetImageSet("Cross");
            image.Source = set.Highlight;
        }

        private async void Exit_MouseLeave(object sender, MouseEventArgs e)
        {
            // Fetch the image.
            Image image = sender as Image;

            // Fetch the image set.
            ImageSet set = await ImageHelper.GetImageSet("Cross");
            image.Source = set.Default;
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Fetch the overlay window base.
            Border border = sender as Border;
            OverlayWindowBase parent = border.Tag as OverlayWindowBase;

            // Invoke the exit event.
            parent.OnControlClick(sender, e);
        }
    }
}
