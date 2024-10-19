using Haondt.Core.Models;

namespace FireflyIIIpp.Core.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<Result<string>> EnsureDownstreamSuccessStatusCode(this HttpResponseMessage result, string downstreamServiceName)
        {
            if (!result.IsSuccessStatusCode)
            {
                var content = "null";
                if (result.Content != null)
                    content = await result.Content.ReadAsStringAsync() ?? "null";
                return new($"{downstreamServiceName} returned status: {result.StatusCode} with content: {content}");
            }
            return new();
        }
    }
}
