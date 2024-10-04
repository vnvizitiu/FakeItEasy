namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Handles comparisons of instances of <see cref="MethodInfo"/>.
    /// </summary>
    internal class MethodInfoManager
    {
        private static readonly ConcurrentDictionary<TypeMethodInfoPair, MethodInfo?> MethodCache = new ConcurrentDictionary<TypeMethodInfoPair, MethodInfo?>();

        /// <summary>
        /// Gets a value indicating whether the two instances of <see cref="MethodInfo"/> would invoke the same method
        /// if invoked on an instance of the target type.
        /// </summary>
        /// <param name="target">The type of target for invocation.</param>
        /// <param name="first">The first <see cref="MethodInfo"/>.</param>
        /// <param name="second">The second <see cref="MethodInfo"/>.</param>
        /// <returns>True if the same method would be invoked.</returns>
        public virtual bool WillInvokeSameMethodOnTarget(Type target, MethodInfo first, MethodInfo second)
        {
            if (first == second)
            {
                return true;
            }

            var methodInvokedByFirst = this.GetMethodOnTypeThatWillBeInvokedByMethodInfo(target, first);
            var methodInvokedBySecond = this.GetMethodOnTypeThatWillBeInvokedByMethodInfo(target, second);

            return methodInvokedByFirst is not null && methodInvokedBySecond is not null && methodInvokedByFirst.Equals(methodInvokedBySecond);
        }

        public virtual MethodInfo? GetMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            var key = new TypeMethodInfoPair(type, method);

            return MethodCache.GetOrAdd(key, k => FindMethodOnTypeThatWillBeInvokedByMethodInfo(k.Type, k.MethodInfo));
        }

        private static MethodInfo? FindMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            return
                (from typeMethod in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                 where typeMethod.HasSameBaseMethodAs(method)
                 select MakeGeneric(typeMethod, method)).FirstOrDefault()
                ?? GetMethodOnTypeThatImplementsInterfaceMethod(type, method)
                ?? GetMethodOnInterfaceTypeImplementedByMethod(type, method);
        }

        private static MethodInfo? GetMethodOnInterfaceTypeImplementedByMethod(Type type, MethodInfo method)
        {
            var reflectedType = method.ReflectedType!;

            if (reflectedType.IsInterface)
            {
                return null;
            }

            var allInterfaces =
                from i in type.GetInterfaces()
                where TypeImplementsInterface(reflectedType, i)
                select i;

            foreach (var interfaceType in allInterfaces)
            {
                var interfaceMap = reflectedType.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);

                var foundMethod =
                    (from methodTargetPair in interfaceMap.InterfaceMethods
                         .Zip(interfaceMap.TargetMethods, (interfaceMethod, targetMethod) => new { InterfaceMethod = interfaceMethod, TargetMethod = targetMethod })
                     where method.HasSameBaseMethodAs(methodTargetPair.TargetMethod)
                     select MakeGeneric(methodTargetPair.InterfaceMethod, method)).FirstOrDefault();

                if (foundMethod is not null)
                {
                    return GetMethodOnTypeThatImplementsInterfaceMethod(type, foundMethod);
                }
            }

            return null;
        }

        private static MethodInfo? GetMethodOnTypeThatImplementsInterfaceMethod(Type type, MethodInfo method)
        {
            var baseDefinition = method.GetBaseDefinition();

            if (!baseDefinition.DeclaringType!.IsInterface || !TypeImplementsInterface(type, baseDefinition.DeclaringType))
            {
                return null;
            }

            var interfaceMap = type.GetTypeInfo().GetRuntimeInterfaceMap(baseDefinition.DeclaringType);

            return
                (from methodTargetPair in interfaceMap.InterfaceMethods
                     .Zip(interfaceMap.TargetMethods, (interfaceMethod, targetMethod) => new { InterfaceMethod = interfaceMethod, TargetMethod = targetMethod })
                 where methodTargetPair.InterfaceMethod.HasSameBaseMethodAs(method)
                 select MakeGeneric(methodTargetPair.TargetMethod, method)).First();
        }

        private static MethodInfo MakeGeneric(MethodInfo methodToMakeGeneric, MethodInfo originalMethod)
        {
            if (!methodToMakeGeneric.IsGenericMethodDefinition)
            {
                return methodToMakeGeneric;
            }

            return methodToMakeGeneric.MakeGenericMethod(originalMethod.GetGenericArguments());
        }

        private static bool TypeImplementsInterface(Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(x => x.Equals(interfaceType));
        }

        private struct TypeMethodInfoPair : IEquatable<TypeMethodInfoPair>
        {
            public TypeMethodInfoPair(Type type, MethodInfo methodInfo)
                : this()
            {
                this.Type = type;
                this.MethodInfo = methodInfo;
            }

            public MethodInfo MethodInfo { get; }

            public Type Type { get; }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (this.Type.GetHashCode() * 23) + this.MethodInfo.GetHashCode();
                }
            }

            [SuppressMessage("Microsoft.Usage", "CA2231:OverloadOperatorEqualsOnOverridingValueTypeEquals", Justification = "The type is used privately only.")]
            public override bool Equals(object? obj)
            {
                return obj is TypeMethodInfoPair other && this.Equals(other);
            }

            public bool Equals(TypeMethodInfoPair other)
            {
                return this.Type == other.Type && this.MethodInfo == other.MethodInfo;
            }
        }
    }
}
