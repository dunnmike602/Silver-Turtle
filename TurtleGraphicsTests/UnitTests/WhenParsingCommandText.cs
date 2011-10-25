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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurtleGraphics;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenParsingCommandText
    {
        private const string CommandWithNoArgs = "P";
        private const string CommandWithStringArgs = "P X";
        private const string CommandWithArgs = "P 1";
        private const string EmptyCommand = "";
        private const string Command = "P";
        private const string Argument = "1";
        private const string ComplexCommand = @"REPEAT 4 [LEFT 10 PRINT ""HELLO WORLD"" REPEAT 10 [RIGHT 10]]";
 
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CanParseCommandWithNoArgs()
        {
            // Arrange
            var textParser = new TurtleGraphicsLexicalAnalyser();

            // Act
            var parsedValue = textParser.Parse(CommandWithNoArgs);

            // Assert
            Assert.AreEqual(CommandWithNoArgs, parsedValue[0]);
            Assert.AreEqual(1, parsedValue.Count);
        }

        [TestMethod]
        public void CanParseComplexCommand()
        {
            // Arrange
            var textParser = new TurtleGraphicsLexicalAnalyser();

            // Act
            var parsedValues = textParser.Parse(ComplexCommand);

            // Assert
            Assert.AreEqual(14, parsedValues.Count);
            Assert.AreEqual("REPEAT", parsedValues[0]);
            Assert.AreEqual("4", parsedValues[1]);
            Assert.AreEqual("[", parsedValues[2]);
            Assert.AreEqual("LEFT", parsedValues[3]);
            Assert.AreEqual("10", parsedValues[4]);
            Assert.AreEqual("PRINT", parsedValues[5]);
            Assert.AreEqual("\"HELLO WORLD\"", parsedValues[6]);
            Assert.AreEqual("REPEAT", parsedValues[7]);
            Assert.AreEqual("10", parsedValues[8]);
            Assert.AreEqual("[", parsedValues[9]);
            Assert.AreEqual("RIGHT", parsedValues[10]);
            Assert.AreEqual("10", parsedValues[11]);
            Assert.AreEqual("]", parsedValues[12]);
            Assert.AreEqual("]", parsedValues[13]);
        }

        [TestMethod]
        public void CanParseCommandWithArgs()
        {
            // Arrange
            var textParser = new TurtleGraphicsLexicalAnalyser();

            // Act
            var parsedValue = textParser.Parse(CommandWithArgs);

            // Assert
            Assert.AreEqual(Command, parsedValue[0]);
            Assert.AreEqual(Argument, parsedValue[1]);
        }

        [TestMethod]
        public void CanParseEmptyCommand()
        {
            // Arrange
            var textParser = new TurtleGraphicsLexicalAnalyser();

            // Act
            var parsedValue = textParser.Parse(EmptyCommand);

            // Assert
            Assert.AreEqual(0, parsedValue.Count);
        }

        [TestMethod]
        public void CanParseCommandWithStringArg()
        {
            // Arrange
            var textParser = new TurtleGraphicsLexicalAnalyser();

            // Act
            var parsedValue = textParser.Parse(CommandWithStringArgs);

            // Assert
            Assert.AreEqual(CommandWithNoArgs, parsedValue[0]);
        }
    }
}
