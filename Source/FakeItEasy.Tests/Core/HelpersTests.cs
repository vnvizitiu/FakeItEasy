﻿namespace FakeItEasy.Tests.Core
{
    using NUnit.Framework;

    [TestFixture]
    public class HelpersTests : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void GetDescription_should_render_method_name_and_empty_arguments_list_when_call_has_no_arguments()
        {
            var call = FakeCall.Create<object>("GetType");

            Assert.That(call.GetDescription(), Is.EqualTo("System.Object.GetType()"));
        }

        [Test]
        public void GetDescription_should_render_method_name_and_all_arguments_when_call_has_arguments()
        {
            var call = CreateFakeCallToFooDotBar("abc", 123);

            Assert.That(call.GetDescription(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(\"abc\", 123)"));
        }

        [Test]
        public void GetDescription_should_render_NULL_when_argument_is_null()
        {
            var call = CreateFakeCallToFooDotBar(null, 123);

            Assert.That(call.GetDescription(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(<NULL>, 123)"));
        }

        [Test]
        public void GetDescription_should_render_string_empty_when_string_is_empty()
        {
            var call = CreateFakeCallToFooDotBar(string.Empty, 123);

            Assert.That(call.GetDescription(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(<string.Empty>, 123)"));
        }

        private static FakeCall CreateFakeCallToFooDotBar(object argument1, object argument2)
        {
            return FakeCall.Create<IFoo>("Bar", new[] { typeof(object), typeof(object) }, new[] { argument1, argument2 });
        }
    }
}
