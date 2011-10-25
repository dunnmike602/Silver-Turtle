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
using TurtleGraphics.Delegates;
using TurtleGraphics.Enums;
using Rhino.Mocks;
using TurtleGraphics.Interfaces;
using TurtleGraphics.Constants;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenExecutingControlStructureCommandLines : WhenExecutingCommandLinesBase
    {
        private const string StringArgument = "X";
        private const string FloatArgument = "1.0";

        private static ITurtleGraphicsControlStructures _turtleGraphicsControlStructuresMock;

        private static List<TurtleGraphicsCommand> CanExecuteCommand(List<string> args, ITurtleGraphicsControlStructures
            turtleGraphicsControlStructuresMock, TurtleGraphicsAttribute turtleGraphicsAttribute,
            List<TurtleGraphicsArgumentAttribute> argumentAttributes, TurtleGraphicsCommand turtleGraphicsCommand,
            string implementingFunctionName, ExecutionEngineStatusChangedEventHandler stateChangedHandler = null,
            bool hasCompletedWithoutErrors = true)
        {
            // Arrange
            var tokens = new List<string> { turtleGraphicsAttribute.CommandText};

            if (args != null)
            {
                tokens.AddRange(args);
            }

            var textParserMock = GetTextParserMock(tokens);

            turtleGraphicsCommand.Attribute = turtleGraphicsAttribute;
            turtleGraphicsCommand.ArgumentAttributes = argumentAttributes;
            turtleGraphicsCommand.CommandText = turtleGraphicsAttribute.CommandText;
            turtleGraphicsCommand.ArgumentValues = args;
            turtleGraphicsCommand.Status = TurtleGraphicsCommandStatus.Valid;
            turtleGraphicsCommand.ExecutionContext = turtleGraphicsControlStructuresMock;
            turtleGraphicsCommand.ImplementingFunctionName = implementingFunctionName;

            var turtleGraphicsSyntaxAnalyser = GetSyntaxAnalyserMock(turtleGraphicsCommand);
            var executionEngine = GetExecutionEngineInstance(textParserMock, turtleGraphicsSyntaxAnalyser);

            if (stateChangedHandler != null)
            {
                executionEngine.StatusChanged += stateChangedHandler;
            }

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
            var turtleGraphicsCommandReturned = executionEngine.ExecuteCommandLine(commandLine, false, true);

            // Assert
            textParserMock.VerifyAllExpectations();
            turtleGraphicsControlStructuresMock.VerifyAllExpectations();
            Assert.AreEqual(hasCompletedWithoutErrors, executionEngine.HasExecutedWithoutErrors());
 
            return turtleGraphicsCommandReturned;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _turtleGraphicsControlStructuresMock.BackToRecord();
            _turtleGraphicsControlStructuresMock.Replay();

            TurtleGraphicsLexicalAnalyser.BackToRecord();
            TurtleGraphicsLexicalAnalyser.Replay();

            TurtleGraphicsSyntaxAnalyser.BackToRecord();
            TurtleGraphicsSyntaxAnalyser.Replay();
        }

        [ClassInitialize]
        public static void InitializeClass(TestContext ctx)
        {
            _turtleGraphicsControlStructuresMock = MockRepository.GenerateMock<ITurtleGraphicsControlStructures>();
            TurtleGraphicsLexicalAnalyser = MockRepository.GenerateMock<ITurtleGraphicsLexicalAnalyser>();
            TurtleGraphicsSyntaxAnalyser = MockRepository.GenerateMock<ITurtleGraphicsSyntaxAnalyser>();
        }
       
        [TestMethod]
        public void CanExecuteSetCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Set(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new []{ DataTypes.String, DataTypes.String});

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument, StringArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.SetCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Set");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void StatusChangesWhenCommandExecutes()
        {
            var programCount = 0;
            var eventExecuted = false;
 
            _turtleGraphicsControlStructuresMock.Expect(m => m.Add(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.String });

            CanExecuteCommand(new List<string> {StringArgument, StringArgument},
                              _turtleGraphicsControlStructuresMock,
                              GetTurtleGraphicsAttribute(
                                  GlobalConstants.AddCommandText),
                              argumentAttributes, new TurtleGraphicsCommand(),
                              "Add",
                              (o, e) =>
                                  {
                                      eventExecuted = true;
                                      programCount += e.ProgramCountIncreased ? 1 : -1;
                                  });

            Assert.AreEqual(programCount, 0);
            Assert.IsTrue(eventExecuted);
        }
        [TestMethod]
        public void CanExecuteAddCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Add(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument, StringArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.AddCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Add");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteRemarkCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Remark(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String});

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> { StringArgument },
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.RemarkCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Remark");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteSubtractCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Subtract(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument, StringArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.SubtractCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Subtract");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }


        [TestMethod]
        public void CanExecuteDivideCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Divide(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument, StringArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.DivideCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Divide");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteMultiplyCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Multiply(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument, StringArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.MultiplyCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Multiply");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteRandomCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Random(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument, StringArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.RandomCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Random");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteSquareRootCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.SquareRoot(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String});

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.SquareRootCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "SquareRoot");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteSineCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Sin(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> { StringArgument },
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.SinCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Sin");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }
        
        [TestMethod]
        public void CanExecuteCosineCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Cos(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> { StringArgument },
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.CosCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Cos");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteTangentCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Tan(null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(1, new[] { DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> { StringArgument },
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.TanCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Tan");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecutePowerCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Power(null,null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.String });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> { StringArgument, StringArgument },
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.PowerCommandText),
                                                                  argumentAttributes, new TurtleGraphicsCommand(),
                                                                  "Power");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteClearFunctionsCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.ClearFunctions()).IgnoreArguments().Repeat.Once();

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string>(),
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.ClearFunctionsCommandText),
                                                                  new List<TurtleGraphicsArgumentAttribute>(),
                                                                  new TurtleGraphicsCommand(),
                                                                  "ClearFunctions");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteClearVariableCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.ClearVariables()).IgnoreArguments().Repeat.Once();

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string>(),
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.ClearVariablesCommandText),
                                                                  new List<TurtleGraphicsArgumentAttribute>(),
                                                                  new TurtleGraphicsCommand(),
                                                                  "ClearVariables");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteStopCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Stop()).IgnoreArguments().Repeat.Never();

            var turtleGraphicsCommandReturned = CanExecuteCommand(null,
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.StopCommandText,
                                                                      typeof(TurtleGraphicsTerminationCommand)), null,
                                                                  new TurtleGraphicsTerminationCommand(),
                                                                  "Stop");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteIfCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.If()).IgnoreArguments().Repeat.Never();

            var turtleGraphicsCommandReturned = CanExecuteCommand(null,
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.IfCommandText, typeof(TurtleGraphicsIfCommand)),
                                                                  null, new TurtleGraphicsIfCommand(),
                                                                  "If");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteRepeatCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Repeat()).IgnoreArguments().Repeat.Never();

            var turtleGraphicsCommandReturned = CanExecuteCommand(null,
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.RepeatCommandText,
                                                                      typeof (TurtleGraphicsRepeatCommand)), null,
                                                                  new TurtleGraphicsRepeatCommand(),
                                                                  "Repeat");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteWhileCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.While()).IgnoreArguments().Repeat.Never();

            var turtleGraphicsCommandReturned = CanExecuteCommand(null,
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.WhileCommandText,
                                                                      typeof(TurtleGraphicsWhileCommand)), null,
                                                                  new TurtleGraphicsWhileCommand(),
                                                                  "While");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteDoCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.Do(null, null)).IgnoreArguments().Repeat.Once();

            var argumentAttributes = GetArgumentAttributesHelper(2, new[] { DataTypes.String, DataTypes.Float });

            var turtleGraphicsCommandReturned = CanExecuteCommand(new List<string> {StringArgument, FloatArgument},
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.DoCommandText,
                                                                      typeof(TurtleGraphicsDoFunctionCommand)), argumentAttributes,
                                                                  new TurtleGraphicsDoFunctionCommand(),
                                                                  "Do");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }

        [TestMethod]
        public void CanExecuteToCommand()
        {
            _turtleGraphicsControlStructuresMock.Expect(m => m.To()).IgnoreArguments().Repeat.Never();

            var turtleGraphicsCommandReturned = CanExecuteCommand(null,
                                                                  _turtleGraphicsControlStructuresMock,
                                                                  GetTurtleGraphicsAttribute(
                                                                      GlobalConstants.ToCommandText,
                                                                      typeof(TurtleGraphicsFunctionCommand)), null,
                                                                  new TurtleGraphicsFunctionCommand(),
                                                                  "To");

            Assert.AreEqual(TurtleGraphicsCommandStatus.Valid, turtleGraphicsCommandReturned[0].Status);
        }
    }
}
