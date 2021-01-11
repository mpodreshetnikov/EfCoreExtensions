using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EfCoreExtensions.Utils;

namespace EfCoreExtensions.Searching
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Search by concatenated user properties with spaces. Don't use with encrypted properties. Skip nulls when build a search string.
        /// </summary>
        public static IQueryable<T> SearchInTextProps<T>(this IQueryable<T> query, string searchingQuery, params Expression<Func<T, string>>[] props)
            => SearchInTextProps(query, searchingQuery, false, props);

        /// <summary>
        /// Search by concatenated user properties with spaces. Don't use with encrypted properties. Skip nulls when build a search string.
        /// </summary>
        public static IQueryable<T> SearchInTextProps<T>(this IQueryable<T> query, string searchingQuery, bool isCaseSensitive = false, params Expression<Func<T, string>>[] props)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));
            ArgumentUtils.MustBeNotNull(searchingQuery, nameof(searchingQuery));
            ArgumentUtils.AllMustBeNotNull(props, nameof(props));

            searchingQuery = searchingQuery.Trim();
            if (!isCaseSensitive)
            {
                searchingQuery = searchingQuery.ToLower();
            }

            // Create lambda for Where clause.
            var parameter = Expression.Parameter(typeof(T));

            // Build search string.
            var concatMethodInfo = typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });

            var expressionsToConcat = new List<Expression>();
            foreach (MemberExpression prop in props.Select(p => p.Body))
            {
                var member = prop.Member;
                // Check on null and build search substring for a property.
                Expression valueExpression = Expression.MakeMemberAccess(parameter, member);
                if (member.ReflectedType.IsClass)
                {
                    var isNullExpression = Expression.Equal(valueExpression, Expression.Constant(null));
                    valueExpression = Expression.Condition(
                        isNullExpression,
                        Expression.Constant(string.Empty),
                        Expression.Add(valueExpression, Expression.Constant(" "), concatMethodInfo));
                }
                expressionsToConcat.Add(valueExpression);
            }
            var searchStringExpression = expressionsToConcat.Aggregate((leftExpr, rightExpr) => Expression.Add(leftExpr, rightExpr, concatMethodInfo));
            if (!isCaseSensitive)
            {
                searchStringExpression = Expression.Call(searchStringExpression, nameof(string.ToLower), null);
            }

            // Check if it contains searching query string.
            var searchingQueryExpression = Expression.Constant(searchingQuery);
            var containsCallExpression = Expression.Call(searchStringExpression, nameof(string.Contains), null, searchingQueryExpression);

            // Create Where clause lambda.
            var whereLambda = Expression.Lambda(containsCallExpression, parameter);

            return query.Where((Expression<Func<T, bool>>)whereLambda);
        }
    }
}
