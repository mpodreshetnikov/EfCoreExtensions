using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EfCoreExtension.Utils
{
    internal static class ReflectionUtils
    {
        public struct MethodReplacementState : IDisposable
        {
            internal IntPtr Location;
            internal IntPtr OriginalValue;

            public void Dispose()
            {
                Restore();
            }

            public unsafe void Restore()
            {
#if DEBUG
                *(int*)Location = (int)OriginalValue;
#else
                *(IntPtr*)Location = OriginalValue;
#endif
            }
        }

        public static unsafe MethodReplacementState Replace(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            MethodReplacementState state;

            IntPtr tar = methodToReplace.MethodHandle.Value;
            if (!methodToReplace.IsVirtual)
            {
                tar += 8;
            }
            else
            {
                var index = (int)(((*(long*)tar) >> 32) & 0xFF);
                var classStart = *(IntPtr*)(methodToReplace.DeclaringType.TypeHandle.Value + (IntPtr.Size == 4 ? 40 : 64));
                tar = classStart + IntPtr.Size * index;
            }
            var inj = methodToInject.MethodHandle.Value + 8;
#if DEBUG
            tar = *(IntPtr*)tar + 1;
            inj = *(IntPtr*)inj + 1;
            state.Location = tar;
            state.OriginalValue = new IntPtr(*(int*)tar);

            *(int*)tar = *(int*)inj + (int)(long)inj - (int)(long)tar;
            return state;
#else
            state.Location = tar;
            state.OriginalValue = *(IntPtr*)tar;
            * (IntPtr*)tar = *(IntPtr*)inj;
            return state;
#endif
        }

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
