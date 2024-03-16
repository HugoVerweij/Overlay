using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Overlay
{
    public class ImageSet
    {
        public BitmapSource Default;
        public BitmapSource Highlight;
    }

    public static class ImageHelper
    {
        public static Dictionary<string, ImageSet> Images = new Dictionary<string, ImageSet>();
        public static bool Loaded;

        public static async Task<ImageSet> GetImageSet(string key)
        {
            // Wait until either the image set is present, or the images are loaded.
            while (!Images.ContainsKey(key) && !Loaded) await Task.Delay(25);

            // Return the result.
            return Images.ContainsKey(key) ? Images[key] : null;
        }

        // TODO Rework with tasks.
        public static async Task LoadImages()
        {
            // Define a new list of tasks.
            List<Task> tasks = new List<Task>();

            // Define the global resource set.
            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            // Loop through every resource.
            foreach (DictionaryEntry entry in resourceSet)
            {
                //// Add the task to the list.
                //tasks.Add(Task.Run(async () =>
                //{

                //}));
                Bitmap bitmap = (Bitmap)entry.Value;

                ImageSet set = new ImageSet()
                {
                    Default = Extentions.BitmapToImageSource(await Extentions.ChangeImageColor(bitmap, Color.Silver)),
                    Highlight = Extentions.BitmapToImageSource(await Extentions.ChangeImageColor(bitmap, Color.White)),
                };

                Images.Add(entry.Key.ToString(), set);
            }

            //// Wait until everything has finished.
            //await Task.WhenAll(tasks);

            // Turn loaded to true.
            Loaded = true;
        }
    }
}
