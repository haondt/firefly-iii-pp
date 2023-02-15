using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Exceptions
{
    public class BusyException : Exception
    {
        public BusyException() : base() { }
        public BusyException(string message) : base(message) { }
        public BusyException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
