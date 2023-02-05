using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Exceptions
{
    public class DownstreamException : Exception
    {
        public DownstreamException() : base() { }
        public DownstreamException(string message) : base(message) { }
        public DownstreamException(string? message, Exception? innerException) : base(message, innerException) { }

    }
}
