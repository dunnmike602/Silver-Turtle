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
using System.ComponentModel;
using System.Threading;
using TurtleGraphics.Delegates;
using TurtleGraphics.EventArguments;
using TurtleGraphics.Interfaces;
using System.Linq;

namespace TurtleGraphics
{
    public class TurtleGraphicsExecutionEngine : ITurtleGraphicsExecutionEngine
    {
        public event ExecutionEngineErrorEventHandler Exception;
        public event ExecutionEngineStatusChangedEventHandler StatusChanged;
        private readonly ITurtleGraphicsLexicalAnalyser _textParser;
        private readonly ITurtleGraphicsSyntaxAnalyser _turtleGraphicsSyntaxAnalyser;
        private readonly ICanceller _canceller;
        private readonly ITurtleGraphicsRuntime _turtleGraphicsRuntime;
   
        private bool _executeWithoutErrors;
       
        public bool HasExecutedWithoutErrors()
        {
            return _executeWithoutErrors;
        }

        public void OnStatusChanged(ExecutionStatusChangedEventArgs e)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, e);
            }
        }

        public void OnException(ExecutionEngineErrorEventArgs e)
        {
            if (Exception != null)
            {
                Exception(this, e);
            }
        }

        public TurtleGraphicsExecutionEngine(ITurtleGraphicsLexicalAnalyser textParser,
            ITurtleGraphicsSyntaxAnalyser turtleGraphicsSyntaxAnalyser, ICanceller canceller,
            ITurtleGraphicsRuntime turtleGraphicsRuntime)
        {
            _textParser = textParser;
            _turtleGraphicsSyntaxAnalyser = turtleGraphicsSyntaxAnalyser;
            _canceller = canceller;
            _turtleGraphicsRuntime = turtleGraphicsRuntime;
        }
        
        private static int CountInValidCommands(IEnumerable<TurtleGraphicsCommand> commands)
        {
            return commands.Sum(turtleGraphicsCommand => turtleGraphicsCommand.CountInValidCommands());
        }

        public void ExecuteCommands(List<TurtleGraphicsCommand> commands)
        {
            var program = new TurtleGraphicsCommand { Commands = commands };

            program.Execute(_canceller);
        }

        public List<TurtleGraphicsCommand> ExecuteCommandLine(string commandLine, bool parseOnly,
            bool waitForResult = false)
        {
            var tokens = _textParser.Parse(commandLine);

            var commands = _turtleGraphicsSyntaxAnalyser.ConvertTokensToCommands(tokens);

            _executeWithoutErrors = (CountInValidCommands(commands) == 0);

            if (!parseOnly && _executeWithoutErrors)
            {
                var worker = new BackgroundWorker();
                var waitHandle = new AutoResetEvent(false);
 
                DoWorkEventHandler doWorkHandler = (s, e) =>
                                                       {
                                                           try
                                                           {
                                                               OnStatusChanged(new ExecutionStatusChangedEventArgs{ProgramCountIncreased = true});
                                                              
                                                               ExecuteCommands(commands);
                                                               _turtleGraphicsRuntime.CleanUpAfterExecution();
                                                           }
                                                           finally
                                                           {
                                                               OnStatusChanged(new ExecutionStatusChangedEventArgs { ProgramCountIncreased = false });
                                                               waitHandle.Set();
                                                           }
                                                       };

                RunWorkerCompletedEventHandler runWorkerCompletedEventHandler = (s, e) =>
                            {
                                if (e.Error != null)
                                {
                                    OnException(
                                        new ExecutionEngineErrorEventArgs
                                            {Error = e.Error});
                                }
                            };

                worker.RunWorkerCompleted += runWorkerCompletedEventHandler;
                worker.DoWork += doWorkHandler;
                worker.RunWorkerAsync();

                // Force the method to simulate being run synchronously if specified, useful to avoid race
                // conditions in unit tests
                if(waitForResult)
                {
                   waitHandle.WaitOne();
                }
            }

            return commands;
        }
    }
}

