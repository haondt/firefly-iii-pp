using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() : base() { }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
