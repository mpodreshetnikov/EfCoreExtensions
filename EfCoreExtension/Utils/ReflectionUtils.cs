using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EfCoreExtensions.Utils
{
    internal static class ReflectionUtils
    {
        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Given a lambda expression that calls a method, returns the method info.
        /// </summary>
        public static MethodInfo GetMethodInfo(LambdaExpression expression)
        {
            if (!(expression.Body is MethodCallExpression outermostExpression))
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }
    }
}
