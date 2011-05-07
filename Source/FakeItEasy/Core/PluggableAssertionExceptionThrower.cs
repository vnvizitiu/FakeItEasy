namespace FakeItEasy.Core
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class PluggableAssertionExceptionThrower
        : IAssertionExceptionThrower
    {
        private readonly IEnumerable<IAssertionExceptionResolver> resolvers;
        private Func<string, Exception> exceptionFactory;
        
        public PluggableAssertionExceptionThrower(IEnumerable<IAssertionExceptionResolver> resolvers)
        {
            this.resolvers = resolvers;
            
            this.exceptionFactory = x =>
            {
                this.InitializeExceptionFactory();
                return this.exceptionFactory.Invoke(x);
            };
        }

        public void ThrowAssertionException(string message)
        {
            throw this.exceptionFactory.Invoke(message);
        }

        private void InitializeExceptionFactory()
        {
            foreach (var resolver in this.resolvers)
            {
                if (resolver != null)
                {
                    var newFactory = resolver.TryCreateExceptionFactory();

                    if (newFactory != null)
                    {
                        this.exceptionFactory = resolver.TryCreateExceptionFactory();
                        return;
                    }
                }
            }

            this.exceptionFactory = x => new ExpectationException(x);
        }
    }
}