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
using System.Collections.Generic;
using SilverTurtle.Models;
using SilverTurtle.ViewModels.Interfaces;
using SimpleMvvmToolkit;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Helpers;

namespace SilverTurtle.ViewModels
{
    public class TurtleMethodViewModel : ViewModelDetailBase<TurtleMethodViewModel, TurtleMethod>, ITurtleMethodViewModel
    {
        private readonly List<TurtleMethod> _turtleMethods = new List<TurtleMethod>();

        private void PopulateMethods()
        {
            foreach (var method in ReflectionHelper.GetAllTurtleMethodNames())
            {
                var attribute = (TurtleGraphicsAttribute)method.GetCustomAttributes(typeof(
                  TurtleGraphicsAttribute), false)[0];

                var turtleMethod = new TurtleMethod
                                       {
                                           Alias = attribute.Alias,
                                           Usage = attribute.Usage,
                                           Description = attribute.Description,
                                           Name = attribute.CommandText
                                       };

                _turtleMethods.Add(turtleMethod);
            }
        }

        public TurtleMethodViewModel()
        {
            PopulateMethods();
        }

        public List<TurtleMethod> TurtleMethods
        {
            get { return _turtleMethods; }
        }
    }
}