using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lynx
{
    public class AssertException : Exception
    {
        public AssertException() : base()
        {
        }

        public AssertException(string? message) : base(message)
        {
        }

        public AssertException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AssertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
