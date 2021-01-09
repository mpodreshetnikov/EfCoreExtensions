using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCoreExtensions.Utils.Specific
{
    internal static class OrderingReflectionUtils
    {
        private class OrderingTester : ExpressionVisitor
        {
            public bool orderingMethodFound;

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var name = node.Method.Name;

                if (node.Method.DeclaringType == typeof(Queryable) && (
                    name.StartsWith(nameof(Queryable.OrderBy), StringComparison.Ordinal)))
                {
                    orderingMethodFound = true;
                }

                return base.VisitMethodCall(node);
            }
        }

        public static bool IsQueryOrdered(IQueryable query)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));

            var visitor = new OrderingTester();
            visitor.Visit(query.Expression);
            return visitor.orderingMethodFound;
        }

        public static IQueryable<T> OrderByAnything<T>(IQueryable<T> query)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));

            var propertyName = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault()?.Name;
            if (propertyName is null)
            {
                try
                {
                    return OrderByItself(query);
                }
                catch (InvalidOperationException e)
                {
                    throw new InvalidOperationException($"Type {typeof(T).FullName} doesn't have public properties so cannot be ordered automatically. Order the query before using pagination.", e);
                }
            }
            return OrderByPropertyOrField(query, propertyName);
        }

        public static IQueryable<T> OrderByItself<T>(IQueryable<T> query, bool ascending = true)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));

            if (!typeof(T).IsPrimitive)
            {
                throw new InvalidOperationException("Type must be primitive.");
            }

            var parameterExpression = Expression.Parameter(typeof(T));
            var orderingMemberAccessorExpression = Expression.Lambda(parameterExpression, parameterExpression);
            return Order(query, orderingMemberAccessorExpression, typeof(T), ascending);
        }

        public static IQueryable<T> OrderByPropertyOrField<T>(IQueryable<T> query, string fieldOrProperty, bool ascending = true)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));
            ArgumentUtils.MustBeNotNull(fieldOrProperty, nameof(fieldOrProperty));

            var parameterExpression = Expression.Parameter(typeof(T));
            var memberAccessExpression = Expression.PropertyOrField(parameterExpression, fieldOrProperty);
            var orderingMemberAccessorExpression = Expression.Lambda(memberAccessExpression, parameterExpression);
            return Order(query, orderingMemberAccessorExpression, memberAccessExpression.Type, ascending);
        }

        private static IQueryable<T> Order<T>(IQueryable<T> query, LambdaExpression orderingMemberAccessorExpression, Type memberType, bool ascending)
        {
            var parameterExpression = Expression.Parameter(typeof(IQueryable<T>));
            var orderingMemberAccessorQuoteExpression = Expression.Quote(orderingMemberAccessorExpression);

            var methodName = ascending ? nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending);
            var genericMethodInfo = typeof(Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == methodName && m.GetParameters().Length == 2);
            var methodInfo = genericMethodInfo.MakeGenericMethod(typeof(T), memberType);

            var callOrderByMethodExpression = Expression.Call(methodInfo,
                parameterExpression, orderingMemberAccessorQuoteExpression);

            var methodCallExpression = Expression.Lambda(callOrderByMethodExpression, parameterExpression);
            return (IQueryable<T>)methodCallExpression.Compile().DynamicInvoke(query);
        }
    }
}
