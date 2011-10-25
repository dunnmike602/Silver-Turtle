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
using TurtleGraphics.Exceptions;
using TurtleGraphics.VirtualMachine;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenHandlingFunctions
    {
        private const string FunctionName = "FUNC";
        private const string FunctionName1 = "FUNC1";
        private const string FunctionProgramText = "TEXT";
        private const string NonExistentFunctionName = "XXXXX";
        private const string FunctionsProgramText =
            "\r\n\r\nREM \"======= User Defined Functions =======\"\r\nTEXT\r\nTEXT\r\n";

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
     
        private static TurtleGraphicsFunctionCommand GetTurtleGraphicsFunctionCommandHelper(string functioName)
        {
            return new TurtleGraphicsFunctionCommand
                       {
                           CommandText = functioName,
                           ArgumentValues = new List<string> { functioName },
                           ProgramText = FunctionProgramText,
                       };
        }

        [TestMethod]
        public void CanCreateInstance()
        {
            // Arrange and act
            var inst = TurtleGraphicsFunctionHandler.Instance;
       
            // Assert
            Assert.IsNotNull(inst);
        }

        [TestMethod]
        public void CanVerifySingletonBehaviour()
        {
            // Arrange and act
            var inst = TurtleGraphicsFunctionHandler.Instance;
            var inst1 = TurtleGraphicsFunctionHandler.Instance;

            // Assert
            Assert.AreSame(inst, inst1);
        }

        [TestMethod]
        public void CanClearFunctions()
        {
            // Arrange
            var functionHandler = TurtleGraphicsFunctionHandler.Instance;

            // Act
            functionHandler.ClearFunctions();

            // Assert
            Assert.AreEqual(functionHandler.GetFunctions().Count, 0);
        }

        [TestMethod]
        public void CanAddFunction()
        {
            // Arrange
            var functionHandler = TurtleGraphicsFunctionHandler.Instance;
            var turtleGraphicsFunctionCommand = GetTurtleGraphicsFunctionCommandHelper(FunctionName);

            // Act
            functionHandler.AddFunction(turtleGraphicsFunctionCommand);

            // Assert
            Assert.AreEqual(functionHandler.GetFunctions().Count, 1);
        }

        [TestMethod]
        public void CanGetFunctionProgramText()
        {
            // Arrange
            var functionHandler = TurtleGraphicsFunctionHandler.Instance;
            var turtleGraphicsFunctionCommand = GetTurtleGraphicsFunctionCommandHelper(FunctionName);
         
            // Act
            functionHandler.AddFunction(turtleGraphicsFunctionCommand);
            var programText = functionHandler.GetFunctionProgramText(FunctionName);

            // Assert
            Assert.AreEqual(programText, FunctionProgramText);
        }

        [TestMethod]
        public void EventFiresWhenFunctionAdded()
        {
            // Arrange
            var hasEventFired = false;
            var functionCount = 0;

            var functionHandler = TurtleGraphicsFunctionHandler.Instance;
            var turtleGraphicsFunctionCommand = GetTurtleGraphicsFunctionCommandHelper(FunctionName);
            functionHandler.FunctionChanged += (o, evt) =>
                                                   {
                                                       hasEventFired = true;
                                                       functionCount = evt.Functions.Count;
                                                   };

            // Act
            functionHandler.AddFunction(turtleGraphicsFunctionCommand);

            // Assert
            Assert.AreEqual(hasEventFired, true);
            Assert.AreEqual(functionCount, 1);
        }

        [TestMethod]
        public void CanGetAllFunctionsProgramText()
        {
            // Arrange
            var functionHandler = TurtleGraphicsFunctionHandler.Instance;
            functionHandler.ClearFunctions();

            var turtleGraphicsFunctionCommand = GetTurtleGraphicsFunctionCommandHelper(FunctionName);
            var turtleGraphicsFunctionCommand1 = GetTurtleGraphicsFunctionCommandHelper(FunctionName1);

            functionHandler.AddFunction(turtleGraphicsFunctionCommand);
            functionHandler.AddFunction(turtleGraphicsFunctionCommand1);
            
            // Act
            var programText = functionHandler.GetAllFunctionProgramText();

            // Assert
            Assert.AreEqual(programText, FunctionsProgramText);
        }

        [TestMethod]
        public void CanGetAllFunctionsProgramTextWhenNoFunctions()
        {
            // Arrange
            var functionHandler = TurtleGraphicsFunctionHandler.Instance;
            functionHandler.ClearFunctions();

            // Act
            var programText = functionHandler.GetAllFunctionProgramText();

            // Assert
            Assert.AreEqual(programText, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(TurtleRuntimeException))]
        public void ThrowsWhenFunctionIsNotFound()
        {
            // Arrange
            var functionHandler = TurtleGraphicsFunctionHandler.Instance;

            // Act
            functionHandler.GetFunctionProgramText(NonExistentFunctionName);
        }

        [TestMethod]
        public void CanAddExistingFunction()
        {
            // Arrange
            var functionHandler = TurtleGraphicsFunctionHandler.Instance;
            var turtleGraphicsFunctionCommand = GetTurtleGraphicsFunctionCommandHelper(FunctionName);

            // Act
            functionHandler.AddFunction(turtleGraphicsFunctionCommand);
            functionHandler.AddFunction(turtleGraphicsFunctionCommand);

            // Assert
            Assert.AreEqual(functionHandler.GetFunctions().Count, 1);
        }
    }
}
