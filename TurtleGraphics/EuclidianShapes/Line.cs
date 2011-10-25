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
using System.Windows;
using TurtleGraphics.Constants;
using TurtleGraphics.Maths;

namespace TurtleGraphics.EuclidianShapes
{
    public class EuclidianLine
    {
       public double Length { get; set; }
       public Point StartPoint { get; set; }
       public Point EndPoint { get; set; }

       public Point SetEndLocationFromAngle(double rotationAngle)
       {
           // North is considered 0 degrees so need to adjust the angle according
           rotationAngle = (rotationAngle - GlobalConstants.NinetyDegrees) % GlobalConstants.ThreeSixtyDegrees;

           EndPoint = new Point
           {
               X = StartPoint.X + Length * Math.Cos(MathsHelper.DegreeToRadian(rotationAngle)),
               Y = StartPoint.Y + Length * Math.Sin(MathsHelper.DegreeToRadian(rotationAngle))
           };

           return EndPoint;
       }

       public Point ClampEndPointToPlane(double planeStartX, double planeStartY, double planeWidth, double planeHeight)
       {
           var newEndPoint = EndPoint;

           newEndPoint.X = Math.Min(newEndPoint.X, planeWidth);
           newEndPoint.X = Math.Max(newEndPoint.X, planeStartX);
           newEndPoint.Y = Math.Min(newEndPoint.Y, planeHeight);
           newEndPoint.Y = Math.Max(newEndPoint.Y, planeStartX);

           if(!(EndPoint == newEndPoint))
           {
               EndPoint = newEndPoint;
           }

           return newEndPoint;
       }
    }
}
