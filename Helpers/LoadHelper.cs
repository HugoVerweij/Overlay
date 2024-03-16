using Overlay.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Overlay
{
    public static class LoadHelper
    {
        public static async Task<List<OverlayWindowState>> LoadWidgetsAsync()
        {
            List<OverlayWindowState> result = new List<OverlayWindowState>();

            // Run the load on a different thread.
            await Task.Run(() =>
            {
                try
                {
                    // Return if the file doesn't exists.
                    if (!File.Exists(Paths.Widgets)) return;

                    // Create a new serializer.
                    XmlSerializer ser = new XmlSerializer(typeof(List<OverlayWindowState>));

                    // Create a new reader.
                    using XmlReader reader = XmlReader.Create(Paths.Widgets);

                    // Set the data.
                    result = (List<OverlayWindowState>)ser.Deserialize(reader);
                }
                catch
                {
                    // Log error.
                }
            });

            return result;
        }

        public static async Task<object> LoadTypeDataAsync(Type type, string path)
        {
            object result = null;

            await Task.Run(() =>
            {
                // Return if the path doesn't exists.
                if (!File.Exists(path)) return;

                // Create a new serializer.
                XmlSerializer ser = new XmlSerializer(typeof(List<>).MakeGenericType(type));

                // Create a new reader.
                using XmlReader reader = XmlReader.Create(Paths.Widgets);

                // Set the data.
                result = ser.Deserialize(reader);
            });

            return result;
        }
    }
}
