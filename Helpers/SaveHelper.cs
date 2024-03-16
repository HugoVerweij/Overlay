using Overlay.Windows;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Overlay
{
    public static class SaveHelper
    {
        public async static void SaveTypeof(Type _type, object _object, string _path)
        {
            // Run the save on a different thread.
            await Task.Run(() =>
            {
                try
                {
                    // Create the doc.
                    XDocument doc = new XDocument();

                    // Assign the writer.
                    using (XmlWriter writer = doc.CreateWriter())
                    {
                        // Create the serializer and serialize the data.
                        XmlSerializer serializer = new XmlSerializer(_type);
                        serializer.Serialize(writer, _object);
                    }

                    // Save the document.
                    doc.Save(_path);
                }
                catch (Exception e)
                {
                    // Handle exception.
                    Console.WriteLine(e);
                }
            });
        }
    }
}
