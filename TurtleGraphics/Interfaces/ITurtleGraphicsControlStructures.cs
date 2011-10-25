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
using TurtleGraphics.Constants;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;

namespace TurtleGraphics.Interfaces
{
    public interface ITurtleGraphicsControlStructures
    {
        [TurtleGraphics(CommandText = "REPEAT", Alias = "RP", Type = typeof(TurtleGraphicsRepeatCommand),
            Description = "Repeat the following command block the specified number of time, Alias: {0}, Usage: {1}",
            Usage = "REPEAT 10 [ ]")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution=true, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Repeat();

        [TurtleGraphics(CommandText = "STOP", Alias = "STOP", Type = typeof(TurtleGraphicsTerminationCommand),
          Description = "Stops Exection or exits a function: {0}, Usage: {1}", Usage = "STOP")]
        void Stop();

        [TurtleGraphics(CommandText = "REMARK", Alias = "REM", Type = typeof(TurtleGraphicsCommand),
          Description = "Allows comments to be specified, Alias: {0}, Usage: {1}", Usage = @"REM ""THIS IS A COMMENT""")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText)]
        void Remark(string text);

        [TurtleGraphics(CommandText = "WHILE", Alias = "WH", Type = typeof(TurtleGraphicsWhileCommand),
            Description = "Repeat the following command block until the condition specified is true, Alias: {0}, Usage: {1}",
            Usage = "WHILE X < 10 [ ]")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 1, RegEx = GlobalConstants.PatternThatMatchesComparisonOperator)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true, Order = 2, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void While();

        [TurtleGraphics(CommandText = "IF", Alias = "IF", Type = typeof(TurtleGraphicsIfCommand),
            Description = "Execute the statement when the condition is true, Alias: {0}, Usage: {1}",
            Usage = "IF Y >= 10 [ ]")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 1, RegEx = GlobalConstants.PatternThatMatchesComparisonOperator)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true, Order = 2, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void If();

        [TurtleGraphics(CommandText = "SET", Alias = "SET", Type = typeof(TurtleGraphicsCommand),
            Description = "Set the specified variable to the specified value, Alias: {0}, Usage: {1}", Usage = "SET X 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Set(string variableName, string value);

        [TurtleGraphics(CommandText = "ADD", Alias = "ADD", Type = typeof(TurtleGraphicsCommand),
            Description = "Adds the variable in the second argument to the value in the first, Alias: {0}, Usage: {1}",
            Usage = "ADD Y 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Add(string variableName, string value);
        
        [TurtleGraphics(CommandText = "DIVIDE", Alias = "DIV", Type = typeof(TurtleGraphicsCommand),
            Description = "Divides the variable in the first argument with the value in the second, Alias: {0}, Usage: {1}",
            Usage = "DIVIDE X 20")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Divide(string variableName, string value);

        [TurtleGraphics(CommandText = "MULTIPLY", Alias = "MUL", Type = typeof(TurtleGraphicsCommand),
           Description = "Multiplies the variable in the first argument with the value in the second, Alias: {0}, Usage: {1}",
           Usage = "MULTIPLY Y 20")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Multiply(string variableName, string value);

        [TurtleGraphics(CommandText = "SUBTRACT", Alias = "SUB", Type = typeof(TurtleGraphicsCommand),
           Description = "Subtract the variable in the second argument with the value in the first, Alias: {0}, Usage: {1}",
           Usage = "SUBTRACT X 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Subtract(string variableName, string value);

        [TurtleGraphics(CommandText = "SQUAREROOT", Alias = "SQRT", Type = typeof(TurtleGraphicsCommand),
          Description = "Calculates the square root of the value in the variable, Alias: {0}, Usage: {1}", 
          Usage = "SQUAREROOT X 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0,
            RegEx = GlobalConstants.PatternThatMatchVariable)]
        void SquareRoot(string variableName);

        [TurtleGraphics(CommandText = "SIN", Alias = "SIN", Type = typeof(TurtleGraphicsCommand),
         Description = "Calculates the sine of the value in the variable which needs to be specified in degrees, Alias: {0}, Usage: {1}",
         Usage = "SIN X 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0,
            RegEx = GlobalConstants.PatternThatMatchVariable)]
        void Sin(string variableName);

        [TurtleGraphics(CommandText = "TAN", Alias = "TAN", Type = typeof(TurtleGraphicsCommand),
         Description = "Calculates the tangent of the value in the variable which needs to be specified in degrees, Alias: {0}, Usage: {1}",
         Usage = "TAN X 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0,
            RegEx = GlobalConstants.PatternThatMatchVariable)]
        void Tan(string variableName);

        [TurtleGraphics(CommandText = "COS", Alias = "COS", Type = typeof(TurtleGraphicsCommand),
         Description = "Calculates the cosine of the value in the variable which needs to be specified in degrees, Alias: {0}, Usage: {1}",
         Usage = "COS X 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0,
            RegEx = GlobalConstants.PatternThatMatchVariable)]
        void Cos(string variableName);

        [TurtleGraphics(CommandText = "POWER", Alias = "POW", Type = typeof(TurtleGraphicsCommand),
          Description = "Raises the value in the exponent parameter to the specified power, Alias: {0}, Usage: {1}", Usage = "POWER X 2")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0,
            RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true, Order = 1, 
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Power(string variableName, string exponent);

        [TurtleGraphics(CommandText = "RANDOM", Alias = "RND", Type = typeof(TurtleGraphicsCommand),
         Description = "Generates a Random number between 0 and specified number, Alias: {0}, Usage: {1}", Usage = "RANDOM X 10")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Random(string variableName, string value);

        [TurtleGraphics(CommandText = "CLEARVAR", Alias = "CLRV", Type = typeof(TurtleGraphicsCommand),
          Description = "Clears the value of all variables, Alias: {0}, Usage: {1}", Usage = "CLEARVAR")]
        void ClearVariables();

        [TurtleGraphics(CommandText = "CLEARFUNC", Alias = "CLRF", Type = typeof(TurtleGraphicsCommand),
          Description = "Clears the value of all variables, Alias: {0}, Usage: {1}", Usage = "CLEARFUNC")]
        void ClearFunctions();

        [TurtleGraphics(CommandText = "TO", Alias = "TO", Type = typeof(TurtleGraphicsFunctionCommand),
          Description = "Declares a function which is a reusable chunk of code, Alias: {0}, Usage: {1}",
          Usage = "DO <FUNCNAME> ? []")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariable)]
        void To();

        [TurtleGraphics(CommandText = "DO", Alias = "DO", Type = typeof(TurtleGraphicsDoFunctionCommand),
        Description = "Executes a function, Alias: {0}, Usage: {1}", Usage = "DO <FUNCNAME> ? []")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false, Order = 0, RegEx = GlobalConstants.PatternThatMatchVariable)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Float, AllowVariableSubstitution = true, Order = 1, RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Do(string functionName, params float[] paramList);

        void SetCommandProcessor(ITurtleGraphicsExecutionEngine commandProcessor);
    }
}