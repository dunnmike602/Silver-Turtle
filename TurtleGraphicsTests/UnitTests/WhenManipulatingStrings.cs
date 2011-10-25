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
using TurtleGraphics.Helpers;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenManipulatingStrings
    {
        private const string TrimmedString = "TEST";

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
        public void CanStripQuotesFromString()
        {
            // Arrange
            const string quotedString = @"""TEST""";

            // Act
            var trimmedString = quotedString.TrimQuotes();

            // Assert
            Assert.AreEqual(trimmedString, TrimmedString);
        }

        [TestMethod]
        public void CanProcessStringWithoutQuotes()
        {
            // Arrange
            const string quotedString = @"TEST";

            // Act
            var trimmedString = quotedString.TrimQuotes();

            // Assert
            Assert.AreEqual(trimmedString, TrimmedString);
        }
    }
}
