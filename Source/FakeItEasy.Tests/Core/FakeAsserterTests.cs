namespace FakeItEasy.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy.Core;
    using NUnit.Framework;

    [TestFixture]
    public class FakeAsserterTests
    {
        private List<IFakeObjectCall> calls;
        private CallWriter callWriter;
        private IAssertionExceptionThrower exceptionThrower;

        [SetUp]
        public void SetUp()
        {
            this.calls = new List<IFakeObjectCall>();
            this.callWriter = A.Fake<CallWriter>();
            this.exceptionThrower = A.Fake<IAssertionExceptionThrower>();
        }

        private FakeAsserter CreateAsserter()
        {
            return new FakeAsserter(this.calls, this.callWriter, this.exceptionThrower);
        }

        private void StubCalls(int numberOfCalls)
        {
            for (int i = 0; i < numberOfCalls; i++)
            {
                this.calls.Add(A.Fake<IFakeObjectCall>());
            }
        }

        [Test]
        public void AssertWasCalled_should_pass_when_the_repeatPredicate_returns_true_for_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => true, "", x => x == 2, "");
        }

        [Test]
        public void AssertWasCalled_should_fail_when_the_repeatPredicate_returns_false_fro_the_number_of_matching_calls()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();
            asserter.AssertWasCalled(x => true, "", x => false, "");
            
            A.CallTo(() => this.exceptionThrower.ThrowAssertionException(A<string>._)).MustHaveHappened();
        }

        [Test]
        public void AssertWasCalled_should_pass_the_number_of_matching_calls_to_the_repeatPredicate()
        {
            int? numberPassedToRepeatPredicate = null;

            this.StubCalls(4);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => this.calls.IndexOf(x) == 0, "",  x => { numberPassedToRepeatPredicate = x; return true; }, "");

            Assert.That(numberPassedToRepeatPredicate, Is.EqualTo(1));
        }

        [Test]
        public void Exception_message_should_start_with_call_specification()
        {
            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => true, @"IFoo.Bar(1)", x => false, "");

            A.CallTo(() => this.exceptionThrower.ThrowAssertionException(A<string>.That.StartsWith(@"

  Assertion failed for the following call:
    IFoo.Bar(1)"))).MustHaveHappened();
        }

        [Test]
        public void Exception_message_should_write_repeat_expectation()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => false, "", x => x == 2, "#2 times");

            A.CallTo(() => this.exceptionThrower.ThrowAssertionException(A<string>.That.Contains(@"
  Expected to find it #2 times but found it #0 times among the calls:"))).MustHaveHappened();
        }

        [Test]
        public void Exception_message_should_call_the_call_writer_to_append_calls_list()
        {
            this.StubCalls(2);

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => false, "", x => false, "");

            A.CallTo(() => this.callWriter.WriteCalls(A<IEnumerable<IFakeObjectCall>>.That.IsThisSequence(this.calls), A<IOutputWriter>._)).MustHaveHappened();
        }

        [Test]
        public void Exception_message_should_write_that_no_calls_were_made_when_calls_is_empty()
        {
            this.calls.Clear();

            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => false, "", x => x == 2, "#2 times");

            A.CallTo(() => this.exceptionThrower.ThrowAssertionException(A<string>.That.Contains(@"
  Expected to find it #2 times but no calls were made to the fake object."))).MustHaveHappened();
        }

        [Test]
        public void Exception_message_should_end_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => false, "", x => false, "");

            A.CallTo(() => this.exceptionThrower.ThrowAssertionException(A<string>.That.Matches(x => x.EndsWith(string.Concat(Environment.NewLine, Environment.NewLine)), "ends with double blank line"))).MustHaveHappened();
        }

        [Test]
        public void Exception_message_should_start_with_blank_line()
        {
            var asserter = this.CreateAsserter();

            asserter.AssertWasCalled(x => false, "", x => false, "");

            A.CallTo(() => this.exceptionThrower.ThrowAssertionException(A<string>.That.StartsWith(Environment.NewLine))).MustHaveHappened();
        }
    }
}