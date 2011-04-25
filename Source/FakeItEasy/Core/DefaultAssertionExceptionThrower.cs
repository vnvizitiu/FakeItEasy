namespace FakeItEasy.Core
{
    internal class DefaultAssertionExceptionThrower
        : IAssertionExceptionThrower
    {
        public void ThrowAssertionException(string message)
        {
            throw new ExpectationException(message);
        }
    }
}