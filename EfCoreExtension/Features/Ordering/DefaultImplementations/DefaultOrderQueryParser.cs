using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreExtensions.Utils;

namespace EfCoreExtensions.Ordering
{
    public class DefaultOrderQueryParser : IOrderQueryParser
    {
        private readonly IReadOnlyList<char> splitters = new[] { ':', ' ' };
        private readonly IReadOnlyList<string> ascendingStrings = new[] { "asc", "ascending" };
        private readonly IReadOnlyList<string> descendingStrings = new[] { "desc", "descending" };

        public DefaultOrderQueryParser() { }
        public DefaultOrderQueryParser(IEnumerable<char> splitters)
        {
            ArgumentUtils.MustBeNotNull(splitters, nameof(splitters));
            this.splitters = (IReadOnlyList<char>)splitters;
        }

        public OrderQuery ParseOrderQuery(string orderQuery)
        {
            if (orderQuery is null)
            {
                return new OrderQuery();
            }
            orderQuery = orderQuery.Trim();

            var splitted = orderQuery.Split(splitters.ToArray());
            if (splitted.Length > 2)
            {
                throw new ArgumentException($"{orderQuery} is incorrect order query.", nameof(orderQuery));
            }
            if (splitted.Length == 1)
            {
                var value = splitted[0];
                return new OrderQuery
                {
                    PropertyName = value,
                };
            }
            if (splitted.Length == 2)
            {
                var firstValue = splitted[0];
                var secondValue = splitted[1];

                if (ascendingStrings.Contains(firstValue.ToLowerInvariant()))
                {
                    return new OrderQuery
                    {
                        PropertyName = secondValue,
                        IsAscending = true,
                    };
                }
                if (descendingStrings.Contains(firstValue.ToLowerInvariant()))
                {
                    return new OrderQuery
                    {
                        PropertyName = secondValue,
                        IsAscending = false,
                    };
                }

                if (ascendingStrings.Contains(secondValue.ToLowerInvariant()))
                {
                    return new OrderQuery
                    {
                        PropertyName = firstValue,
                        IsAscending = true,
                    };
                }
                if (descendingStrings.Contains(secondValue.ToLowerInvariant()))
                {
                    return new OrderQuery
                    {
                        PropertyName = firstValue,
                        IsAscending = false,
                    };
                }

                if (string.IsNullOrWhiteSpace(firstValue))
                {
                    return new OrderQuery
                    {
                        PropertyName = secondValue,
                    };
                }
                if (string.IsNullOrWhiteSpace(secondValue))
                {
                    return new OrderQuery
                    {
                        PropertyName = firstValue,
                    };
                }
            }
            return new OrderQuery();
        }
    }
}
