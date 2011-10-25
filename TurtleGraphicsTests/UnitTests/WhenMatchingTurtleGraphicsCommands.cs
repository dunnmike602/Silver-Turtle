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
using TurtleGraphics.Enums;
using Rhino.Mocks;
using TurtleGraphics.Interfaces;
using TurtleGraphics.Constants;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenMatchingTurtleGraphicsCommands
    {
        private const string InvalidCommand = "XXXX";

        private ITurtleGraphicsCommands _turtleGraphicsSystemStub;
        private ITurtleGraphicsControlStructures _turtleGraphicsControlStructuresSystemStub;
        private List<Tuple<string, string, Type>> _commands;

        [TestInitialize]
        public void MyTestInitialize()
        {
            _turtleGraphicsSystemStub = MockRepository.GenerateStub<ITurtleGraphicsCommands>();
            _turtleGraphicsControlStructuresSystemStub = MockRepository.GenerateStub<ITurtleGraphicsControlStructures>();

            var turtleGraphicsCommand = typeof(TurtleGraphicsCommand);

            _commands = new List<Tuple<string, string, Type>>
                            {
                                new Tuple<string, string, Type>(GlobalConstants.SelectPenCommandText, "SelectPen", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.PenDownCommandText, "PenDown", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.PenUpCommandText, "PenUp", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.BackColCommandText, "SelectBackGround", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.BackwardCommandText, "Backward", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.CenterTurtleCommandText, "CenterTurtle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.ClearScreenCommandText, "ClearScreen", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.DrawCircleCommandText, "DrawCircle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.DrawTriangleCommandText, "DrawTriangle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.DrawSquareCommandText, "DrawSquare", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.ForwardCommandText, "Forward", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.MoveCommandText, "Move", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.LeftCommandText, "Left", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.RightCommandText, "Right", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.RotateCommandText, "Rotate", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.LabelCommandText, "Label", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.PenWidthCommandText, "PenWidth", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.PrintCommandText, "Print", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.PrintLineCommandText, "PrintLine", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.SetPosCommandText, "SetPos", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.SetXCommandText, "SetX", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.SetYCommandText, "SetY", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.WaitCommandText, "Wait", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.HideTurtleCommandText, "HideTurtle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.ShowTurtleCommandText, "ShowTurtle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.HideAllTurtlesCommandText, "HideAllTurtles", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.ShowAllTurtlesCommandText, "ShowAllTurtles", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.DiagnoseCommandText, "Diagnose", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.ChangeTurtleImageCommandText, "ChangeTurtleImage", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.TurtleCommandText, "Turtle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.DeleteTurtleCommandText, "DeleteTurtle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.ActiveTurtleCommandText, "SetActiveTurtle", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.SetCommandText, "Set", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.AddCommandText, "Add", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.SubtractCommandText, "Subtract", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.MultiplyCommandText, "Multiply", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.DivideCommandText, "Divide", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.RandomCommandText, "Random", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.SquareRootCommandText, "SquareRoot", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.StopCommandText, "Stop", typeof (TurtleGraphicsTerminationCommand)),
                                new Tuple<string, string, Type>(GlobalConstants.IfCommandText, "If", typeof (TurtleGraphicsIfCommand)),
                                new Tuple<string, string, Type>(GlobalConstants.RepeatCommandText, "Repeat", typeof (TurtleGraphicsRepeatCommand)),
                                new Tuple<string, string, Type>(GlobalConstants.WhileCommandText, "While", typeof (TurtleGraphicsWhileCommand)),
                                new Tuple<string, string, Type>(GlobalConstants.DoCommandText, "Do", typeof (TurtleGraphicsDoFunctionCommand)),
                                new Tuple<string, string, Type>(GlobalConstants.ToCommandText, "To", typeof (TurtleGraphicsFunctionCommand)),
                                new Tuple<string, string, Type>(GlobalConstants.ClearFunctionsCommandText, "ClearFunctions", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.ClearVariablesCommandText, "ClearVariables", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.RemarkCommandText, "Remark", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.SinCommandText, "Sin", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.CosCommandText, "Cos", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.TanCommandText, "Tan", turtleGraphicsCommand),
                                new Tuple<string, string, Type>(GlobalConstants.PowerCommandText, "Power", turtleGraphicsCommand)
                            };
        }

        private TurtleGraphicsCommand CanMatchACommand(string commandText, string methodName)
        {
            // Arrange
            var turtleGraphicsReflectionMatcher = new TurtleGraphicsReflectionMatcher(_turtleGraphicsSystemStub,
                _turtleGraphicsControlStructuresSystemStub);

            // Act
            var returnedCommand = turtleGraphicsReflectionMatcher.Match(commandText);

            // Assert
            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, returnedCommand.Status);
            Assert.AreEqual(commandText, returnedCommand.CommandText);
            Assert.AreEqual(methodName, returnedCommand.ImplementingFunctionName);

            return returnedCommand;
        }

        [TestMethod]
        public void CanMatchAllValidCommandsInLanguage()
        {
            foreach (var nextCommand in _commands)
            {
                var returnedCommand = CanMatchACommand(nextCommand.Item1, nextCommand.Item2);
                Assert.IsInstanceOfType(returnedCommand, nextCommand.Item3);
            }
        }

        [TestMethod]
        public void SetsCorrectStatusWhenInvalidCommand()
        {
            // Arrange
            var turtleGraphicsCommands = new TurtleGraphicsReflectionMatcher(_turtleGraphicsSystemStub,
                _turtleGraphicsControlStructuresSystemStub);

            // Act
            TurtleGraphicsCommand returnedCommand = turtleGraphicsCommands.Match(InvalidCommand);

            // Assert
            Assert.AreEqual(TurtleGraphicsCommandStatus.InvalidCommand, returnedCommand.Status);
            Assert.IsInstanceOfType(returnedCommand, typeof(TurtleGraphicsCommand));
        }
    }
}
