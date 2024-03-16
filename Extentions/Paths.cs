using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay
{
    public static class Paths
    {
        // Root.
        public static string ExeDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string XmlDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XML");

        // Xml.
        public static string Widgets = Path.Combine(XmlDir, "Widgets.xml");
        public static string Playlists = Path.Combine(XmlDir, "Playlists.xml");
    }
}
