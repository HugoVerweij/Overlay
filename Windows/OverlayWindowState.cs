using System;
using System.Windows;
using System.Xml.Serialization;

namespace Overlay.Windows
{
    [Serializable]
    [XmlType("Settings")]
    public class OverlayWindowState
    {
        [XmlElement("Key")]
        public string Key;
        [XmlElement("Style")]
        public StyleTypes Style;
        [XmlElement("Position")]
        public Point Position;
        [XmlElement("ZIndex")]
        public double ZIndex;
        [XmlElement("Width")]
        public double Width;
        [XmlElement("Height")]
        public double Height;
        [XmlElement("Locked")]
        public bool Locked;
    }
}
