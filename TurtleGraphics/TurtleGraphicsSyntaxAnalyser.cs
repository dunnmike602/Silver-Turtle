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
using System.Linq;
using TurtleGraphics.Constants;
using TurtleGraphics.Enums;
using TurtleGraphics.Exceptions;
using TurtleGraphics.Helpers;
using TurtleGraphics.Interfaces;
using TurtleGraphics.VirtualMachine;

namespace TurtleGraphics
{
    public class TurtleGraphicsSyntaxAnalyser : ITurtleGraphicsSyntaxAnalyser
    {
        private readonly ITurtleGraphicsCommandMatcher _turtleGraphicsCommandMatcher;
        private int _currentTokenIndex;

        public TurtleGraphicsSyntaxAnalyser(ITurtleGraphicsCommandMatcher turtleGraphicsCommandMatcher)
        {
            _turtleGraphicsCommandMatcher = turtleGraphicsCommandMatcher;
        }

        private void GetDoFunctionArgumentsFromTokens(IList<string> tokens,
             TurtleGraphicsDoFunctionCommand doFunctionCommand)
        {
            ITurtleGraphicsFunctionHandler turtleGraphicsFunctionHandler = TurtleGraphicsFunctionHandler.Instance;

            if(_currentTokenIndex >= tokens.Count)
            {
                return;
            }

            var argValues = new List<string> { tokens[_currentTokenIndex] };

            _currentTokenIndex++;

            var function = turtleGraphicsFunctionHandler.GetFunctions().Where(
                m => m.FunctionName.ToLower() == argValues[0].ToLower()).
                FirstOrDefault();

            if(function == null)
            {
                throw new TurtleRuntimeException(
                    string.Format(ResourceHelper.GetStaticText("FunctionHasNotBeenDeclared"), argValues[0]));
            }

            // Function takes a variable number of parameters that can only be determined at this point
            // Both attributes and values have to be populated.
            var numArgs = function.ArgumentValues.Count - 1;

            while (_currentTokenIndex < tokens.Count && numArgs > 0)
            {
                argValues.Add(tokens[_currentTokenIndex]);
                _currentTokenIndex++;
                numArgs--;
            }

            doFunctionCommand.ArgumentValues = argValues;
            
            // Add the additional ArgumentAttributes
            var commandAttribute = doFunctionCommand.ArgumentAttributes[1];

            for (var i = 0; i < function.ArgumentValues.Count - 2; i++)
            {
                doFunctionCommand.ArgumentAttributes.Add(commandAttribute);
            }
        }

        private void GetFunctionArgumentsFromTokens(IList<string> tokens, TurtleGraphicsCommand 
            turtleGraphicsCommand)
        {
            var argValues = new List<string>();

            while (_currentTokenIndex < tokens.Count && !IsStartBlockToken(tokens[_currentTokenIndex]))
            {
                argValues.Add(tokens[_currentTokenIndex]);
                _currentTokenIndex++;
            }

            turtleGraphicsCommand.ArgumentValues = argValues;

            // Function takes a variable number of parameters that can only be determined at this point
            var countDifference = turtleGraphicsCommand.ArgumentValues.Count -
                                  turtleGraphicsCommand.ArgumentAttributes.Count;

            if(countDifference >= 1)
            {
                var commandAttribute = turtleGraphicsCommand.ArgumentAttributes[0];

                for (var i = 0; i < countDifference; i++)
                {
                    turtleGraphicsCommand.ArgumentAttributes.Add(commandAttribute);
                }
            }
        }

        private void GetArgumentsFromTokens(IList<string> tokens, TurtleGraphicsCommand turtleGraphicsCommand)
        {
            var argValues = new List<string>();

            for (var nextAttrIdx = 0; nextAttrIdx < turtleGraphicsCommand.ArgumentAttributes.Count;
                 nextAttrIdx++)
            {
                if (_currentTokenIndex >= tokens.Count)
                {
                    break;
                }

                argValues.Add(tokens[_currentTokenIndex]);

                _currentTokenIndex++;
            }

            turtleGraphicsCommand.ArgumentValues = argValues;
        }

        private static bool IsStartBlockToken(string currentToken)
        {
            return currentToken == GlobalConstants.StartLoopToken;
        }

        private static bool IsEndBlockToken(string currentToken)
        {
            return currentToken == GlobalConstants.EndLoopToken;
        }

        private static bool IsBlockToken(string currentToken)
        {
            return IsStartBlockToken(currentToken) || IsEndBlockToken(currentToken);
        }

        private static void AddProgramText(TurtleGraphicsCommand turtleGraphicsCommand, IList<string> tokens, 
            int startTokenForCommand, int endTokenForCommand)
        {
            turtleGraphicsCommand.ProgramText = string.Empty;

            for (var i = startTokenForCommand; i < endTokenForCommand; i++)
            {
                var currentToken = tokens[i];

                if(IsBlockToken(currentToken))
                {
                    turtleGraphicsCommand.ProgramText += Environment.NewLine + currentToken + " " + Environment.NewLine;
                }
                else
                {
                    turtleGraphicsCommand.ProgramText += currentToken + " ";
                }
            }
        }

        private void ConvertTokensToCommandsHelper(IList<string> tokens, ICollection<TurtleGraphicsCommand> commands)
        {
            var currentToken = tokens[_currentTokenIndex];

            // Check for loop boundary tokens
            if (IsStartBlockToken(currentToken))
            {
                _currentTokenIndex++;

                if (_currentTokenIndex < tokens.Count)
                {
                    currentToken = tokens[_currentTokenIndex];
                }
                else
                {
                    return;
                }
            }

            if (IsEndBlockToken(currentToken))
            {
                return;
            }

            var turtleGraphicsCommand = _turtleGraphicsCommandMatcher.Match(currentToken);

            if (turtleGraphicsCommand.Status != TurtleGraphicsCommandStatus.Valid)
            {
                turtleGraphicsCommand.CommandText = currentToken;
                commands.Add(turtleGraphicsCommand);

                _currentTokenIndex++;

                if (_currentTokenIndex < tokens.Count)
                {
                    ConvertTokensToCommandsHelper(tokens, commands);
                }

                return;
            }

            var blockCommand = turtleGraphicsCommand as TurtleGraphicsBlockCommand;
            var functionCommand = turtleGraphicsCommand as TurtleGraphicsFunctionCommand;
            var doFunctionCommand = turtleGraphicsCommand as TurtleGraphicsDoFunctionCommand;

            var startTokenForCommand = _currentTokenIndex;

            _currentTokenIndex++;

            commands.Add(turtleGraphicsCommand);

            if (blockCommand != null)
            {
                GetArgumentsFromTokens(tokens, blockCommand);
                blockCommand.Commands = new List<TurtleGraphicsCommand>();

                if (_currentTokenIndex < tokens.Count)
                {
                    ConvertTokensToCommandsHelper(tokens, blockCommand.Commands);
                    _currentTokenIndex++;
                }

                blockCommand.ValidateCommand();
            }
            else if (functionCommand != null)
            {
                GetFunctionArgumentsFromTokens(tokens, functionCommand);

                if (functionCommand.Status == TurtleGraphicsCommandStatus.Valid)
                {
                    TurtleGraphicsFunctionHandler.Instance.AddFunction(functionCommand);
                }

                functionCommand.Commands = new List<TurtleGraphicsCommand>();

                if (_currentTokenIndex < tokens.Count)
                {
                    ConvertTokensToCommandsHelper(tokens, functionCommand.Commands);
                    _currentTokenIndex++;
                }

                functionCommand.ValidateCommand();

                if (functionCommand.Status == TurtleGraphicsCommandStatus.Valid)
                {
                    // Record the actual program text of functions so it can be called later.
                    AddProgramText(turtleGraphicsCommand, tokens, startTokenForCommand, _currentTokenIndex);
                }

            }
            else if (doFunctionCommand != null)
            {
                GetDoFunctionArgumentsFromTokens(tokens, doFunctionCommand);
                doFunctionCommand.ValidateCommand();
            }
            else
            {
                GetArgumentsFromTokens(tokens, turtleGraphicsCommand);
                turtleGraphicsCommand.ValidateCommand();
            }

            if (_currentTokenIndex < tokens.Count)
            {
                ConvertTokensToCommandsHelper(tokens, commands);
            }
        }

        public List<TurtleGraphicsCommand> ConvertTokensToCommands(IList<string> tokens)
        {
            _currentTokenIndex = 0;

            var commands = new List<TurtleGraphicsCommand>();

            ConvertTokensToCommandsHelper(tokens, commands);

            return commands;
        }
    }
}