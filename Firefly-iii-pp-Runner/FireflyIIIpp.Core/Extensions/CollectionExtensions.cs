using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireflyIIIpp.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static int Pages<T>(this ICollection<T> items, int pageSize)
        {
            return (items.Count - 1) / pageSize + 1;
        }

        public static ICollection<IEnumerable<T>> Paginate<T>(this ICollection<T> items, int pageSize)
        {
            if (items.Count == 0)
                return new List<IEnumerable<T>>();

            var pages = items.Pages(pageSize);
            var paginated = new List<IEnumerable<T>>();
            var page = 0;
            while(page < pages)
            {
                page++;
                paginated.Add(items.Page(pageSize, page));
            }

            return paginated;
        }
    }
}
