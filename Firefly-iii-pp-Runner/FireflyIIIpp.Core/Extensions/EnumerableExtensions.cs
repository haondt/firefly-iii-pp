using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Extensions
{
    public static class EnumerableExtensions
    {

        public static IEnumerable<T> Page<T>(this IEnumerable<T> items, int pageSize, int page)
        {
            return items.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
