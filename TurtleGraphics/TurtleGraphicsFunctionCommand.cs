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
using TurtleGraphics.Enums;
using TurtleGraphics.Helpers;
using TurtleGraphics.Interfaces;

namespace TurtleGraphics
{
    public class TurtleGraphicsFunctionCommand : TurtleGraphicsCommand
    {
        public string FunctionName
        {
            get { return ArgumentValues[0]; }
        }

        public override void ValidateCommand()
        {
            Status = TurtleGraphicsCommandStatus.Valid;

            if (Commands.Count == 0)
            {
                Status = TurtleGraphicsCommandStatus.FunctionHasNoCommands;
                ErrorMessage = ReflectionHelper.GetEnumDescription(Status);
                return;
            }

            base.ValidateCommand();
        }

        /// <summary>
        /// Do nothing in this method as functions are not directly executed.
        /// </summary>
        public override bool Execute(ICanceller canceller)
        {
            if (canceller.ShouldCancel())
            {
                return false;
            }

            return true;
        }
    }
}