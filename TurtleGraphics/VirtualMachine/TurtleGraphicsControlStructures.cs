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
using System.Linq;
using TurtleGraphics.Interfaces;
using TurtleGraphics.Maths;

namespace TurtleGraphics.VirtualMachine
{
    public class TurtleGraphicsControlStructures : ITurtleGraphicsControlStructures
    {
        private readonly IVariableHandler<float> _globalVariableHandler;
        private ITurtleGraphicsExecutionEngine _commandProcessor;

        private readonly ITurtleGraphicsFunctionHandler _turtleGraphicsFunctionHandler;
        private readonly Random _randomNumberGenerator = new Random();

        public void SetCommandProcessor(ITurtleGraphicsExecutionEngine commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public TurtleGraphicsControlStructures(IVariableHandler<float> globalVariableHandler,
            ITurtleGraphicsFunctionHandler turtleGraphicsFunctionHandler)
        {
            _globalVariableHandler = globalVariableHandler;
            _turtleGraphicsFunctionHandler = turtleGraphicsFunctionHandler;
        }

        public void Repeat()
        {
            // Never executed, processing handled by syntax analyser
            // and runtime
        }

        public void Stop()
        {
            // Never executed, processing handled by syntax analyser
            // and runtime
        }

        public void Remark(string text)
        {
            // Nothing to execute this command implements a comment
        }

        public void While()
        {
            // Never executed, processing handled by syntax analyser
            // and runtime
        }

        public void If()
        {
            // Never executed, processing handled by syntax analyser
            // and runtime
        }

        public void To()
        {
            // Never executed, processing handled by syntax analyser
            // and runtime
        }

        public void ClearFunctions()
        {
            _turtleGraphicsFunctionHandler.ClearFunctions();
        }

        public void Do(string functionName, params float[] paramList)
        {
            var function =
                _turtleGraphicsFunctionHandler.GetFunctions().Where(
                    m => m.FunctionName.ToLower() == functionName.ToLower()).
                    FirstOrDefault();

            var originalValues = new float[paramList.Length];

            // Mask existing variable values to act as parameter for function call
            for (var i = 0; i < paramList.Length; i++)
            {
                var value = paramList[i];
                originalValues[i] = _globalVariableHandler.Get(function.ArgumentValues[i + 1]);
                _globalVariableHandler.Set(function.ArgumentValues[i + 1], value);
            }

            _commandProcessor.ExecuteCommands(function.Commands);

            for (var i = 0; i < paramList.Length; i++)
            {
                _globalVariableHandler.Set(function.ArgumentValues[i + 1], originalValues[i]);
            }
        }

        public void Set(string variableName, string value)
        {
            var numericValue = MathsHelper.IsNumber(value) ? (float)Convert.ToDouble(value) : 
                _globalVariableHandler.Get(value);

            _globalVariableHandler.Set(variableName, numericValue);
        }

        public void Add(string variableName, string value)
        {
            var numericValue = MathsHelper.IsNumber(value) ? (float)Convert.ToDouble(value) : 
                _globalVariableHandler.Get(value);

            numericValue = _globalVariableHandler.Get(variableName) + numericValue;

            _globalVariableHandler.Set(variableName, numericValue);
        }

        public void Divide(string variableName, string value)
        {
            var numericValue = MathsHelper.IsNumber(value) ? (float)Convert.ToDouble(value) : 
                _globalVariableHandler.Get(value);

            numericValue = _globalVariableHandler.Get(variableName) / numericValue;

            _globalVariableHandler.Set(variableName, numericValue);
        }

        public void Multiply(string variableName, string value)
        {
            var numericValue = MathsHelper.IsNumber(value) ? (float)Convert.ToDouble(value) : 
                _globalVariableHandler.Get(value);

            numericValue = _globalVariableHandler.Get(variableName) * numericValue;

            _globalVariableHandler.Set(variableName, numericValue);
        }

        public void Subtract(string variableName, string value)
        {
            var numericValue = MathsHelper.IsNumber(value) ? (float)Convert.ToDouble(value) : 
                _globalVariableHandler.Get(value);

            numericValue = _globalVariableHandler.Get(variableName) - numericValue;

            _globalVariableHandler.Set(variableName, numericValue);
        }

        public void SquareRoot(string variableName)
        {
            var value = (float)Math.Sqrt(_globalVariableHandler.Get(variableName));

            _globalVariableHandler.Set(variableName, value);
        }

        public void Sin(string variableName)
        {
            var valueInDegrees = MathsHelper.DegreeToRadian(_globalVariableHandler.Get(variableName));
            var value = (float)Math.Sin(valueInDegrees);

            _globalVariableHandler.Set(variableName, value);
        }

        public void Cos(string variableName)
        {
            var valueInDegrees = MathsHelper.DegreeToRadian(_globalVariableHandler.Get(variableName));
            var value = (float)Math.Cos(valueInDegrees);

            _globalVariableHandler.Set(variableName, value);
        }

        public void Tan(string variableName)
        {
            var valueInDegrees = MathsHelper.DegreeToRadian(_globalVariableHandler.Get(variableName));
            var value = (float)Math.Tan(valueInDegrees);

            _globalVariableHandler.Set(variableName, value);
        }

        public void Power(string variableName, string exponent)
        {
            var numericExponentValue = MathsHelper.IsNumber(exponent) ? (float)Convert.ToDouble(exponent) :
              _globalVariableHandler.Get(exponent);

            var value = (float)Math.Pow(_globalVariableHandler.Get(variableName), numericExponentValue);

            _globalVariableHandler.Set(variableName, value);
        }

        public void Random(string variableName, string value)
        {
            var numericValue = MathsHelper.IsNumber(value) ? (float)Convert.ToDouble(value) : 
                _globalVariableHandler.Get(value);

            var randomNumber = (float)(_randomNumberGenerator.NextDouble() * numericValue);

            _globalVariableHandler.Set(variableName, randomNumber);
        }

        public void ClearVariables()
        {
            _globalVariableHandler.Clear();
        }
    }
}