using System.Linq;
using EfCoreExtensions.Utils;
using EfCoreExtensions.Utils.Specific;

namespace EfCoreExtensions.Ordering
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string orderQuery, IOrderQueryParser orderQueryParser = null)
        {
            ArgumentUtils.DefaultIfNull(ref orderQueryParser, new DefaultOrderQueryParser());

            var orderQueryObject = orderQueryParser.ParseOrderQuery(orderQuery);
            if (OrderingReflectionUtils.ApplyAndAcceptOrderQueryFor<T>(orderQueryObject))
            {
                return OrderingReflectionUtils.OrderByPropertyOrField(query, orderQueryObject.PropertyName, orderQueryObject.IsAscending);
            }
            return OrderingReflectionUtils.OrderByAnything(query);
        }
    }
}
