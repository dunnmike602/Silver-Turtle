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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;
using TurtleGraphics.Interfaces;

namespace TurtleGraphics.Helpers
{
    public static class ReflectionHelper
    {
        private static List<TurtleGraphicsAttribute> _turtleGraphicsCommandAttributes;
        private static List<MethodInfo> _turtleGraphicsMethodNames;
        private static IEnumerable<MethodInfo> _turtleGraphicsCommandMethods;
        private static IEnumerable<MethodInfo> _turtleGraphicsControlStructureMethods;

        private static void EnsureMethodCollections()
        {
            if (_turtleGraphicsCommandMethods != null) 
            {    
                return;
            }
            
            _turtleGraphicsCommandMethods = typeof(ITurtleGraphicsCommands).GetMethods().
                Where(method => method.GetCustomAttributes(typeof(TurtleGraphicsAttribute), false).Length != 0);

            _turtleGraphicsControlStructureMethods = typeof(ITurtleGraphicsControlStructures).GetMethods().
                Where(method => method.GetCustomAttributes(typeof(TurtleGraphicsAttribute), false).Length != 0);
        }

        /// <summary>
        /// Given a TurtleGraphicsCommand that represents a textual command to the turtle graphics system
        /// reflection is used to get a reference to the actual implementation method (not the underlying interface method)
        /// </summary>
        public static string GetMethodForCommand(string commandText, TurtleGraphicsCommandTypes type)
        {
            EnsureMethodCollections();

            var methods = type == TurtleGraphicsCommandTypes.Commands ? _turtleGraphicsCommandMethods : 
                _turtleGraphicsControlStructureMethods;

            var implementingMethod = (from imethod in methods
                                      let turtleGraphicsAttribute = (TurtleGraphicsAttribute)imethod.
                                      GetCustomAttributes(typeof(TurtleGraphicsAttribute), true)[0]
                                      where (turtleGraphicsAttribute.CommandText == commandText ||
                                      turtleGraphicsAttribute.Alias == commandText)
                                      select imethod).FirstOrDefault();

            return implementingMethod == null ? null : implementingMethod.Name;
        }

        public static List<MethodInfo> GetAllTurtleMethodNames()
        {
            if (_turtleGraphicsMethodNames == null)
            {
                var turtleGraphicsControlStructuresMethods = typeof (ITurtleGraphicsCommands).GetMethods().Where(
                    m => m.GetCustomAttributes(true).Length > 0);

                var turtleGraphicsSystemMethods = typeof(ITurtleGraphicsControlStructures).GetMethods().Where(
                    m => m.GetCustomAttributes(true).Length > 0);

                _turtleGraphicsMethodNames = turtleGraphicsControlStructuresMethods.Union(turtleGraphicsSystemMethods).
                    OrderBy(method => method.Name).ToList();
            }

            return _turtleGraphicsMethodNames;
        }

        /// <summary>
        /// This methood uses reflection to extract metadata attributes from the interface name TurtleGraphicsSystemInterfaceName
        /// These attributes contain the mappings between textual commands and the methods that implement them
        /// </summary>
        public static List<TurtleGraphicsAttribute> GetTurtleGraphicsCommandAttributes()
        {
            if (_turtleGraphicsCommandAttributes == null)
            {
                var turtleGraphicsSystemInterfaceMethodQuery = typeof(ITurtleGraphicsCommands)
                    .GetMethods().Where(method => method.GetCustomAttributes(typeof (TurtleGraphicsAttribute), false).Length != 0);

                var systemAttributesIntefaceQuery =
                    turtleGraphicsSystemInterfaceMethodQuery.Select(
                        method =>
                        (TurtleGraphicsAttribute)method.GetCustomAttributes(typeof(TurtleGraphicsAttribute), true)[0]);

                var turtleGraphicsSystemControlMethodQuery = typeof (ITurtleGraphicsControlStructures)
                    .GetMethods().Where(method => method.GetCustomAttributes(typeof (TurtleGraphicsAttribute), false).
                        Length != 0);

                var controlAttributesIntefaceQuery =
                    turtleGraphicsSystemControlMethodQuery.Select(
                        method =>
                        (TurtleGraphicsAttribute)method.GetCustomAttributes(typeof(TurtleGraphicsAttribute), true)[0]);


                _turtleGraphicsCommandAttributes = controlAttributesIntefaceQuery.Union(systemAttributesIntefaceQuery).ToList();
            }

            return _turtleGraphicsCommandAttributes;
        }
        
        /// <summary>
        /// This methood uses reflection to extract metadata attributes for the specified method.
        /// </summary>
        public static List<TurtleGraphicsArgumentAttribute> GetArgumentAttributesForMethod(string methodName)
        {
            GetAllTurtleMethodNames();

            var attributes =
                (TurtleGraphicsArgumentAttribute[])
                _turtleGraphicsMethodNames.Where(method => method.Name == methodName).First().GetCustomAttributes(
                    typeof (
                        TurtleGraphicsArgumentAttribute), false);

            var turtleGraphicsArgumentAttribute = new List<TurtleGraphicsArgumentAttribute>();
            turtleGraphicsArgumentAttribute.AddRange(attributes.OrderBy(s => s.Order));

            return turtleGraphicsArgumentAttribute;
        }

        public static string GetEnumDescription(Enum value)
        {
            // Get the Description attribute value for the enum value
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), 
                false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
