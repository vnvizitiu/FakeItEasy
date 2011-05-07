namespace FakeItEasy.Core
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    internal class NUnitAssertionExceptionResolver
        : IAssertionExceptionResolver
    {
        public Func<string, Exception> TryCreateExceptionFactory()
        {
            var trace = new StackTrace();

            var isNunitRunner =
                (from frame in trace.GetFrames()
                 let method = frame.GetMethod()
                 where method.DeclaringType.Assembly.FullName.EndsWith("PublicKeyToken=96d09a1eb7f44a77")
                 select method).FirstOrDefault() != null;

            if (!isNunitRunner)
            {
                return null;
            }

            try
            {
                var assertionException = Type.GetType("NUnit.Framework.AssertionException, NUnit.Framework");

                if (assertionException == null)
                {
                    return null;
                }

                return x => (Exception)Activator.CreateInstance(assertionException, new object[] { x });

            }
            catch
            {
                return null;
            }
        }
    }
}