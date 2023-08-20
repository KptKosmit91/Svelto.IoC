using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.Context
{
    public class ContextException : Exception
    {
        public ContextException() 
        {
        }

        public ContextException(string message) : base(message)
        {
        }

        public ContextException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
