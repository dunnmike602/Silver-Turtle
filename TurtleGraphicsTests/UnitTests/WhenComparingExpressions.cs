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
using TurtleGraphics.Constants;
using TurtleGraphics.Maths;

namespace TurtleGraphicsTests.UnitTests
{
    [TestClass]
    public class WhenComparingExpressions
    {
        [TestMethod]
        public void CanProcessLessThanTrue()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(1, GlobalConstants.LessThan, 2);

            // Assert 
            Assert.AreEqual(conditionIsTrue, true);
        }

        [TestMethod]
        public void CanProcessLessThanFalse()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(2, GlobalConstants.LessThan, 1);

            // Assert 
            Assert.AreEqual(conditionIsTrue, false);
        }

        [TestMethod]
        public void CanProcessGreaterThanTrue()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(2, GlobalConstants.GreaterThan, 1);

            // Assert 
            Assert.AreEqual(conditionIsTrue, true);
        }

        [TestMethod]
        public void CanProcessGreaterThanFalse()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(1, GlobalConstants.GreaterThan, 2);

            // Assert 
            Assert.AreEqual(conditionIsTrue, false);
        }

        [TestMethod]
        public void CanProcessEqualTrue()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(1, GlobalConstants.Equal, 1);

            // Assert 
            Assert.AreEqual(conditionIsTrue, true);
        }

        [TestMethod]
        public void CanProcessEqualFalse()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(1, GlobalConstants.Equal, 2);

            // Assert 
            Assert.AreEqual(conditionIsTrue, false);
        }

        [TestMethod]
        public void CanProcessNotEqualTrue()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(2, GlobalConstants.NotEqual, 1);

            // Assert 
            Assert.AreEqual(conditionIsTrue, true);
        }

        [TestMethod]
        public void CanProcessNotEqualFalse()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(1, GlobalConstants.NotEqual, 1);

            // Assert 
            Assert.AreEqual(conditionIsTrue, false);
        }

        public void CanProcessLessThanOrEqualTrue()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(1, GlobalConstants.LessThanOrEqual, 2);

            // Assert 
            Assert.AreEqual(conditionIsTrue, true);
        }

        [TestMethod]
        public void CanProcessLessThanOrEqualFalse()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(2, GlobalConstants.LessThanOrEqual, 1);

            // Assert 
            Assert.AreEqual(conditionIsTrue, false);
        }

        [TestMethod]
        public void CanProcessGreaterThanOrEqualTrue()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(2, GlobalConstants.GreaterThanOrEqual, 1);

            // Assert 
            Assert.AreEqual(conditionIsTrue, true);
        }

        [TestMethod]
        public void CanProcessGreaterThanOrEqualFalse()
        {
            // Arrange and Act
            var conditionIsTrue = ExpressionComparerHelper.CheckCondition(1, GlobalConstants.GreaterThanOrEqual, 2);

            // Assert 
            Assert.AreEqual(conditionIsTrue, false);
        }
    }
}
