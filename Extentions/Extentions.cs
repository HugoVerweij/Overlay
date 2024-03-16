using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;

namespace Overlay
{
    public static class Extentions
    {
        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Fetches every type class thoughout the assembly using a namespace.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="nameSpace"></param>
        /// <returns></returns>
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                      .ToArray();
        }

        /// <summary>
        /// Fetches every type class throughout the assembly using a type.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetTypesInAssembly(Assembly assembly, Type type)
        {
            return
                assembly.GetTypes()
                        .Where(t => type.IsAssignableFrom(t) && t != type)
                        .ToArray();
        }

        public static async Task<Bitmap> ChangeImageColor(Bitmap bitmap, Color color)
        {
            try
            {
                // Set the bitmap.
                Bitmap map = new Bitmap(bitmap);

                await Task.Run(() =>
                {
                    // Loop through the x and y.
                    for (int x = 0; x < map.Width; x++)
                    {
                        for (int y = 0; y < map.Height; y++)
                        {
                            // Get the pixel
                            Color pixel = map.GetPixel(x, y);

                            // Set the color if it's invis.
                            if (pixel.A != 0)
                            {
                                // Set the color.
                                map.SetPixel(x, y, color);
                            }
                        }
                    }
                });

                // Return the image.
                return map;
            }
            catch
            {
                return null;
            }
        }

        public static BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            // Get the hbit from the bitmap.
            IntPtr Hbit = bitmap.GetHbitmap();

            // Set the result.
            BitmapSource result;

            try
            {
                // Attempt to convert.
                result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    Hbit,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                // Set the result to null.
                result = null;
            }
            finally
            {
                // Prevent a memory leak.
                DeleteObject(Hbit);
            }

            // Return the result.
            return result;
        }

        public static Bitmap ImageSourceToBitmap(BitmapSource source)
        {
            // Set the result.
            Bitmap result = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);

            // Set the data.
            BitmapData data = result.LockBits(new Rectangle(Point.Empty, result.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            
            // Copy the pixels.
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);

            // Unlock the bits.
            result.UnlockBits(data);

            // Return the result.
            return result;
        }

        public static Bitmap GetIconFromWidget(Type widget)
        {
            // Set the result.
            Bitmap result = null;

            // Fetch the project resource set.
            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            // Loopt through the results.
            foreach (DictionaryEntry entry in resourceSet)
            {
                // Check if the entry matches the widget.
                if (entry.Key.ToString().Equals(widget.Name.Replace("OverlayWindow", "Widget")))
                {
                    // Set the result and break.
                    result = (Bitmap)entry.Value;
                    break;
                }
            }

            // Return the result.
            return result;
        }

        public static decimal Map(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static string FormatTime(this TimeSpan time)
        {
            // Return the formatted time depending if the timespan is long enough for hours.
            return time.TotalHours >= 1 ? time.ToString(@"hh\:mm\:ss") : time.ToString(@"mm\:ss");
        }
    }
}
