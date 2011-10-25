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
using TurtleGraphics.Exceptions;
using TurtleGraphics.Helpers;

namespace TurtleGraphics.VirtualMachine
{
    /// <summary>
    /// Encapsulates a mechanism for handling float variables. ReadOnlyVariables are only used
    /// by the application to create constants, they will be never updated on multiple threads
    /// only read. Other variables are thread-safe for access on multiple threads.
    /// </summary>
    public sealed class FloatVariableHandler : NumericVariableHandler<float>
    {
        private static readonly NumericVariableHandler<float> FloatHandlerInstance 
            = new FloatVariableHandler();

        private readonly object _lock = new object();

        private FloatVariableHandler() { }

        public static NumericVariableHandler<float> Instance
        {
            get
            {
                return FloatHandlerInstance;
            }
        }

        public override void SetReadOnly(string name, float value)
        {
            name = name.ToUpper();

            if (ReadOnlyVariables.ContainsKey(name))
            {
                ReadOnlyVariables[name] = value;
            }
            else
            {
                ReadOnlyVariables.Add(name, value);
            }
        }

        public override void SetReadOnly(string[] name, float[] value)
        {
            for (var nameIdx = 0; nameIdx < name.Length; nameIdx++)
            {
                var nextName = name[nameIdx];
                var nextValue = value[nameIdx];

                SetReadOnly(nextName, nextValue);
            }
        }

        public override void Set(string name, float value)
        {
            name = name.ToUpper();

            if (ReadOnlyVariables.ContainsKey(name))
            {
                throw new TurtleRuntimeException(ResourceHelper.GetStaticText("CannotChangeReadOnlyVariable"));
            }

            // Need to lock the collection when checking for a value or eventually we will
            // see an duplicate key exception when multiple threads try to initalise the same
            // variable
            lock (_lock)
            {
                if (!Variables.ContainsKey(name))
                {
                    Variables.Add(name, value);
                }
            }

            // Don't needs to take a lock here, if 2 threads are using the same variable (not recommended anyway)
            // the last in will win.
            Variables[name] = value;
        }

        public override float Get(string name)
        {
            name = name.ToUpper();

            if (ReadOnlyVariables.ContainsKey(name))
            {
                return ReadOnlyVariables[name];
            }

            // Retrieving a value needs to be an atomic operation.
            lock (_lock)
            {
                if (!Variables.ContainsKey(name))
                {
                    Variables.Add(name, 0);
                }

                return Variables[name];
            }
        }
    }
}
