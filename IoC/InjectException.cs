using System;

namespace Svelto.IoC
{
    public class InjectException : Exception
    {
        public InjectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal InjectException(string message) : base(message)
        {
        }
    }
}
