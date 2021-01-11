using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace EfCoreExtensions.Utils
{
    internal static class ExpressionUtils
    {
        public static Expression GetNestedMemberOrDefault
            (LambdaExpression memberAccessExpression, object defaultValue, ParameterExpression newParameterExpression = default)
        {
            ArgumentUtils.MustBeNotNull(memberAccessExpression, nameof(memberAccessExpression));

            if (!(memberAccessExpression.Body is MemberExpression castedMemberAccessExpression))
            {
                throw new InvalidOperationException("Provided member access chain is invalid.");
            }

            var memberAccessors = new List<MemberExpression>();
            Expression expression = castedMemberAccessExpression;
            while (expression is MemberExpression castedExpression)
            {
                memberAccessors.Add(castedExpression);
                expression = castedExpression.Expression;
            }
            memberAccessors.Reverse();

            var rootExpression = newParameterExpression
                ?? (expression is ParameterExpression
                    ? expression
                    : throw new InvalidOperationException("Provided member access chain is invalid."));

            return MakeConditionalExpression(rootExpression, defaultValue, memberAccessors);
        }

        private static Expression MakeConditionalExpression(Expression expression, object defaultValue, IEnumerable<MemberExpression> memberExpressions)
        {
            if (!memberExpressions.Any())
            {
                return expression;
            }

            var testIsNullExpression = Expression.Equal(expression, Expression.Constant(null));
            var ifTrueExpression = Expression.Constant(defaultValue);
            var ifFalseExpression = MakeConditionalExpression(
                Expression.MakeMemberAccess(expression, memberExpressions.First().Member),
                defaultValue,
                memberExpressions.Skip(1));
            return Expression.Condition(testIsNullExpression, ifTrueExpression, ifFalseExpression);
        }
    }
}
