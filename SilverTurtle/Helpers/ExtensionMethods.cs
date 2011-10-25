#region SILVERTURTLE - GPL Copyright (c) 2011 MLD Computing Limited
//
// This file is part of SILVERTURTLE.
//
// SILVERTURTLE is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// SILVERTURTLE is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SilverTurtle; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//================================================================================
#endregion
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace SilverTurtle.Helpers
{ 
    public static class ExtensionMethods
    {
        public static uint ColorToUInt(this Color color)
        {
            return (uint) ((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
        }

        public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Converter<TInput, TOutput> converter)
        {
            if (array == null)
                throw new ArgumentException();

            return (from item in array select converter(item)).ToArray();
        }

        public static Color ToColor(this uint argb)
        {
            return Color.FromArgb((byte)((argb & -16777216) >> 0X18),
            (byte)((argb & 0xff0000) >> 0X10),
            (byte)((argb & 0xff00) >> 8),
            (byte)(argb & 0xff));
        }

        public static Array GetValues(this Enum enumType)
        {
            var type = enumType.GetType();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            var array = Array.CreateInstance(type, fields.Length);

            for (var i = 0; i < fields.Length; i++)
            {
                var obj = fields[i].GetValue(null);
                array.SetValue(obj, i);
            }

            return array;
        }
    }
}
