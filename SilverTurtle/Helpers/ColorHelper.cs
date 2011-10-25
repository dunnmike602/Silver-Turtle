/**********************************************************************************************************
 *******                                                                                            *******
 *******  ColorHelpers                                                                              *******
 *******  Olaf Rabbachin 02/2010                                                                    *******
 *******                                                                                            *******
 *******  Taken from the blog post http://www.blogs.intuidev.com/post/2010/02/05/ColorHelper.aspx   *******
 *******                                                                                            *******
 *******  2010-02-13 Update: Hash-comparison replaced with object comparison                        ******* 
 *******             (the hashcodes returned by the GetHashCode() method aren't unique)             *******
 *******                                                                                            *******
 *******  2010-02-15 Update: try-catch for GetColorByName added                                     *******
 *******                                                                                            *******
 **********************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Reflection;

namespace TurtleWpf.Helpers
{
    public static class ColorHelper
    {
        public static SolidColorBrush GetSolidColorBrush(Color color)
        {
            var brush = new SolidColorBrush {Color = color};
            
            #if !SILVERLIGHT
            brush.Freeze();
            #endif
            return brush;
        }

        /// <summary>
        /// Returns a list containing all known colors, each as a KeyValuePair with the name
        /// as the key and the Color as the value.
        /// </summary>
        public static List<KeyValuePair<string, Color>> GetKnownColors()
        {
            var colorType = typeof(Colors);
            var arrPiColors = colorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            return arrPiColors.Select(pi => new KeyValuePair<string, Color>(pi.Name, (Color) pi.GetValue(null, null))).ToList();
        }

        public static Color[] GetKnownColorsAsArray()
        {
            var colorType = typeof(Colors);
            var arrPiColors = colorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            return arrPiColors.Select(pi => (Color)pi.GetValue(null, null)).ToArray();
        }

        /// <summary>
        /// Returns the known name of the color passed (if found), or an empty string.
        /// </summary>
        /// <param name="color">The color whose name is to be returned.</param>
        /// <returns>
        /// The name of the passed color resp. an empty string if 
        /// no matching known color could be found.
        /// </returns>
        public static string GetKnownColorName(Color color)
        {
            //Use reflection to get all known colors
            var colorType = typeof(Colors);
            var arrPiColors = colorType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            //Iterate over all known colors, convert each to a <Color> and then compare
            //that color to the passed color.
            foreach (var pi in arrPiColors)
            {
                var clrKnownColor = (Color)pi.GetValue(null, null);
                if (clrKnownColor == color) return pi.Name;
            }

            return string.Empty;
        }
    }
}
