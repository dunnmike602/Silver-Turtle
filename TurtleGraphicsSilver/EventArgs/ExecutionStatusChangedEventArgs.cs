using System;

namespace TurtleGraphics.EventArguments
{
    public class ExecutionStatusChangedEventArgs : EventArgs
    {
        public bool ProgramCountIncreased { get; set; }
    }
}