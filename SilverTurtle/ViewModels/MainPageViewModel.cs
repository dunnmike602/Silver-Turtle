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
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SilverTurtle.Helpers;
using SilverTurtle.Io.Interfaces;
using SilverTurtle.Models;
using SilverTurtle.MVVMCommandHelpers;
using SilverTurtle.ViewModels.Interfaces;
using SimpleMvvmToolkit;
using TurtleGraphics.Constants;
using TurtleGraphics.CustomAttributes;
using TurtleGraphics.EventArguments;
using TurtleGraphics.Interfaces;
using TurtleGraphics.VirtualMachine;

namespace SilverTurtle.ViewModels
{
    public class MainPageViewModel : ViewModelDetailBase<MainPageViewModel, TurtleProgramStorage>, IMainPageViewModel
    {
        private const string PenStatusDown = "Pen Down";
        private const string PenStatusUp = "Pen Up";

        private readonly ICanceller _canceller;
        private readonly ITurtleGraphicsRuntime _turtleGraphicsRuntime;
        private readonly int _startForeColorIndex;
        private readonly int _initialPenWidth;
        private readonly Image _turtlePointer;
        private readonly TextBox _debugWindow;
        private readonly TabItem _tiErrorTab;
        private ICommand _printCommand;
        private ICommand _addSnippitCommand;
        private ICommand _addColorCommand;
        private ICommand _addMethodCommand;
        private ICommand _displayFunctionTextCommand;
        private ICommand _showTurtleCommand;
        private ICommand _selectTurtleCommand;
        private readonly ITurtleGraphicsControlStructures _turtleGraphicsControlStructures;
        private readonly ITurtleGraphicsExecutionEngine _turtleGraphicsExecutionEngine;
        private readonly IModalSaveDialog _saveDialog;
        private readonly IFileOperations _fileOperations;
        private readonly IModalPrintDialog _modalPrintDialog;
        private readonly IModalLoadDialog _modalLoadDialog;
        private readonly Color[] _colors;
        private readonly ObservableCollection<FunctionSourceCode> _functionSourceCodeItems =
          new ObservableCollection<FunctionSourceCode>();

        private readonly object _lock = new object();

        public MainPageViewModel(TurtleProgramStorage turtleProgramStorage, Image turtlePointer,
            TextBox debugWindow, TabItem tiErrorTab, ICanceller canceller, ITurtleGraphicsRuntime turtleGraphicsRuntime,
            int startForeColorIndex, int initialPenWidth, Color[] colors, ITurtleGraphicsControlStructures turtleGraphicsControlStructures,
            ITurtleGraphicsExecutionEngine turtleGraphicsExecutionEngine, IModalSaveDialog saveDialog, IFileOperations fileOperations,
            IModalPrintDialog modalPrintDialog, IModalLoadDialog modalLoadDialog)
        {
            _turtlePointer = turtlePointer;
            _debugWindow = debugWindow;
            _tiErrorTab = tiErrorTab;
            _canceller = canceller;
            _turtleGraphicsRuntime = turtleGraphicsRuntime;
            _startForeColorIndex = startForeColorIndex;
            _initialPenWidth = initialPenWidth;
            _colors = colors;
            _turtleGraphicsControlStructures = turtleGraphicsControlStructures;
            _turtleGraphicsExecutionEngine = turtleGraphicsExecutionEngine;
            _saveDialog = saveDialog;
            _fileOperations = fileOperations;
            _modalPrintDialog = modalPrintDialog;
            _modalLoadDialog = modalLoadDialog;
            Model = turtleProgramStorage;
        }

        private void AddMethod(ComboBox methodSelector)
        {
            if (methodSelector.SelectedIndex == -1)
            {
                return;
            }

            Model.AppendImmediateText(((TurtleMethod) methodSelector.SelectedItem).Usage);
        }

        private void DisplayFunctionText(ComboBox functionSelector) 
        {
            var function = functionSelector.SelectedItem as FunctionSourceCode;

            if (function != null)
            {
                Model.CurrentFunctionProgramText =
                    TurtleGraphicsFunctionHandler.Instance.GetFunctionProgramText(function.Name);
            }
        }

        private void AddColor(ComboBox colorSelector)
        {
            if (colorSelector.SelectedIndex == -1)
            {
                return;
            }

            var turtleColor = (TurtleColor) colorSelector.SelectedItem;

            Model.AppendImmediateText(ResourceHelper.GetCodeSnippit(colorSelector.Name) + " " + 
                turtleColor.WellKnownColorName);
        }

        private  void PrintCanvas(UIElement itemToPrint)
        {
           _modalPrintDialog.PrintPage(ResourceHelper.GetStaticText("PrintHeading"), itemToPrint);
        }

        private void AddSnippit(string resourceName)
        {
            Model.AppendImmediateText(ResourceHelper.GetCodeSnippit(resourceName));
        }

        public void TurtleGraphicsExecutionEngineException(object sender, ExecutionEngineErrorEventArgs e)
        {
            Model.ClearErrors();
            Model.AppendErrorText(e.Error.GetBaseException().Message);
            _tiErrorTab.IsSelected = true;
        }

        public void TurtleGraphicsExecutionEngineStatusChanged(object sender, ExecutionStatusChangedEventArgs e)
        {
            if(e.ProgramCountIncreased)
            {
                IncrementProgramCount();
            }
            else
            {
                DecrementProgramCount();
            }
        }

        private void SelectTurtle(ComboBox cmbTurtles)
        {
            if (cmbTurtles.SelectedIndex == -1 ||  _turtlePointer == null)
            {
                return;
            }

            var turtle = (Turtle)cmbTurtles.SelectedItem;

            var imageToSet =  new BitmapImage();
            imageToSet.SetSource(ResourceHelper.GetApplicationResourceStream(turtle.Image));

            _turtlePointer.Source = imageToSet;
        }

        private void ShowTurtle(CheckBox turtleSelection)
        {
            _canceller.SetCancel(false);

            var command = GlobalConstants.HideAllTurtlesCommandText;

            if (turtleSelection.IsChecked != null)
            {
                if (!turtleSelection.IsChecked.Value)
                {
                    command = GlobalConstants.ShowAllTurtlesCommandText;
                }
            }

            _turtleGraphicsExecutionEngine.ExecuteCommandLine(command, false);
        }

        private void HandleFunctionChanged(object sender, FunctionChangedEventArgs e)
        {
            ClearFunctionCodeItems();

            foreach (var function in e.Functions)
            {
                AddFunctionCodeItem(new FunctionSourceCode
                                        {
                                            Name = function.FunctionName,
                                        });
            }

            Model.CurrentFunctionProgramText = string.Empty;
        }

        private void WpfTurtleGraphicsPenWidthChanged(object sender, PenWidthEventArgs e)
        {
            Model.PenWidth = e.Width.ToString();
        }

        private void WpfTurtleGraphicsPenStatusChanged(object sender, PenStatusEventArgs e)
        {
            Model.PenStatus = e.IsUp ? PenStatusUp : PenStatusDown;
        }

        private void NewProgram()
        {
            Model.ClearAllStorage();
        }

        private void WpfTurtleGraphicsColorChanged(object sender, ColorChangeEventArgs e)
        {
            if (e.IsForeground)
            {
                Model.CurrentForegroundBrush = new SolidColorBrush { Color = e.NewColor };
            }
        }

        private void SetupModel()
        {
            Model.PenWidth = _initialPenWidth.ToString();
            Model.PenStatus = PenStatusDown;
            Model.CurrentForegroundBrush = new SolidColorBrush { Color = _colors[_startForeColorIndex] };
        }

        private void InitRuntime()
        {
            _turtleGraphicsRuntime.PenStatusChanged += WpfTurtleGraphicsPenStatusChanged;
            _turtleGraphicsRuntime.PenWidthChanged += WpfTurtleGraphicsPenWidthChanged;
            _turtleGraphicsRuntime.ColorChanged += WpfTurtleGraphicsColorChanged;
        }

        private void ClearFunctionCodeItems()
        {
            _functionSourceCodeItems.Clear();
            NotifyPropertyChanged(m => m.FunctionSourceCodeItems);
        }

        private void AddFunctionCodeItem(FunctionSourceCode functionSourceCode)
        {
            _functionSourceCodeItems.Add(functionSourceCode);
            NotifyPropertyChanged(m => m.FunctionSourceCodeItems);
        }

        private void RunProgramHelper(bool parseOnly)
        {
            Model.ClearErrors();

            var immediateText = Model.ImmediateText.Replace(Environment.NewLine, " ").Trim();

            if (string.IsNullOrEmpty(immediateText))
            {
                return;
            }

            try
            {
                if (ProcessCommandString(immediateText, parseOnly))
                {
                    _tiErrorTab.IsSelected = true;
                }
                else if (!parseOnly)
                {
                    Model.ClearImmediateText();
                }
            }
            catch (Exception ex)
            {
                Model.ClearErrors();
                Model.AppendErrorText(ex.GetBaseException().Message);
                _tiErrorTab.IsSelected = true;
            }
        }

        private bool ProcessCommandString(string commandText, bool parseOnly = false)
        {
            _canceller.SetCancel(false);

            var hasErrors = false;

            var commands = _turtleGraphicsExecutionEngine.ExecuteCommandLine(commandText, parseOnly, WaitForResult);

            if (_turtleGraphicsExecutionEngine.HasExecutedWithoutErrors())
            {
                Model.AppendValidProgramText(Model.ImmediateText);
            }
            else
            {
                hasErrors = true;

                foreach (var command in commands)
                {
                    Model.AppendErrorText(command.GetErrorMessages());
                }
            }

            return hasErrors;
        }

        public ICommand AddSnippitCommand
        {
            get { return _addSnippitCommand ?? (_addSnippitCommand = new RelayCommand<string>(AddSnippit)); }
        }

        public ICommand DisplayFunctionTextCommand
        {
            get
            {
                return _displayFunctionTextCommand ?? (_displayFunctionTextCommand = new
                                                                                         RelayCommand<ComboBox>(
                                                                                         DisplayFunctionText));
            }
        }
      
        public void SetCancel()
        {
            _canceller.SetCancel(true);
            _turtleGraphicsRuntime.CancelWait();
        }

        public ICommand AddMethodCommand
        {
            get { return _addMethodCommand ?? (_addMethodCommand = new RelayCommand<ComboBox>(AddMethod)); }
        }

        public ICommand AddColorCommand
        {
            get { return _addColorCommand ?? (_addColorCommand = new RelayCommand<ComboBox>(AddColor)); }
        }

        public ICommand ShowTurtleCommand
        {
            get { return _showTurtleCommand ?? (_showTurtleCommand = new RelayCommand<CheckBox>(ShowTurtle)); }
        }

        public ICommand SelectTurtleCommand
        {
            get { return _selectTurtleCommand ?? (_selectTurtleCommand = new RelayCommand<ComboBox>(SelectTurtle)); }
        }
        
        public ICommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new RelayCommand<UIElement>(PrintCanvas)); }
        }

        public bool WaitForResult
        {
            get;
            set;
        }

        public void CheckProgram()
        {
            RunProgramHelper(true);
        }

        public void RunProgram()
        {
            RunProgramHelper(false);
        }

        public void StartNewProgram()
        {
            NewProgram();
            _turtleGraphicsRuntime.ClearScreen();
        }

        public void ClearDebugWindow()
        {
            _debugWindow.Text = string.Empty;
        }

        public void SaveProgram()
        {
            _saveDialog.Show();

            if (_saveDialog.DialogResult == null || !_saveDialog.DialogResult.Value)
            {
                return;
            }

            var file = _saveDialog.File();

            _fileOperations.SaveTextFile(file, Model.ProcessedCommands + TurtleGraphicsFunctionHandler.Instance.
                                                                             GetAllFunctionProgramText());
        }

        public void EditProgram()
        {
            Model.ImmediateText = Model.ProcessedCommands;
            Model.ClearProcessedCommands();
        }

        public void IncrementProgramCount()
        {
            lock (_lock)
            {
                Model.ExecutingProgramCount++;
            }
        }

        public void DecrementProgramCount()
        {
            lock (_lock)
            {
                Model.ExecutingProgramCount--;
            }
        }

        public void LoadProgram()
        {
            _modalLoadDialog.Show();

            if (_modalLoadDialog.DialogResult == null || !_modalLoadDialog.DialogResult.Value)
            {
                return;
            }

            NewProgram();

            _turtleGraphicsRuntime.ClearScreen();

            using (var contents = _modalLoadDialog.FileContents())
            {
                Model.ImmediateText = contents.ReadToEnd();
            }
        }

        public ObservableCollection<FunctionSourceCode> FunctionSourceCodeItems
        {
            get { return _functionSourceCodeItems; }
        }

        public void InitialiseTurtleGraphicsSystem()
        {
            SetupModel();
            NewProgram();
        }

        public void StartTurtleGraphicsSystem()
        {
            InitRuntime();
          
            _turtleGraphicsExecutionEngine.Exception += TurtleGraphicsExecutionEngineException;
            _turtleGraphicsExecutionEngine.StatusChanged += TurtleGraphicsExecutionEngineStatusChanged;
            TurtleGraphicsFunctionHandler.Instance.FunctionChanged += HandleFunctionChanged;
            _turtleGraphicsControlStructures.SetCommandProcessor(_turtleGraphicsExecutionEngine);
       }

        public void CentreDefaultTurtle()
        {
            _turtleGraphicsRuntime.CenterDefaultTurtle();
        }
    }
}
