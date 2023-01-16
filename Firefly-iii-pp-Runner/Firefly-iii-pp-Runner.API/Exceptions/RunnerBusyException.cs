namespace Firefly_iii_pp_Runner.API.Exceptions
{
    public class RunnerBusyException : Exception
    {
        public RunnerBusyException() : base() { }
        public RunnerBusyException(string message) : base(message) { }
        public RunnerBusyException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
