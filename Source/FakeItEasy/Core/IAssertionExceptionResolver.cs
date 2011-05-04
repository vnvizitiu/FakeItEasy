namespace FakeItEasy.Core
{
    using System;

    public interface IAssertionExceptionResolver
    {
        Func<string, Exception> TryCreateExceptionFactory();
    }
}