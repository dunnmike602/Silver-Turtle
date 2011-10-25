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
using TurtleGraphics.Interfaces;
using TurtleGraphics.Maths;

namespace TurtleGraphics
{
    public class TurtleGraphicsIfCommand : TurtleGraphicsBlockCommand
    {
        public int Lhs
        {
            get { return Convert.ToInt32(GetTypedArgumentValue(0)); }
        }

        public string Operator
        {
            get { return GetTypedArgumentValue(1).ToString(); }
        }

        public int Rhs
        {
            get { return Convert.ToInt32(GetTypedArgumentValue(2)); }
        }

        /// <summary>
        /// If commands are conditionally executed.
        /// </summary>
        public override bool Execute(ICanceller canceller)
        {
            if (canceller.ShouldCancel())
            {
                return false;
            }

            var shouldContinue = true;

            if (ExpressionComparerHelper.CheckCondition(Lhs, Operator, Rhs))
            {
                shouldContinue = base.Execute(canceller);
            }

            return shouldContinue;
        }
    }
}