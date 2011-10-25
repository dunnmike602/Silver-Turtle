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
using System.Windows.Media;
using SimpleMvvmToolkit;

namespace SilverTurtle.Models
{
    public class TurtleProgramStorage : ModelBase<TurtleProgramStorage>
    {
        private string _immediateText = string.Empty;
        private string _processedCommands = string.Empty;
        private string _programErrors = string.Empty;
        private string _penWidth = string.Empty;
        private string _penStatus = string.Empty;
        private SolidColorBrush _currentForegroundBrush;
        private string _currentForegroundColorName;
        private string _currentFunctionProgramText;
        private int _executingProgramCount;

        public bool EnableNewProgram
        {
            get
            {
                return (ExecutingProgramCount <= 0);
            }
        }

        public int ExecutingProgramCount
        {
            get { return _executingProgramCount; }
            set
            {
                _executingProgramCount = value;
                NotifyPropertyChanged(m => m.EnableNewProgram);
            }
        }

        public string CurrentFunctionProgramText
        {
            get { return _currentFunctionProgramText; }
            set
            {
                _currentFunctionProgramText = value;
                NotifyPropertyChanged(m => m.CurrentFunctionProgramText);
            }
        }

        public string PenStatus
        {
            get { return _penStatus; }
            set
            {
                _penStatus = value;
                NotifyPropertyChanged(m => m.PenStatus);
            }
        }

        public string PenWidth
        {
            get { return _penWidth; }
            set
            {
                _penWidth = value;
                NotifyPropertyChanged(m => m.PenWidth);
            }
        }

        public string ProcessedCommands
        {
            get { return _processedCommands.Trim();}
            private set
            {
                _processedCommands = value;
                NotifyPropertyChanged(m => m.ProcessedCommands);
            }
        }

        public string ProgramErrors
        {
            get { return _programErrors.Trim();}
            set
            {
                _programErrors = value;
                NotifyPropertyChanged(m => m.ProgramErrors);
            }
        }

        public string CurrentForegroundColorName
        {
            get { return _currentForegroundColorName; }
            set
            {
                _currentForegroundColorName = value;
                NotifyPropertyChanged(m => m.CurrentForegroundColorName);
            }
        }

        public SolidColorBrush CurrentForegroundBrush
        {
            get { return _currentForegroundBrush; }
            set
            {
                _currentForegroundBrush = value;
                CurrentForegroundColorName = new TurtleColor {Color = value.Color}.WellKnownColorName;
                NotifyPropertyChanged(m => m.CurrentForegroundBrush);
            }
        }

        public string ImmediateText
        {
            get
            {
                return _immediateText.Trim();
            }
            set
            {
                _immediateText = value;
                NotifyPropertyChanged(m => m.ImmediateText);
            }
        }

        public void ClearErrors()
        {
            ProgramErrors = string.Empty;
        }

        public void ClearProcessedCommands()
        {
            ProcessedCommands = string.Empty;
        }

        public void ClearImmediateText()
        {
            ImmediateText = string.Empty;
        }

        public void ClearAllStorage()
        {
            ClearImmediateText();
            ClearErrors();
            ClearProcessedCommands();
        }

        public void AppendErrorText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var programErrors = ProgramErrors;

            if (!string.IsNullOrEmpty(programErrors))
            {
                programErrors += Environment.NewLine;
            }

            programErrors += value;

            ProgramErrors = programErrors;
        }

        public void AppendImmediateText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var immediateText = ImmediateText;

            if (!string.IsNullOrEmpty(immediateText))
            {
                immediateText += Environment.NewLine;
            }

            immediateText += value;

            ImmediateText = immediateText;
        }

        public void AppendValidProgramText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var processedCommands = ProcessedCommands;

            if (!string.IsNullOrEmpty(processedCommands))
            {
                processedCommands += Environment.NewLine;
            }

            processedCommands += value;

            ProcessedCommands = processedCommands;
        }
    }
}
