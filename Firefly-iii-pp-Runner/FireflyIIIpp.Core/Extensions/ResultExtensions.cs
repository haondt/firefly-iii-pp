using Haondt.Core.Models;

namespace FireflyIIIpp.Core.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T, TReason> Fail<T, TReason>(TReason reason) => new(reason);
        public static Result<T, TReason> Success<T, TReason>(TReason reason) => new(reason);
    }
}
