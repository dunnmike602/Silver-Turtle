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
using System.Collections.Generic;
using Rhino.Mocks;
using TurtleGraphics;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;
using TurtleGraphics.Interfaces;

namespace TurtleGraphicsTests.UnitTests
{
    public class WhenExecutingCommandLinesBase
    {
        public static ITurtleGraphicsLexicalAnalyser TurtleGraphicsLexicalAnalyser;
        public static ITurtleGraphicsSyntaxAnalyser TurtleGraphicsSyntaxAnalyser;

        public static TurtleGraphicsAttribute GetTurtleGraphicsAttribute(string commandText,
            Type processType = null)
        {
            if(processType == null)
            {
                processType = typeof (TurtleGraphicsCommand);
            }

            return new TurtleGraphicsAttribute
                       {
                           Type = processType,
                           CommandText = commandText
                       };
        }

        public static ITurtleGraphicsSyntaxAnalyser GetSyntaxAnalyserMock(TurtleGraphicsCommand command)
        {
            var commands = new List<TurtleGraphicsCommand> { command };

            TurtleGraphicsSyntaxAnalyser.Expect(m => m.ConvertTokensToCommands(null)).IgnoreArguments().
                Return(commands).Repeat.Once();

            return TurtleGraphicsSyntaxAnalyser;
        }

        public static TurtleGraphicsExecutionEngine GetExecutionEngineInstance(ITurtleGraphicsLexicalAnalyser textParserMock,
           ITurtleGraphicsSyntaxAnalyser turtleGraphicsSyntaxAnalyser)
        {
            return new TurtleGraphicsExecutionEngine(textParserMock, turtleGraphicsSyntaxAnalyser,
                MockRepository.GenerateStub<ICanceller>(), MockRepository.GenerateStub<ITurtleGraphicsRuntime>());
        }
        
        public static ITurtleGraphicsLexicalAnalyser GetTextParserMock(List<string> commandText)
        {
            TurtleGraphicsLexicalAnalyser.Expect(m => m.Parse(null)).IgnoreArguments().
                Return(commandText).Repeat.Once();

            return TurtleGraphicsLexicalAnalyser;
        }

        public static List<TurtleGraphicsArgumentAttribute> GetArgumentAttributesHelper(int count, 
                                                                                         IList<DataTypes> dataTypes)
        {
            var attributes = new List<TurtleGraphicsArgumentAttribute>();

            for (var i = 0; i < count; i++)
            {
                attributes.Add(new TurtleGraphicsArgumentAttribute
                                   {
                                       ArgumentType = dataTypes[i],
                                   });
            }

            return attributes;
        }
    }
}