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
using System.Windows.Input;

namespace SilverTurtle.MVVMCommandHelpers
{
    public sealed class RelayCommand : ICommand, IDisposable
    {
        #region Member variables

        private Predicate<object> _oCanExecuteMethod;
        private Action<object> _oExecuteMethod;

        #endregion

        #region Constructors

        public RelayCommand(Action<object> oExecuteMethod)
            : this(oExecuteMethod, null)
        {
        }
        public RelayCommand(Action<object> oExecuteMethod, Predicate<object> oCanExecuteMethod)
        {
            if (oExecuteMethod == null)
            {
                throw new ArgumentNullException("oExecuteMethod", @"Delegate commands can not be null");
            }

            _oExecuteMethod = oExecuteMethod;
            _oCanExecuteMethod = oCanExecuteMethod;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                
            }
            remove
            {
                
            }
        }
        public bool CanExecute(object parameter)
        {
            return _oCanExecuteMethod == null ? true : _oCanExecuteMethod(parameter);
        }
        public void Execute(object parameter)
        {
            _oExecuteMethod(parameter);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _oCanExecuteMethod = null;
            _oExecuteMethod = null;
        }

        #endregion
    }

    public sealed class RelayCommand<T> : ICommand, IDisposable
    {
        #region Member variables

        private Predicate<T> _oCanExecuteMethod;
        private Action<T> _oExecuteMethod;

        #endregion

        #region Constructors

        public RelayCommand(Action<T> oExecuteMethod)
            : this(oExecuteMethod, null)
        {
        }
        public RelayCommand(Action<T> oExecuteMethod, Predicate<T> oCanExecuteMethod)
        {
            if (oExecuteMethod == null)
            {
                throw new ArgumentNullException("oExecuteMethod", @"Delegate commands can not be null");
            }

            _oExecuteMethod = oExecuteMethod;
            _oCanExecuteMethod = oCanExecuteMethod;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                
            }
            remove
            {
                
            }
        }

        public bool CanExecute(object parameter)
        {
            return _oCanExecuteMethod == null ? true : _oCanExecuteMethod((T)parameter);
        }

        public void Execute(object parameter)
        {
            _oExecuteMethod((T)parameter);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _oCanExecuteMethod = null;
            _oExecuteMethod = null;
        }

        #endregion
    }
}
