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

using System.ComponentModel;
using System.Windows;
using SilverTurtle.ViewModels;
using SilverTurtle.ViewModels.Interfaces;

namespace SilverTurtle.Locators
{
    public class ViewModelLocator
    {
        private readonly ITurtleMethodViewModel _turtleMethodViewModel;
        private readonly ITurtleColorViewModel _turtleColorViewModel;
        private readonly ITurtleViewModel _turtleViewModel;

        public ViewModelLocator()
        {
            if(!DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                 _turtleMethodViewModel = new TurtleMethodViewModel();
                 _turtleColorViewModel = new TurtleColorViewModel();
                 _turtleViewModel = new TurtleViewModel();
            }
        }

        public ITurtleMethodViewModel TurtleMethodViewModel
        {
            get { return _turtleMethodViewModel; }
        }

        public ITurtleColorViewModel TurtleColorViewModel
        {
            get { return _turtleColorViewModel; }
        }

        public ITurtleViewModel TurtleViewModel
        {
            get { return _turtleViewModel; }
        }
    }
}
