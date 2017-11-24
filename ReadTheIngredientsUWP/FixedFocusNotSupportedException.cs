using System;

namespace ReadTheIngredientsUWP
{
    internal class FixedFocusNotSupportedException : Exception
    {
        public FixedFocusNotSupportedException()
        {
        }

        public FixedFocusNotSupportedException(string message) : base(message)
        {
        }

        public FixedFocusNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}