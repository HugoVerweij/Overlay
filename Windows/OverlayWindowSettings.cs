using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Overlay.Windows
{
    public struct OverlayWindowSettings
    {
        public MainWindow Parent;
        public StyleTypes Style;
        public Point StartPos;
        public double ZIndex;
        public double Width;
        public double Height;
        public bool Locked;
    }
}
