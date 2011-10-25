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
using Rhino.Mocks;
using TurtleGraphics;
using TurtleGraphics.Constants;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;
using TurtleGraphics.Interfaces;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenExecutingTurtleGraphicsCommand
    {
        private const string NotAValidLogoCommand = "Not a valid LOGO command.";
        private const string ArgumentMissing = "Argument 1 is missing.";
        private const string ArgumentNotAnInteger = "Argument 2 is not a integer value.";
        private const string InvalidVariablePattern = "*";
        private const string ValidVariablePattern = "X";
        private const string ArgumentInvalidPattern = "Argument 1 is not an allowable value.";

        [TestMethod]
        public void CanGetTypedValueOfVariableInParameter()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
            {
                Attribute = new TurtleGraphicsAttribute(),
                ArgumentAttributes =
                    new List<TurtleGraphicsArgumentAttribute>
                                                        {
                                                            new TurtleGraphicsArgumentAttribute
                                                                {
                                                                    ArgumentType = DataTypes.String,
                                                                    AllowVariableSubstitution = true,
                                                                    RegEx = GlobalConstants.PatternThatMatchVariable
                                                                }
                                                        },
                ArgumentValues = new List<string> { ValidVariablePattern }
            };

            // Act
            var value = turtleGraphicsCommand.GetTypedArgumentValue(0);

            // Assert 
            Assert.AreEqual("0", value);
        }

        [TestMethod]
        public void CanValidateArgumentPattern()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
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
                                                ArgumentValues = new List<string> {InvalidVariablePattern}
                                            };

            // Act
            turtleGraphicsCommand.ValidateCommand();

            // Assert 
            Assert.AreEqual(turtleGraphicsCommand.Status, TurtleGraphicsCommandStatus.InvalidArgumentPattern);
            Assert.AreEqual(turtleGraphicsCommand.ErrorMessage, ArgumentInvalidPattern);
        }

        [TestMethod]
        public void CanValidateNoIntegerArgument()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
                                            {
                                                Attribute = new TurtleGraphicsAttribute(),
                                                ArgumentAttributes =
                                                    new List<TurtleGraphicsArgumentAttribute>
                                                        {
                                                            new TurtleGraphicsArgumentAttribute
                                                                {ArgumentType = DataTypes.Integer},
                                                            new TurtleGraphicsArgumentAttribute
                                                                {ArgumentType = DataTypes.Integer}
                                                        },
                                                        ArgumentValues = new List<string>{"1", "NAN"}
                                            };

            // Act
            turtleGraphicsCommand.ValidateCommand();

            // Assert 
            Assert.AreEqual(turtleGraphicsCommand.Status, TurtleGraphicsCommandStatus.NotAnInteger);
            Assert.AreEqual(turtleGraphicsCommand.ErrorMessage, ArgumentNotAnInteger);
        }

        [TestMethod]
        public void CanValidateMissingArgument()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
                                            {
                                                Attribute = new TurtleGraphicsAttribute(),
                                                ArgumentAttributes =
                                                    new List<TurtleGraphicsArgumentAttribute>
                                                        {
                                                            new TurtleGraphicsArgumentAttribute()
                                                        }
                                            };

            // Act
            turtleGraphicsCommand.ValidateCommand();
          
            // Assert 
            Assert.AreEqual(turtleGraphicsCommand.Status, TurtleGraphicsCommandStatus.MissingArguments);
            Assert.AreEqual(turtleGraphicsCommand.ErrorMessage, ArgumentMissing);
        }

        [TestMethod]
        public void CanValidateMissingAttributes()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand();

            // Act
            turtleGraphicsCommand.ValidateCommand();

            // Assert 
            Assert.AreEqual(turtleGraphicsCommand.Status, TurtleGraphicsCommandStatus.InvalidCommand);
            Assert.AreEqual(turtleGraphicsCommand.ErrorMessage, NotAValidLogoCommand);
        }

        [TestMethod]
        public void CanGetErrorMessages()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
            {
                ProgramText = null,
                Commands = new List<TurtleGraphicsCommand>()
            };

            turtleGraphicsCommand.Commands.Add(new TurtleGraphicsCommand());

            // Act
            var errorMessages = turtleGraphicsCommand.GetErrorMessages().Trim();

            // Assert 
            Assert.AreEqual(errorMessages, string.Empty);
        }

        [TestMethod]
        public void CanCountInvalidCommand()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
            {
                ProgramText = null,
                Commands = new List<TurtleGraphicsCommand>()
            };

            turtleGraphicsCommand.Commands.Add(new TurtleGraphicsCommand());

            // Act
            var invalidCommandCount = turtleGraphicsCommand.CountInValidCommands();

            // Assert 
            Assert.AreEqual(invalidCommandCount, 2);
        }


        [TestMethod]
        public void CanCancelExecutionOnInnerCommand()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand
                                            {
                                                ProgramText = null,
                                                Commands = new List<TurtleGraphicsCommand>()
                                            };

            turtleGraphicsCommand.Commands.Add(new TurtleGraphicsCommand());
            turtleGraphicsCommand.Commands.Add(new TurtleGraphicsCommand());

            var cancellerMock = MockRepository.GenerateMock<ICanceller>();
            cancellerMock.Expect(m => m.ShouldCancel()).Return(false).Repeat.Once();
            cancellerMock.Expect(m => m.ShouldCancel()).Return(true).Repeat.Once();

            // Act
            var result = turtleGraphicsCommand.Execute(cancellerMock);

            // Assert 
            Assert.AreEqual(false, result);
            cancellerMock.VerifyAllExpectations();
        }

        [TestMethod]
        public void CanCancelExecution()
        {
            // Arrange
            var turtleGraphicsCommand = new TurtleGraphicsCommand { ProgramText = null };
            var cancellerMock = MockRepository.GenerateMock<ICanceller>();
            cancellerMock.Expect(m => m.ShouldCancel()).Return(true).Repeat.Once();

            // Act
            var result = turtleGraphicsCommand.Execute(cancellerMock);

            // Assert 
            Assert.AreEqual(false, result);
            cancellerMock.VerifyAllExpectations();
        }
    }
}
