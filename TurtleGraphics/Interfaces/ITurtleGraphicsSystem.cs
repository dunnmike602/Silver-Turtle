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
    public interface ITurtleGraphicsCommands
    {
        [TurtleGraphics(CommandText = "PENCOL", Alias="PCOL", Type = typeof(TurtleGraphicsCommand),
            Description = "Select a Pen Colour specified in the argument, Alias: {0}, Usage: {1}",
            Usage = "PENCOL ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true, 
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void SelectPen(int penColor);

        [TurtleGraphics(CommandText = "HIDEALLTURTLES", Alias = "HAT", Type = typeof(TurtleGraphicsCommand),
          Description = "Hides all defined turtles including the default one and the screen grid, Alias: {0}, Usage: {1}", Usage = "HIDEALLTURTLES")]
        void HideAllTurtles();

        [TurtleGraphics(CommandText = "SHOWALLTURTLES", Alias = "SAT", Type = typeof(TurtleGraphicsCommand),
          Description = "Shows all defined turtles including the default one and the screen grid, Alias: {0}, Usage: {1}", Usage = "HIDEALLTURTLES")]
        void ShowAllTurtles();

        [TurtleGraphics(CommandText = "DIAGNOSE", Alias = "DIAG", Type = typeof(TurtleGraphicsCommand),
          Description = "Writes diagnostic information to the Debug window, Alias: {0}, Usage: {1}", Usage = @"DIAGNOSE ""TID""")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText)]
        void Diagnose(string runtimeProperty);

        [TurtleGraphics(CommandText = "ACTIVETURTLE", Alias = "AT", Type = typeof(TurtleGraphicsCommand),
          Description = "Sets the currently active turtle by name. This turtle must have been defined, drawing will now be done with this turtle, Alias: {0}, Usage: {1}", Usage = @"ACTIVETURTLE ""?""")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText)]
        void SetActiveTurtle(string turtleName);

        [TurtleGraphics(CommandText = "TURTLE", Alias = "TU", Type = typeof(TurtleGraphicsCommand),
         Description = "Creates and initialises a new turtle, Alias: {0}, Usage: {1}", Usage = @"TURTLE ""?"" ? ? ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText, Order = 0)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
           RegEx = GlobalConstants.PatternThatMatchVariableOrNumber, Order = 1)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
           RegEx = GlobalConstants.PatternThatMatchVariableOrNumber, Order = 2)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
           RegEx = GlobalConstants.PatternThatMatchVariableOrNumber, Order = 3)]
        void Turtle(string turtleName, int turtleImageIndex, int startPosX, int startPosY);

        [TurtleGraphics(CommandText = "CHANGETURTLEIMAGE", Alias = "CTI", Type = typeof(TurtleGraphicsCommand),
        Description = "Changes the image of an existing turtle, Alias: {0}, Usage: {1}", Usage = @"CHANGETURTLEIMAGE ""?"" ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText, Order = 0)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
           RegEx = GlobalConstants.PatternThatMatchVariableOrNumber, Order = 1)]
        void ChangeTurtleImage(string turtleName, int turtleImageIndex);

        [TurtleGraphics(CommandText = "HIDETURTLE", Alias = "HT", Type = typeof(TurtleGraphicsCommand),
            Description = "Hides the specified turtle, Alias: {0}, Usage: {1}",
            Usage = @"HIDETURTLE ""?""")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText, Order = 0)]
        void HideTurtle(string turtleName);

        [TurtleGraphics(CommandText = "DELETETURTLE", Alias = "DT", Type = typeof(TurtleGraphicsCommand),
          Description = "Deletes the specified turtle, Alias: {0}, Usage: {1}",
          Usage = @"DELETETURTLE ""?""")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText, Order = 0)]
        void DeleteTurtle(string turtleName);

        [TurtleGraphics(CommandText = "SHOWTURTLE", Alias = "ST", Type = typeof(TurtleGraphicsCommand),
           Description = "Shows the specified turtle, Alias: {0}, Usage: {1}",
           Usage = @"SHOWTURTLE ""MyTurtle""")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchText, Order = 0)]
        void ShowTurtle(string turtleName);

        [TurtleGraphics(CommandText = "BACKCOL", Alias = "BKCOL", Type = typeof(TurtleGraphicsCommand),
          Description = "Select a Background Colour specified in the argument, Alias: {0}, Usage: {1}",
          Usage = "BACKCOL ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void SelectBackGround(int penColor);

        [TurtleGraphics(CommandText = "PENDOWN", Alias = "PD", Type = typeof(TurtleGraphicsCommand),
         Description = "Put the pen down on the drawing surface, Alias: {0}, Usage: {1}",
         Usage = "PENDOWN")]
        void PenDown();

        [TurtleGraphics(CommandText = "PENUP", Alias = "PU", Type = typeof(TurtleGraphicsCommand),
            Description = "Lift the pen from the drawing surface, Alias: {0}, Usage: {1}",
            Usage = "PENUP")]
        void PenUp();

        [TurtleGraphics(CommandText = "CLEARSCREEN", Alias = "CS", Type = typeof(TurtleGraphicsCommand),
            Description = "Reset and clear the screen, Alias: {0}, Usage: {1}",
            Usage = "CLEARSCREEN")]
        void ClearScreen();

        [TurtleGraphics(CommandText = "SQUARE", Alias = "SQ", Type = typeof(TurtleGraphicsCommand),
            Description = "Draw a square of the specified size, Alias: {0}, Usage: {1}",
            Usage = "SQUARE ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void DrawSquare(int length);

        [TurtleGraphics(CommandText = "CIRCLE", Alias = "CIR", Type = typeof(TurtleGraphicsCommand),
            Description = "Draw a circle of the specified size, Alias: {0}, Usage: {1}",
            Usage = "CIRCLE ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void DrawCircle(int diameter);

        [TurtleGraphics(CommandText = "TRIANGLE", Alias = "TRI", Type = typeof(TurtleGraphicsCommand),
            Description = "Draw a triangle of the specified size, Alias: {0}, Usage: {1}",
            Usage = "TRIANGLE ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void DrawTriangle(int length);

        [TurtleGraphics(CommandText = "SETHEADING", Alias = "SETH", Type = typeof(TurtleGraphicsCommand),
            Description = "Turns the turtle to the degree position specified by its input, Alias: {0}, Usage: {1}",
            Usage = "SETHEADING ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Rotate(int angle);

        [TurtleGraphics(CommandText = "LEFT", Alias = "LT", Type = typeof(TurtleGraphicsCommand),
            Description = "Rotate the turtle the specified amount left in degrees, Alias: {0}, Usage: {1}",
            Usage = "LEFT ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Left(int angle);

        [TurtleGraphics(CommandText = "RIGHT", Alias = "RT", Type = typeof(TurtleGraphicsCommand),
            Description = "Rotate the turtle the specified amount right in degrees, Alias: {0}, Usage: {1}",
            Usage = "RIGHT ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Right(int angle);

        [TurtleGraphics(CommandText = "FORWARD", Alias="FD",  Type = typeof(TurtleGraphicsCommand),
            Description = "Move the turtle forward the specified amount, drawing a line if pen is down, Alias: {0}, Usage: {1}",
            Usage = "FORWARD ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Forward(int length);

        [TurtleGraphics(CommandText = "MOVE", Alias = "MV", Type = typeof(TurtleGraphicsCommand),
           Description = "Move without drawing a line, the turtle forward the specified amount, Alias: {0}, Usage: {1}",
           Usage = "MOVE ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Move(int length);

        [TurtleGraphics(CommandText = "BACKWARD", Alias = "BK", Type = typeof(TurtleGraphicsCommand),
            Description = "Move the turtle backwards the specified amount, Alias: {0}, Usage: {1}",
            Usage = "BACKWARD ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Backward(int length);

        [TurtleGraphics(CommandText = "PENWIDTH", Alias = "PENW", Type = typeof(TurtleGraphicsCommand),
            Description = "Set the width of the pen to the specified amount, Alias: {0}, Usage: {1}",
            Usage = "PENWIDTH ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void PenWidth(int width);

        [TurtleGraphics(CommandText = "HOME", Alias = "HOME", Type = typeof(TurtleGraphicsCommand),
            Description = "Position the turtle in the centre of the screen, draws a line if pen is down, Alias: {0}, Usage: {1}",
            Usage = "HOME")]
        void CenterTurtle();

        [TurtleGraphics(CommandText = "LABEL", Alias="LAB", Type = typeof(TurtleGraphicsCommand),
            Description = "Output the specified text string at the specified font size to the graphics window, Alias: {0}, Usage: {1}", Usage = "LABEL \"?\"")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = false,
            RegEx = GlobalConstants.PatternThatMatchesString, Order = 1)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Float, AllowVariableSubstitution = true,
           RegEx = GlobalConstants.PatternThatMatchVariableOrNumber, Order = 2)]
        void Label(string text, float fontSize);

        [TurtleGraphics(CommandText = "PRINT", Alias = "PRI", Type = typeof(TurtleGraphicsCommand),
            Description = "Output the specified variable, text or number to the debug window, Alias: {0}, Usage: {1}", Usage = "PRINT X")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchTextOrVariableOrNumber)]
        void Print(string text);

        [TurtleGraphics(CommandText = "PRINTLN", Alias = "PRILN", Type = typeof(TurtleGraphicsCommand),
            Description = "Output the specified variable, text or number to the graphics window followed by a newline, Alias: {0}, Usage: {1}", Usage = "PRINTLN X")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.String, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchTextOrVariableOrNumber)]
        void PrintLine(string text);

        [TurtleGraphics(CommandText = "SETX", Alias = "SX", Type = typeof(TurtleGraphicsCommand),
            Description = "Move the turtle to the specified X position, Alias: {0}, Usage: {1}",
            Usage = "SETX ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void SetX(int x);

        [TurtleGraphics(CommandText = "SETPOS", Alias = "SETXY", Type = typeof(TurtleGraphicsCommand),
            Description = "Move the turtle to the specified X, Y position, drawing a line if the pen is down, Alias: {0}, Usage: {1}",
            Usage = "SetPos ? ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
         RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void SetPos(int x, int y);

        [TurtleGraphics(CommandText = "SETY", Alias = "SY", Type = typeof(TurtleGraphicsCommand),
            Description = "Move the turtle to the specified Y position, Alias: {0}, Usage: {1}",
            Usage = "SETY ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void SetY(int y);

        [TurtleGraphics(CommandText = "WAIT", Alias = "WT", Type = typeof(TurtleGraphicsCommand),
            Description = "Wait the specified time in microseconds, Alias: {0}, Usage: {1}",
            Usage = "WAIT ?")]
        [TurtleGraphicsArgumentAttribute(ArgumentType = DataTypes.Integer, AllowVariableSubstitution = true,
            RegEx = GlobalConstants.PatternThatMatchVariableOrNumber)]
        void Wait(int delay);

    }
}