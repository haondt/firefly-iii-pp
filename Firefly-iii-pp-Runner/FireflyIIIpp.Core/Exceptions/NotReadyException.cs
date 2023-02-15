using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Exceptions
{
    public class NotReadyException : Exception
    {
        public NotReadyException() : base() { }
        public NotReadyException(string message) : base(message) { }
        public NotReadyException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
