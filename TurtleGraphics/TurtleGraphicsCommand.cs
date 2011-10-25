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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TurtleGraphics.Constants;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Enums;
using System.Reflection;
using System.Linq;
using TurtleGraphics.Helpers;
using TurtleGraphics.Interfaces;
using TurtleGraphics.Maths;
using System;
using TurtleGraphics.VirtualMachine;

namespace TurtleGraphics
{
    public class TurtleGraphicsCommand
    {
        private List<TurtleGraphicsArgumentAttribute> _argumentAttributes;
        private string _commandText;
        private readonly IVariableHandler<float> _globalVariableHandler;
        private string _programText;

        public TurtleGraphicsCommand()
        {
            ArgumentAttributes = new List<TurtleGraphicsArgumentAttribute>();
            ArgumentValues = new List<string>();
            _globalVariableHandler = FloatVariableHandler.Instance;
            Commands = new List<TurtleGraphicsCommand>();
        }

        public List<TurtleGraphicsCommand> Commands { get; set; }
        
        public string ProgramText
        {
            get
            {
                return string.IsNullOrEmpty(_programText)
                           ? string.Empty
                           : _programText.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }
            set { _programText = value; }
        }

        public string CommandText
        {
            get { return _commandText; }
            set { _commandText = value.ToUpper(); }
        }

        public TurtleGraphicsCommandStatus Status { get; set; }

        public string ErrorMessage { get;  set; }

        public object ExecutionContext { get; set; }

        public string ImplementingFunctionName { get; set; }

        private MethodInfo ImplementingFunction
        {
            get { return ExecutionContext.GetType().GetMethod(ImplementingFunctionName); }
        }

        public TurtleGraphicsAttribute Attribute { get; set; }
        
        public List<TurtleGraphicsArgumentAttribute> ArgumentAttributes
        {
            get { return _argumentAttributes; }
            set
            {
                if (value != null)
                {
                    _argumentAttributes = value;
                }
            }
        }

        public List<string> ArgumentValues { get; set; }

        public object[] GetTypedArgumentValues()
        {   
            var argumentValues = new object[ArgumentValues.Count];

            for (var nextArgIdx = 0; nextArgIdx < ArgumentValues.Count; nextArgIdx++)
            {
                argumentValues[nextArgIdx] = GetTypedArgumentValue(nextArgIdx);
            }

            return argumentValues;
        }

        public object GetTypedArgumentValue(int argumentIndex)
        {
            switch (ArgumentAttributes[argumentIndex].ArgumentType)
            {
                case DataTypes.Integer:
                    var intValue = ArgumentAttributes[argumentIndex].AllowVariableSubstitution
                                       ? SubstituteVariables(argumentIndex)
                                       : Convert.ToInt32(ArgumentValues[argumentIndex]);

                    return (int) intValue;

                case DataTypes.Float:
                    var floatValue = ArgumentAttributes[argumentIndex].AllowVariableSubstitution
                                         ? SubstituteVariables(argumentIndex)
                                         : (float) Convert.ToDouble(ArgumentValues[argumentIndex]);

                    return floatValue;

                case DataTypes.String:
                    var stringValue = ArgumentValues[argumentIndex];

                    if (!stringValue.Contains(GlobalConstants.Quote) &&
                        ArgumentAttributes[argumentIndex].AllowVariableSubstitution)
                    {
                        stringValue = SubstituteVariables(argumentIndex).ToString();
                    }

                    return stringValue;

                default:
                    return ArgumentValues[argumentIndex];
            }
        }

        private void ValidateArguments()
        {
            for (var nextArgIdx = 0; nextArgIdx < ArgumentAttributes.Count; nextArgIdx++)
            {
                if (ArgumentValues.Count == 0 || ArgumentValues.Count <= nextArgIdx)
                {
                    Status = TurtleGraphicsCommandStatus.MissingArguments;
                    ErrorMessage = string.Format(ReflectionHelper.GetEnumDescription(Status), nextArgIdx + 1);
                    return;
                }

                var argumentType = ArgumentAttributes[nextArgIdx].ArgumentType;
                var argumentValue = ArgumentValues[nextArgIdx];
                var regEx = ArgumentAttributes[nextArgIdx].RegEx;

                if (!ArgumentAttributes[nextArgIdx].AllowVariableSubstitution && 
                    argumentType == DataTypes.Integer && !MathsHelper.IsNumber(argumentValue))
                {
                    Status = TurtleGraphicsCommandStatus.NotAnInteger;
                    ErrorMessage = string.Format(ReflectionHelper.GetEnumDescription(Status), nextArgIdx + 1);
                    return;
                }

                if (regEx != null && new Regex(regEx).Match(argumentValue).Length == 0) 
                {
                    Status = TurtleGraphicsCommandStatus.InvalidArgumentPattern;
                    ErrorMessage = string.Format(ReflectionHelper.GetEnumDescription(Status), nextArgIdx+1);
                    return;
                }
            }
        }

        private float SubstituteVariables(int nextArgIdx)
        {
            if (!MathsHelper.IsNumber(ArgumentValues[nextArgIdx]))
            {
                return _globalVariableHandler.Get(ArgumentValues[nextArgIdx]);
            }

            return (float)Convert.ToDouble(ArgumentValues[nextArgIdx]);
        }

        public virtual void ValidateCommand()
        {  
            Status = TurtleGraphicsCommandStatus.Valid;

            if (Attribute == null)
            {
                Status = TurtleGraphicsCommandStatus.InvalidCommand;
                ErrorMessage = ReflectionHelper.GetEnumDescription(Status);
            }

            ValidateArguments();
        }
  
        public string GetErrorMessages()
        {
            var invalidCommands = string.Empty;

            if (Status != TurtleGraphicsCommandStatus.Valid)
            {
                invalidCommands += (CommandText + " " + ErrorMessage);
                invalidCommands += Environment.NewLine;
            }

            return Commands.Aggregate(invalidCommands, (current, turtleGraphicsCommand) => 
                current + turtleGraphicsCommand.GetErrorMessages());
        }

        public int CountInValidCommands()
        {
            var invalidCommands = 0;
            
            if(Status != TurtleGraphicsCommandStatus.Valid)
            {
                invalidCommands += 1;
            }

            invalidCommands += Commands.Sum(command => command.CountInValidCommands());

            return invalidCommands;
        }

        public bool HasCommandOfName(string commandName)
        {
            if(commandName == CommandText)
            {
                return true;
            }

            var hasCommand = false;
  
            foreach (var innerCommand in Commands)
            {
                if (!hasCommand)
                {
                    hasCommand = innerCommand.HasCommandOfName(commandName);
                }
                else
                {
                    break;
                }
            }

            return hasCommand;
        }

        /// <summary>
        /// Executes the turtle graphics command and all nested commands, overridden in subclasses
        /// to implemente execution behavior. For example loops and function will behave differently
        /// </summary>
        public virtual bool Execute(ICanceller canceller)
        {
            if(canceller.ShouldCancel())
            {
                return false;
            }

            var shouldContinue = true;

            if (Attribute != null && Attribute.Type == typeof(TurtleGraphicsCommand))
            {
                ImplementingFunction.Invoke(ExecutionContext, ArgumentValues.Count == 0
                                                                  ? null
                                                                  : GetTypedArgumentValues());
            }
            else if (Attribute != null && Attribute.Type == typeof(TurtleGraphicsDoFunctionCommand))
            {
                // Do function takes a param array so arguments need to be repackaged.
                var argumentList = GetTypedArgumentValues();
                var invokeList = new object[2];

                invokeList[0] = argumentList[0];
               
                var destinationArray = new float[argumentList.Length - 1];
                Array.Copy(argumentList, 1, destinationArray, 0, argumentList.Length - 1);
                invokeList[1] = destinationArray;

                ImplementingFunction.Invoke(ExecutionContext, invokeList);
            }

            foreach (var innerCommand in Commands)
            {
                if (shouldContinue)
                {
                    shouldContinue = innerCommand.Execute(canceller);
                }
                else
                {
                    break;
                }
            }

            return shouldContinue;
        }
    }
}
