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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurtleGraphics;
using TurtleGraphics.Constants;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenExecutingTurtleGraphicsBlockCommand
    {
        private const string VariablePattern = "X";

        [TestMethod]
        public void CanValidateValidCommand()
        {
            // Arrange
            var turtleGraphicsBlockCommand = new TurtleGraphicsBlockCommand
            {
                Commands = new List<TurtleGraphicsCommand> { new TurtleGraphicsCommand() },
                Attribute = new TurtleGraphicsAttribute(),
                ArgumentAttributes =
                    new List<TurtleGraphicsArgumentAttribute>
                                                        {
                                                            new TurtleGraphicsArgumentAttribute
                                                                {
                                                                    ArgumentType = DataTypes.String,
                                                                    RegEx = GlobalConstants.PatternThatMatchVariable
                                                                }
                                                        },
                ArgumentValues = new List<string> { VariablePattern }
            };

            // Act
            turtleGraphicsBlockCommand.ValidateCommand();

            // Assert 
            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsBlockCommand.Status);
        }

        [TestMethod]
        public void CanValidateInvalidValidCommand()
        {
            // Arrange
            var turtleGraphicsBlockCommand = new TurtleGraphicsBlockCommand
            {
                Attribute = new TurtleGraphicsAttribute(),
                ArgumentAttributes =
                    new List<TurtleGraphicsArgumentAttribute>
                                                        {
                                                            new TurtleGraphicsArgumentAttribute
                                                                {
                                                                    ArgumentType = DataTypes.String,
                                                                    RegEx = GlobalConstants.PatternThatMatchVariable
                                                                }
                                                        },
                ArgumentValues = new List<string> { VariablePattern }
            };

            // Act
            turtleGraphicsBlockCommand.ValidateCommand();

            // Assert 
            Assert.AreEqual(TurtleGraphicsCommandStatus.LoopHasNoCommands, turtleGraphicsBlockCommand.Status);
        }
    }
}
