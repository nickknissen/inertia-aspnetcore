using System;

namespace InertiaAdapter.Exceptions
{
    internal class KeyDoesNotExistException : Exception
    {
        public KeyDoesNotExistException()
        {
        }

        public KeyDoesNotExistException(string message)
            : base(message)
        {
        }

        public KeyDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
