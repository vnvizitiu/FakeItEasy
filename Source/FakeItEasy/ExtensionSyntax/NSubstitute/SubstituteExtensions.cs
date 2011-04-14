using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSubstitute
{
    using FakeItEasy;
    using FakeItEasy.Creation;
using FakeItEasy.Core;

    public static class SubstituteExtensions
    {
        public static void Returns<T>(this T value, Func<T> returnThis)
        {
            var call = Substitute.LastTrappedCall;
            //call.Item1.AddRuleFirst();
            A.CallTo(call.Item1.Object).Where(x => x.Method.Equals(call.Item2.Method)).WithReturnType<T>().Returns(returnThis.Invoke());
        }
    }

    public static class Substitute
    {
        public static Tuple<FakeManager, IWritableFakeObjectCall> LastTrappedCall;

        public static T For<T>()
        {
            var result = A.Fake<T>();
            var manager = Fake.GetFakeManager(result);
            manager.CallWasIntercepted += (sender, e) => LastTrappedCall = Tuple.Create(manager, e.Call);

            return (T) manager.Object;
        }
    }
}
