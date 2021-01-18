using System;

namespace InertiaAdapter.Exceptions
{
    public class DuplicateKeysException : Exception
    {
        public DuplicateKeysException()
        {
        }

        public DuplicateKeysException(string message)
            : base(message)
        {
        }

        public DuplicateKeysException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
