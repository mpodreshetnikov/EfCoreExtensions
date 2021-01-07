using System;
using System.Linq;
using System.Linq.Expressions;

namespace EfCoreExtensions.Utils.Specific
{
    internal class OrderingTester : ExpressionVisitor
    {
        private bool orderingMethodFound;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var name = node.Method.Name;

            if (node.Method.DeclaringType == typeof(Queryable) && (
                name.StartsWith("OrderBy", StringComparison.Ordinal)))
            {
                orderingMethodFound = true;
            }

            return base.VisitMethodCall(node);
        }

        public static bool IsQueryOrdered(IQueryable query)
        {
            ArgumentUtils.MustBeNotNull(query, nameof(query));

            var visitor = new OrderingTester();
            visitor.Visit(query.Expression);
            return visitor.orderingMethodFound;
        }
    }
}
