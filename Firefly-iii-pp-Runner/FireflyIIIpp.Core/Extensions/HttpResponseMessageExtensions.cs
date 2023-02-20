using FireflyIIIpp.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<HttpResponseMessage> EnsureDownstreamSuccessStatusCode(this HttpResponseMessage result, string downstreamServiceName)
        {
            if (!result.IsSuccessStatusCode)
            {
                var content = "null";
                if (result?.Content != null)
                    content = await result.Content.ReadAsStringAsync() ?? "null";
                throw new DownstreamException($"{downstreamServiceName} returned status: {result.StatusCode} with content: {content}");
            }

            return result;
        }
    }
}
