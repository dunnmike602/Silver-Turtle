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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurtleGraphics;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;
using TurtleGraphics.Constants;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenCreatingTurtleGraphicsCommand
    {
        private const string SimpleProgramText = "SET X 1";
        private const string SimpleCommandText = "SET";

        [TestMethod]
        public void CanSetNullProgramText()
        {
            // Arrange and Act
            var turtleGraphicsCommand = new TurtleGraphicsCommand { ProgramText = null };

            // Assert 
            Assert.AreEqual(string.Empty, turtleGraphicsCommand.ProgramText);
        }

        [TestMethod]
        public void CanSetProgramText()
        {
            // Arrange and Act
            var turtleGraphicsCommand = new TurtleGraphicsCommand {ProgramText = SimpleProgramText};

            // Assert 
            Assert.AreEqual(SimpleProgramText, turtleGraphicsCommand.ProgramText);
        }

        [TestMethod]
        public void CanFindCommandOfName()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
                                            {
                                                CommandText = SimpleCommandText,
                                            };

            // Act
            var hasCommand = turtleGraphicsCommand.HasCommandOfName(SimpleCommandText);

            // Assert
            Assert.AreEqual(hasCommand, true);
        }

        [TestMethod]
        public void CanFindCommandOfNameInInnerCommand()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
            {
               Commands = new List<TurtleGraphicsCommand>(),
            };

            turtleGraphicsCommand.Commands.Add(new TurtleGraphicsCommand {CommandText = SimpleCommandText,});
            turtleGraphicsCommand.Commands.Add(new TurtleGraphicsCommand());

            // Act
            var hasCommand = turtleGraphicsCommand.HasCommandOfName(SimpleCommandText);

            // Assert
            Assert.AreEqual(hasCommand, true);
        }

        [TestMethod]
        public void CanSetProgramTextAndNormaliseLineBreaks()
        {
            // Arrange and Act
            var turtleGraphicsCommand = new TurtleGraphicsCommand
                                            {
                                                ProgramText = SimpleProgramText + Environment.NewLine + Environment.NewLine
                                            };

            // Assert 
            Assert.AreEqual(SimpleProgramText + Environment.NewLine, turtleGraphicsCommand.ProgramText);
        }

        [TestMethod]
        public void CanCreateADefaultInstance()
        { 
            // Arrange and Act
            var turtleGraphicsCommand = new TurtleGraphicsCommand();

            // Assert
            Assert.AreEqual(null, turtleGraphicsCommand.CommandText);
            Assert.AreEqual(string.Empty, turtleGraphicsCommand.ProgramText);
            Assert.AreEqual(0, turtleGraphicsCommand.Commands.Count);
            Assert.AreEqual(TurtleGraphicsCommandStatus.InvalidCommand, turtleGraphicsCommand.Status);
            Assert.AreEqual(null, turtleGraphicsCommand.Attribute);
            Assert.AreEqual(null, turtleGraphicsCommand.ImplementingFunctionName);
            Assert.AreEqual(null, turtleGraphicsCommand.ExecutionContext);
           
        }

        [TestMethod]
        public void CanCreateASelectPenInstance()
        {
            // Arrange and Act
            var turtleGraphicsAttribute = new TurtleGraphicsAttribute();
            var turtleGraphicsArgumentAttribute = new List<TurtleGraphicsArgumentAttribute>();
            var argumentValues = new List<string>();

            var turtleGraphicsCommand = new TurtleGraphicsCommand
                                            {
                                                CommandText = GlobalConstants.SelectPenCommandText,
                                                Attribute = turtleGraphicsAttribute,
                                                ArgumentAttributes = turtleGraphicsArgumentAttribute,
                                                ArgumentValues = argumentValues,
                                                Status = TurtleGraphicsCommandStatus.Valid
                                            };

            // Assert
            Assert.AreEqual(GlobalConstants.SelectPenCommandText, turtleGraphicsCommand.CommandText);
            Assert.AreEqual(turtleGraphicsAttribute, turtleGraphicsCommand.Attribute);
            Assert.AreEqual(argumentValues, turtleGraphicsCommand.ArgumentValues);
            Assert.AreEqual(turtleGraphicsArgumentAttribute, turtleGraphicsCommand.ArgumentAttributes);
            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommand.Status);
        }
    }
}
