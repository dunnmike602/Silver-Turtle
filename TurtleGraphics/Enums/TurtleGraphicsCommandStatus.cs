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

namespace TurtleGraphics.Enums
{
    public enum TurtleGraphicsCommandStatus
    {
        [Description("Not a valid LOGO command.")]
        InvalidCommand = 0,
        [Description("Valid.")]
        Valid=1,
        [Description("Argument {0} is not a integer value.")]
        NotAnInteger = 2,
        [Description("Argument {0} is missing.")]
        MissingArguments = 3,
        [Description("Function contains no code.")]
        FunctionHasNoCommands = 4,
        [Description("The looping command has no commands to execute.")]
        LoopHasNoCommands = 5,
        [Description("While command has invalid operator.")]
        WhileInvalidOperator = 6,
        [Description("Argument {0} is not an allowable value.")]
        InvalidArgumentPattern = 7,

  }
}