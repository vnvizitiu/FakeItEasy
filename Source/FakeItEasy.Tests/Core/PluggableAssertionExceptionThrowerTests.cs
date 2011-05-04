namespace FakeItEasy.Tests.Core
{
    using System.Collections.Generic;
    using FakeItEasy.Core;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class PluggableAssertionExceptionThrowerTests
    {
        private List<IAssertionExceptionResolver> resolvers;
        private PluggableAssertionExceptionThrower exceptionThrower;

        [SetUp]
        public void SetUp()
        {
            this.resolvers = new List<IAssertionExceptionResolver>();
            this.exceptionThrower = new PluggableAssertionExceptionThrower(this.resolvers);
        }

        [Test]
        public void Should_throw_expectation_exception_with_correct_message_when_no_factory_has_been_plugged_in()
        {
            // Arrange

            // Act, Assert
            Assert.That(() =>
            {
                this.exceptionThrower.ThrowAssertionException("foo");
            },
            Throws.Exception.InstanceOf<ExpectationException>().With.Message.EqualTo("foo"));
        }

        [Test]
        public void Should_throw_exception_produced_by_resolver()
        {
            // Arrange
            var expectedException = new Exception();

            var resolver = A.Fake<IAssertionExceptionResolver>();
            A.CallTo(() => resolver.TryCreateExceptionFactory()).Returns(_ => expectedException);

            this.resolvers.Add(resolver);

            // Act, Assert
            Assert.That(() => this.exceptionThrower.ThrowAssertionException("foo"),
                Throws.Exception.SameAs(expectedException));
        }

        [Test]
        public void Should_throw_expectation_exception_when_resolver_returns_null()
        {
            // Arrange
            var resolver = A.Fake<IAssertionExceptionResolver>();
            A.CallTo(() => resolver.TryCreateExceptionFactory()).Returns(null);

            this.resolvers.Add(resolver);

            // Act

            // Assert
            Assert.That(() =>
            {
                this.exceptionThrower.ThrowAssertionException("foo");
            },
           Throws.Exception.InstanceOf<ExpectationException>().With.Message.EqualTo("foo"));
        }

        [Test]
        public void Should_throw_exception_produced_by_resolver_that_is_not_the_first()
        {
            // Arrange
            var resolverThatReturnsNull = A.Fake<IAssertionExceptionResolver>();
            A.CallTo(() => resolverThatReturnsNull.TryCreateExceptionFactory()).Returns(null);

            var expectedException = new Exception();

            var resolver = A.Fake<IAssertionExceptionResolver>();
            A.CallTo(() => resolver.TryCreateExceptionFactory()).Returns(_ => expectedException);

            this.resolvers.Add(resolverThatReturnsNull);
            this.resolvers.Add(resolver);

            // Act, Assert
            Assert.That(() => this.exceptionThrower.ThrowAssertionException("foo"),
                Throws.Exception.SameAs(expectedException));
        }

        [Test]
        public void Should_use_factory_from_first_resolver_that_returns()
        {
            // Arrange
            var expectedException = new Exception();

            var first = A.Fake<IAssertionExceptionResolver>();
            A.CallTo(() => first.TryCreateExceptionFactory()).Returns(_ => expectedException);

            var second = A.Fake<IAssertionExceptionResolver>();
            A.CallTo(() => second.TryCreateExceptionFactory()).Returns(_ => new Exception());

            this.resolvers.Add(first);
            this.resolvers.Add(second);

            // Act, Assert
            Assert.That(() => this.exceptionThrower.ThrowAssertionException("foo"),
                Throws.Exception.SameAs(expectedException));
        }
    }
}