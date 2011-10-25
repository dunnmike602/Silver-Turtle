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
using TurtleGraphics.Enums;
using TurtleGraphics.Helpers;
using TurtleGraphics.Interfaces;
using System.Collections.Generic;
using TurtleGraphics.CustomAttributes;
using System.Linq;

namespace TurtleGraphics
{
    public class TurtleGraphicsReflectionMatcher : ITurtleGraphicsCommandMatcher
    {
        private readonly ITurtleGraphicsCommands _turtleGraphicsSystem;
        private readonly ITurtleGraphicsControlStructures _turtleGraphicsControlStructures;
        private List<TurtleGraphicsAttribute> _turtleGraphicsCommandAttributes;
        
        public TurtleGraphicsReflectionMatcher(ITurtleGraphicsCommands turtleGraphicsSystem,
            ITurtleGraphicsControlStructures turtleGraphicsControlStructures)
        {
            _turtleGraphicsSystem = turtleGraphicsSystem;
            _turtleGraphicsControlStructures = turtleGraphicsControlStructures;
            _turtleGraphicsCommandAttributes = new List<TurtleGraphicsAttribute>();
        }

        private TurtleGraphicsCommand MatchBuiltInCommand(string commandText)
        {
            commandText = commandText.ToLower();

            _turtleGraphicsCommandAttributes = ReflectionHelper.GetTurtleGraphicsCommandAttributes();

            TurtleGraphicsAttribute turtleGraphicsAttribute =
                _turtleGraphicsCommandAttributes.SingleOrDefault(p => p.CommandText.ToLower() == commandText
                    || p.Alias.ToLower() == commandText);

            if (turtleGraphicsAttribute == null)
            {
                return null;
            }

            var turtleGraphicsCommand = CreateCommandFromType(turtleGraphicsAttribute.Type);

            turtleGraphicsCommand.CommandText = commandText;
            turtleGraphicsCommand.Status = TurtleGraphicsCommandStatus.Valid;
            turtleGraphicsCommand.Attribute = turtleGraphicsAttribute;
            turtleGraphicsCommand.ImplementingFunctionName = ReflectionHelper.GetMethodForCommand(turtleGraphicsCommand.
                CommandText, TurtleGraphicsCommandTypes.Commands);
  
            turtleGraphicsCommand.ExecutionContext = _turtleGraphicsSystem;

            if (turtleGraphicsCommand.ImplementingFunctionName == null)
            {
                turtleGraphicsCommand.ExecutionContext = _turtleGraphicsControlStructures;
                turtleGraphicsCommand.ImplementingFunctionName = ReflectionHelper.GetMethodForCommand(turtleGraphicsCommand.
                    CommandText, TurtleGraphicsCommandTypes.ControlStructures);
            }

            turtleGraphicsCommand.ArgumentAttributes = ReflectionHelper.GetArgumentAttributesForMethod(turtleGraphicsCommand.
                ImplementingFunctionName);
      
            return turtleGraphicsCommand;
        }

        private static TurtleGraphicsCommand CreateCommandFromType(Type processType)
        {
            return (TurtleGraphicsCommand)Activator.CreateInstance(processType);
        }

        private static TurtleGraphicsCommand GetUncertainCommandHelper()
        {
            var turtleGraphicsCommand = new TurtleGraphicsCommand
            {
                Status = TurtleGraphicsCommandStatus.InvalidCommand,
            };

            turtleGraphicsCommand.ErrorMessage = ReflectionHelper.GetEnumDescription(
                turtleGraphicsCommand.Status);

            return turtleGraphicsCommand;
        }

        public TurtleGraphicsCommand Match(string commandText)
        {
            return MatchBuiltInCommand(commandText) ?? GetUncertainCommandHelper();
        }
    }
}
