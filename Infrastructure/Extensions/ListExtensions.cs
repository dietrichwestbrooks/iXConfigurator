using System.Collections.Generic;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions
{
    public static class ListExtensions
    {
        public static IList<T> AddRange<T>(this IList<T> source, IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                source.Add(item);
            }

            return source;
        } 
    }
}
