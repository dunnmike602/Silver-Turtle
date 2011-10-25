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

namespace TurtleGraphics.Constants
{
    public static class GlobalConstants
    {
        public const char Quote = '"';
        public const string BlockLeftDelimiter = "[";
        public const string BlockRightDelimiter = "]";

        #region Turtle Graphics Commands

        public const string SelectPenCommandText = "PENCOL";
        public const string PenDownCommandText = "PENDOWN";
        public const string PenUpCommandText = "PENUP";
        public const string BackColCommandText = "BACKCOL";
        public const string BackwardCommandText = "BACKWARD";
        public const string CenterTurtleCommandText = "HOME";
        public const string ClearScreenCommandText = "CLEARSCREEN";
        public const string DrawCircleCommandText = "CIRCLE";
        public const string DrawTriangleCommandText = "TRIANGLE";
        public const string DrawSquareCommandText = "SQUARE";
        public const string ForwardCommandText = "FORWARD";
        public const string MoveCommandText = "MOVE";
        public const string LeftCommandText = "LEFT";
        public const string RightCommandText = "RIGHT";
        public const string RotateCommandText = "SETHEADING";
        public const string LabelCommandText = "LABEL";
        public const string PenWidthCommandText = "PENWIDTH";
        public const string PrintCommandText = "PRINT";
        public const string PrintLineCommandText = "PRINTLN";
        public const string SetPosCommandText = "SETPOS";
        public const string SetXCommandText = "SETX";
        public const string SetYCommandText = "SETY";
        public const string WaitCommandText = "WAIT";
        public const string HideTurtleCommandText = "HIDETURTLE";
        public const string ShowTurtleCommandText = "SHOWTURTLE";
        public const string HideAllTurtlesCommandText = "HIDEALLTURTLES";
        public const string ShowAllTurtlesCommandText = "SHOWALLTURTLES";
        public const string DiagnoseCommandText = "DIAGNOSE";
        public const string ChangeTurtleImageCommandText = "CHANGETURTLEIMAGE";
        public const string TurtleCommandText = "TURTLE";
        public const string DeleteTurtleCommandText = "DELETETURTLE";
        public const string ActiveTurtleCommandText = "ACTIVETURTLE";

        #endregion

        #region Control Structure Commands

        public const string SetCommandText = "SET";
        public const string AddCommandText = "ADD";
        public const string SubtractCommandText = "SUBTRACT";
        public const string MultiplyCommandText = "MULTIPLY";
        public const string DivideCommandText = "DIVIDE";
        public const string RandomCommandText = "RANDOM";
        public const string SquareRootCommandText = "SQUAREROOT";
        public const string StopCommandText = "STOP";
        public const string IfCommandText = "IF";
        public const string RepeatCommandText = "REPEAT";
        public const string WhileCommandText = "WHILE";
        public const string DoCommandText = "DO";
        public const string ToCommandText = "TO";
        public const string ClearFunctionsCommandText = "CLEARFUNC";
        public const string ClearVariablesCommandText = "CLEARVAR";
        public const string RemarkCommandText = "REMARK";
        public const string SinCommandText = "SIN";
        public const string CosCommandText = "COS";
        public const string TanCommandText = "TAN";
        public const string PowerCommandText = "POWER";

        #endregion

        public const string DiagnoseThreadId = "tid";

        public const int ThreeSixtyDegrees = 360;
        public const int OneEightDegrees = 180;
        public const int NinetyDegrees = 90;

        public const string StartLoopToken = "[";
        public const string EndLoopToken = "]";

        public const string LessThan = "<";
        public const string GreaterThan = ">";
        public const string Equal = "=";
        public const string NotEqual = "!=";
        public const string LessThanOrEqual = "<=";
        public const string GreaterThanOrEqual = ">=";

        public const string LexerPattern = @"-{0,1}\d*\.{1}\d*|\[{1}|\]{1}|-{1}\d*|<=|>=|<|=|>|!=|""[^""]*""|\w*";

        public const string PatternThatMatchNumber = @"-{0,1}\d*\.{1}\d*|\d*|-{1}\d*";
        public const string PatternThatMatchVariableOrNumber = @"-{0,1}\d*\.{1}\d*|\[{1}|\]{1}|-{1}\d*|\w*";
        public const string PatternThatMatchVariable = @"[a-zA-Z]{1}\w*";
        public const string PatternThatMatchesComparisonOperator = "<=|>=|>|<|!=|=";
        public const string PatternThatMatchesString = @"""[^""]*""";
        public const string PatternThatMatchTextOrVariableOrNumber = @"-{0,1}\d*\.{1}\d*|\[{1}|\]{1}|-{1}\d*|""[^""]*""|\w*";
        public const string PatternThatMatchText = @"""[^""]*""";
     }
}
