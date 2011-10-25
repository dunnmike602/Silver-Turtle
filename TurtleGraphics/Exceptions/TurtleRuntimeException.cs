using System;

namespace TurtleGraphics.Exceptions
{
    public sealed class TurtleRuntimeException : Exception
    {
        public TurtleRuntimeException()
        {
        }

        public TurtleRuntimeException(string message)
            : base(message)
        {
        }

        public TurtleRuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}