namespace FakeItEasy.Core
{
    /// <summary>
    /// Responsible for throwing assertion exceptions.
    /// </summary>
    public interface IAssertionExceptionThrower
    {
        /// <summary>
        /// Throws an exception with the specified message, the type
        /// of exception varies depending on the test runner.
        /// </summary>
        /// <param name="message">The message of the exception to throw.</param>
        void ThrowAssertionException(string message);
    }
}