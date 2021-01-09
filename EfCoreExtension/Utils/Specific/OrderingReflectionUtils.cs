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

        public static IQueryable<T> OrderByAnyPropertyOrItself<T>(IQueryable<T> query)
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
            return OrderByFieldOrProperty(query, propertyName);
        }

        public static IQueryable<T> OrderByItself<T>(IQueryable<T> query)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));

            if (!typeof(T).IsPrimitive)
            {
                throw new InvalidOperationException("Type must be primitive.");
            }

            var parameterExpression = Expression.Parameter(typeof(T));
            var orderingMemberAccessorExpression = Expression.Lambda(parameterExpression, parameterExpression);
            return Order(query, orderingMemberAccessorExpression, typeof(T));
        }

        public static IQueryable<T> OrderByFieldOrProperty<T>(IQueryable<T> query, string fieldOrProperty)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));
            ArgumentUtils.MustBeNotNull(fieldOrProperty, nameof(fieldOrProperty));

            var parameterExpression = Expression.Parameter(typeof(T));
            var memberAccessExpression = Expression.PropertyOrField(parameterExpression, fieldOrProperty);
            var orderingMemberAccessorExpression = Expression.Lambda(memberAccessExpression, parameterExpression);
            return Order(query, orderingMemberAccessorExpression, memberAccessExpression.Type);
        }

        private static IQueryable<T> Order<T>(IQueryable<T> query, LambdaExpression orderingMemberAccessorExpression, Type memberType)
        {
            var parameterExpression = Expression.Parameter(typeof(IQueryable<T>));
            var orderingMemberAccessorQuoteExpression = Expression.Quote(orderingMemberAccessorExpression);

            var genericMethodInfo = typeof(Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);
            var methodInfo = genericMethodInfo.MakeGenericMethod(typeof(T), memberType);

            var callOrderByMethodExpression = Expression.Call(methodInfo,
                parameterExpression, orderingMemberAccessorQuoteExpression);

            var methodCallExpression = Expression.Lambda(callOrderByMethodExpression, parameterExpression);
            return (IQueryable<T>)methodCallExpression.Compile().DynamicInvoke(query);
        }
    }
}
