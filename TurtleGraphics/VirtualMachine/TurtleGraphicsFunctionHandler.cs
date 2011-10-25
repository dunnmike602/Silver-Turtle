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
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.Delegates;
using TurtleGraphics.Exceptions;
using TurtleGraphics.Helpers;
using TurtleGraphics.Interfaces;

namespace TurtleGraphics.VirtualMachine
{
    public class TurtleGraphicsFunctionHandler : ITurtleGraphicsFunctionHandler
    {
       private static readonly TurtleGraphicsFunctionHandler FunctionHandler = new TurtleGraphicsFunctionHandler();
       private TurtleGraphicsFunctionHandler() { }
       private readonly object _functionLock = new object();

       public static TurtleGraphicsFunctionHandler Instance
        {
            get
            {
                return FunctionHandler;
            }
        }

        public event FunctionChangedEventHandler FunctionChanged;

        private readonly List<TurtleGraphicsFunctionCommand> _functions = new List<TurtleGraphicsFunctionCommand>();

        protected virtual void OnFunctionChange(FunctionChangedEventArgs e)
        {
            if (FunctionChanged != null)
            {
                FunctionChanged(this, e);
            }
        }

        public List<TurtleGraphicsFunctionCommand> GetFunctions()
        {
            return _functions;
        }
        
        public void AddFunction(TurtleGraphicsFunctionCommand function)
        {
            lock (_functionLock)
            {
                // Remove existing function so declarations can be replaced.
                var functionToDelete = _functions.Where(m => m.FunctionName == function.FunctionName).
                    FirstOrDefault();

                if (functionToDelete != null)
                {
                    _functions.Remove(functionToDelete);
                }

                _functions.Add(function);
            }

            OnFunctionChange(new FunctionChangedEventArgs(_functions));
        }

        public string GetFunctionProgramText(string functionName)
        {
            lock (_functionLock)
            {
                var function = _functions.Where(m => m.FunctionName == functionName).
                    FirstOrDefault();

                if (function == null)
                {
                    throw new TurtleRuntimeException(
                        string.Format(ResourceHelper.GetStaticText("FunctionHasNotBeenDeclared"), functionName));
                }

                return function.ProgramText;
            }
        }

        public string GetAllFunctionProgramText()
        {
            lock (_functionLock)
            {
                if (_functions.Count == 0)
                {
                    return string.Empty;
                }

                var text = Environment.NewLine + Environment.NewLine + @"REM ""======= User Defined Functions ======="""
                           + Environment.NewLine;

                foreach (var function in _functions)
                {
                    text += function.ProgramText;
                    text += Environment.NewLine;
                }

                return text;
            }
        }

        public void ClearFunctions()
        {
            lock (_functionLock)
            {
                _functions.Clear();
            }

            OnFunctionChange(new FunctionChangedEventArgs(_functions));
        }
    }
}