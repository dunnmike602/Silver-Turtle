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
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;
using Rhino.Mocks;
using TurtleGraphics.Interfaces;
using TurtleGraphics.Constants;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenExecutingTurtleGraphicsCommandLines : WhenExecutingCommandLinesBase
    {
        private const string InvalidCommand = "XXXX";
        private const string IntegerArgument = "1";
        private const string DefaultTurtleName = "default";

        private static ITurtleGraphicsCommands _turtleGraphicsCommands;
   
        private static void CanExecuteCommand(List<string> args, ITurtleGraphicsCommands
            turtleGraphicsSystemMock, TurtleGraphicsAttribute turtleGraphicsAttribute,
            List<TurtleGraphicsArgumentAttribute> argumentAttributes, string implementingFunctionName)
        {
            // Arrange
            var tokens = new List<string> { turtleGraphicsAttribute.CommandText};

            if (args != null)
            {
                tokens.AddRange(args);
            }

            var textParserMock = GetTextParserMock(tokens);

            var turtleGraphicsCommand = new TurtleGraphicsCommand
            {
                Attribute = turtleGraphicsAttribute,
                ArgumentAttributes = argumentAttributes,
                CommandText = turtleGraphicsAttribute.CommandText,
                ArgumentValues = args,
                Status = TurtleGraphicsCommandStatus.Valid,
                ExecutionContext = turtleGraphicsSystemMock,
                ImplementingFunctionName = implementingFunctionName,
            };

            var turtleGraphicsSyntaxAnalyser = GetSyntaxAnalyserMock(turtleGraphicsCommand);
 
            var commandProcessor = GetExecutionEngineInstance(textParserMock, turtleGraphicsSyntaxAnalyser);

            var commandLine = turtleGraphicsAttribute.CommandText + " ";

            if (args != null)
            {
                foreach (var arg in args)
                {
                    commandLine += arg;
                    commandLine += " ";
                }
            }

            // Act
            var turtleGraphicsCommandReturned = commandProcessor.ExecuteCommandLine(commandLine, false, true);

            // Assert
            textParserMock.VerifyAllExpectations();
            turtleGraphicsSystemMock.VerifyAllExpectations();
            turtleGraphicsSyntaxAnalyser.VerifyAllExpectations();
            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }
    
        [TestInitialize]
        public void TestInitialize()
        {
            TurtleGraphicsLexicalAnalyser.BackToRecord();
            TurtleGraphicsLexicalAnalyser.Replay();

            _turtleGraphicsCommands.BackToRecord();
            _turtleGraphicsCommands.Replay();

            TurtleGraphicsSyntaxAnalyser.BackToRecord();
            TurtleGraphicsSyntaxAnalyser.Replay();
        }

        [ClassInitialize]
        public static void InitializeClass(TestContext ctx)
        {
            TurtleGraphicsLexicalAnalyser = MockRepository.GenerateMock<ITurtleGraphicsLexicalAnalyser>();
            _turtleGraphicsCommands = MockRepository.GenerateMock<ITurtleGraphicsCommands>();
            TurtleGraphicsSyntaxAnalyser = MockRepository.GenerateMock<ITurtleGraphicsSyntaxAnalyser>();
        }

        [TestMethod]
        public void CanHandleInvalidCommand()
        {
            // Arrange
            var tokens = new List<string> { InvalidCommand };

            var textParserMock = GetTextParserMock(tokens);

            var turtleGraphicsCommand = new TurtleGraphicsCommand
            {
                CommandText = InvalidCommand,
                Status = TurtleGraphicsCommandStatus.InvalidCommand
            };

            var turtleGraphicsSyntaxAnalyser = GetSyntaxAnalyserMock(turtleGraphicsCommand);
            var commandProcessor = GetExecutionEngineInstance(textParserMock, turtleGraphicsSyntaxAnalyser);

            // Act
            var turtleGraphicsCommandReturned = commandProcessor.ExecuteCommandLine(InvalidCommand,
                false);

            // Assert
            textParserMock.VerifyAllExpectations();
 
            Assert.AreEqual(TurtleGraphicsCommandStatus.InvalidCommand, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteBackwardCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Backward(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.BackwardCommandText), argumentAttributes,
                              "Backward");
        }

        [TestMethod]
        public void CanExecuteDiagnoseCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Diagnose(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            CanExecuteCommand(new List<string> { string.Empty }, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.DiagnoseCommandText), argumentAttributes,
                              "Diagnose");
        }

        [TestMethod]
        public void CanExecuteLabelCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Label(string.Empty, 1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.Integer });

            CanExecuteCommand(new List<string> {@"""""", IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.LabelCommandText), argumentAttributes, "Label");
        }

        [TestMethod]
        public void CanExecuteForwardCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Forward(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.ForwardCommandText), argumentAttributes,
                              "Forward");
        }

        [TestMethod]
        public void CanExecuteMoveCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Move(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.MoveCommandText), argumentAttributes, "Move");
        }

        [TestMethod]
        public void CanExecuteLeftCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Left(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.LeftCommandText), argumentAttributes, "Left");
        }

        [TestMethod]
        public void CanExecutePenWidthCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.PenWidth(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.PenWidthCommandText), argumentAttributes,
                              "PenWidth");
        }

        [TestMethod]
        public void CanExecutePrintCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Print(string.Empty)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            CanExecuteCommand(new List<string> {string.Empty}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.PrintCommandText), argumentAttributes, "Print");
        }

        [TestMethod]
        public void CanExecuteSelectBackColCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.SelectBackGround(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.BackColCommandText), argumentAttributes,
                              "SelectBackGround");
        }

        [TestMethod]
        public void CanExecuteSetXCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.SetX(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.SetXCommandText), argumentAttributes, "SetX");
        }

        [TestMethod]
        public void CanExecuteWaitCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Wait(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.WaitCommandText), argumentAttributes, "Wait");
        }

        [TestMethod]
        public void CanExecuteSetYCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.SetY(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.SetYCommandText), argumentAttributes, "SetY");
        }

        [TestMethod]
        public void CanExecuteSetPosCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.SetPos(1,1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.Integer, DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument, IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.SetPosCommandText), argumentAttributes,
                              "SetPos");
        }

        [TestMethod]
        public void CanExecutePrintLineCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.PrintLine(string.Empty)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            CanExecuteCommand(new List<string> {string.Empty}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.PrintLineCommandText), argumentAttributes,
                              "PrintLine");
        }

        [TestMethod]
        public void CanExecuteRightCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Right(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.RightCommandText), argumentAttributes, "Right");
        }

        [TestMethod]
        public void CanExecuteRotateCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Rotate(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.RotateCommandText), argumentAttributes,
                              "Rotate");
        }

        [TestMethod]
        public void CanExecuteCircleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.DrawCircle(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> { IntegerArgument }, _turtleGraphicsCommands,
                               GetTurtleGraphicsAttribute(GlobalConstants.DrawCircleCommandText), argumentAttributes, "DrawCircle");
        }

        [TestMethod]
        public void CanExecuteTriangleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.DrawTriangle(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.DrawTriangleCommandText), argumentAttributes,
                              "DrawTriangle");
        }


        [TestMethod]
        public void CanExecuteSquareCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.DrawSquare(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> {IntegerArgument}, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.DrawSquareCommandText), argumentAttributes,
                              "DrawSquare");
        }

        [TestMethod]
        public void CanExecuteCentreTurtleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.CenterTurtle()).IgnoreArguments().Repeat.Once();

            CanExecuteCommand(new List<string>(), _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.CenterTurtleCommandText), null, "CenterTurtle");
        }

        [TestMethod]
        public void CanExecuteClearScreenCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.ClearScreen()).IgnoreArguments().Repeat.Once();

            CanExecuteCommand(new List<string>(), _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.ClearScreenCommandText), null, "ClearScreen");
        }

        [TestMethod]
        public void CanExecutePenDownCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.PenDown()).IgnoreArguments().Repeat.Once();

            CanExecuteCommand(new List<string>(), _turtleGraphicsCommands,
                               GetTurtleGraphicsAttribute(GlobalConstants.PenDownCommandText), null, "PenDown");
        }

        [TestMethod]
        public void CanExecutePenUpCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.PenUp()).IgnoreArguments().Repeat.Once();

            CanExecuteCommand(new List<string>(), _turtleGraphicsCommands,
                             GetTurtleGraphicsAttribute(GlobalConstants.PenUpCommandText), null, "PenUp");
        }

        [TestMethod]
        public void CanExecuteHideAllTurtlesCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.HideAllTurtles()).IgnoreArguments().Repeat.Once();

            CanExecuteCommand(new List<string>(),_turtleGraphicsCommands,
                             GetTurtleGraphicsAttribute(GlobalConstants.HideAllTurtlesCommandText), null, "HideAllTurtles");
        }

        [TestMethod]
        public void CanExecuteShowAllTurtlesCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.ShowAllTurtles()).IgnoreArguments().Repeat.Once();

            CanExecuteCommand(new List<string>(), _turtleGraphicsCommands,
                             GetTurtleGraphicsAttribute(GlobalConstants.ShowAllTurtlesCommandText), null, "ShowAllTurtles");
        }

        [TestMethod]
        public void CanExecuteHideTurtleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.HideTurtle(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            CanExecuteCommand(new List<string> { DefaultTurtleName}, _turtleGraphicsCommands,
                             GetTurtleGraphicsAttribute(GlobalConstants.HideTurtleCommandText), argumentAttributes, "HideTurtle");
        }

        [TestMethod]
        public void CanExecuteChangeTurtleImageCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.ChangeTurtleImage(null, 0)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.Integer});

            CanExecuteCommand(new List<string> { string.Empty, IntegerArgument }, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.ActiveTurtleCommandText), argumentAttributes,
                              "ChangeTurtleImage");
        }

        [TestMethod]
        public void CanExecuteSetActiveTurtleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.SetActiveTurtle(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            CanExecuteCommand(new List<string> { string.Empty }, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.DeleteTurtleCommandText), argumentAttributes,
                              "SetActiveTurtle");
        }

        [TestMethod]
        public void CanExecuteDeleteTurtleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.DeleteTurtle(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            CanExecuteCommand(new List<string> { string.Empty }, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.TurtleCommandText), argumentAttributes,
                              "DeleteTurtle");
        }

        [TestMethod]
        public void CanExecuteTurtleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.Turtle(null,0,0,0)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(4, new[] { DataTypes.String, DataTypes.Integer, DataTypes.Integer, DataTypes.Integer });

            CanExecuteCommand(new List<string> { string.Empty, IntegerArgument, IntegerArgument, IntegerArgument }, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.ChangeTurtleImageCommandText), argumentAttributes,
                              "Turtle");
        }

        [TestMethod]
        public void CanExecuteShowTurtleCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.ShowTurtle(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            CanExecuteCommand(new List<string> { DefaultTurtleName }, _turtleGraphicsCommands,
                             GetTurtleGraphicsAttribute(GlobalConstants.ShowTurtleCommandText), argumentAttributes, "ShowTurtle");
        }

        [TestMethod]
        public void CanExecuteSelectPenCommand()
        {
            _turtleGraphicsCommands.Expect(m => m.SelectPen(1)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.Integer });

            CanExecuteCommand(new List<string> { IntegerArgument }, _turtleGraphicsCommands,
                              GetTurtleGraphicsAttribute(GlobalConstants.SelectPenCommandText), argumentAttributes, "SelectPen");

        }
   }
}
