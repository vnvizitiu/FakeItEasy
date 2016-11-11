namespace FakeItEasy.Configuration
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Core;

    internal class RuleBuilder
        : IVoidArgumentValidationConfiguration,
          IAfterCallSpecifiedWithOutAndRefParametersConfiguration
    {
        private readonly FakeAsserter.Factory asserterFactory;
        private readonly FakeManager manager;
        private bool wasRuleAdded;

        internal RuleBuilder(BuildableCallRule ruleBeingBuilt, FakeManager manager, FakeAsserter.Factory asserterFactory)
        {
            this.RuleBeingBuilt = ruleBeingBuilt;
            this.manager = manager;
            this.asserterFactory = asserterFactory;
        }

        /// <summary>
        /// Represents a delegate that creates a configuration object from
        /// a fake object and the rule to build.
        /// </summary>
        /// <param name="ruleBeingBuilt">The rule that's being built.</param>
        /// <param name="fakeObject">The fake object the rule is for.</param>
        /// <returns>A configuration object.</returns>
        internal delegate RuleBuilder Factory(BuildableCallRule ruleBeingBuilt, FakeManager fakeObject);

        public BuildableCallRule RuleBeingBuilt { get; }

        public IEnumerable<ICompletedFakeObjectCall> Calls => this.manager.GetRecordedCalls();

        public ICallMatcher Matcher => new RuleMatcher(this);

        public void NumberOfTimes(int numberOfTimesToRepeat)
        {
            if (numberOfTimesToRepeat <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfTimesToRepeat),
                    numberOfTimesToRepeat,
                    "The number of times to repeat is not greater than zero.");
            }

            this.RuleBeingBuilt.NumberOfTimesToCall = numberOfTimesToRepeat;
        }

        public virtual IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
        {
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.UseApplicator(x => { throw exceptionFactory(x); });
            return this;
        }

        public IVoidConfiguration WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            Guard.AgainstNull(argumentsPredicate, nameof(argumentsPredicate));

            this.RuleBeingBuilt.UsePredicateToValidateArguments(argumentsPredicate);
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration DoesNothing()
        {
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.UseDefaultApplicator();
            return this;
        }

        public virtual IVoidConfiguration Invokes(Action<IFakeObjectCall> action)
        {
            Guard.AgainstNull(action, nameof(action));
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.Actions.Add(action);
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration CallsBaseMethod()
        {
            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.UseApplicator(x => { });
            this.RuleBeingBuilt.CallBaseMethod = true;
            return this;
        }

        public virtual IAfterCallSpecifiedConfiguration AssignsOutAndRefParametersLazily(Func<IFakeObjectCall, ICollection<object>> valueProducer)
        {
            Guard.AgainstNull(valueProducer, nameof(valueProducer));

            this.AddRuleIfNeeded();
            this.RuleBeingBuilt.OutAndRefParametersValueProducer = valueProducer;

            return this;
        }

        public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
        {
            Guard.AgainstNull(repeatConstraint, nameof(repeatConstraint));

            var asserter = this.asserterFactory.Invoke(this.Calls);

            var description = new StringBuilderOutputWriter();
            this.RuleBeingBuilt.WriteDescriptionOfValidCall(description);

            asserter.AssertWasCalled(this.Matcher.Matches, description.Builder.ToString(), repeatConstraint.Matches, repeatConstraint.ToString());

            return new UnorderedCallAssertion(this.manager, this.Matcher, description.Builder.ToString(), repeatConstraint);
        }

        private void AddRuleIfNeeded()
        {
            if (!this.wasRuleAdded)
            {
                this.manager.AddRuleFirst(this.RuleBeingBuilt);
                this.wasRuleAdded = true;
            }
        }

        public class ReturnValueConfiguration<TMember>
            : IAnyCallConfigurationWithReturnTypeSpecified<TMember>
        {
            public ReturnValueConfiguration(RuleBuilder parentConfiguration)
            {
                this.ParentConfiguration = parentConfiguration;
            }

            public RuleBuilder ParentConfiguration { get; }

            public ICallMatcher Matcher => this.ParentConfiguration.Matcher;

            public IEnumerable<ICompletedFakeObjectCall> Calls => this.ParentConfiguration.Calls;

            public IAfterCallSpecifiedConfiguration Throws(Func<IFakeObjectCall, Exception> exceptionFactory)
            {
                return this.ParentConfiguration.Throws(exceptionFactory);
            }

            public IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsLazily(Func<IFakeObjectCall, TMember> valueProducer)
            {
                Guard.AgainstNull(valueProducer, nameof(valueProducer));
                this.ParentConfiguration.AddRuleIfNeeded();
                this.ParentConfiguration.RuleBeingBuilt.UseApplicator(x => x.SetReturnValue(valueProducer(x)));
                return this.ParentConfiguration;
            }

            public IReturnValueConfiguration<TMember> Invokes(Action<IFakeObjectCall> action)
            {
                this.ParentConfiguration.Invokes(action);
                return this;
            }

            public IAfterCallSpecifiedConfiguration CallsBaseMethod()
            {
                return this.ParentConfiguration.CallsBaseMethod();
            }

            public IReturnValueConfiguration<TMember> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate)
            {
                this.ParentConfiguration.WhenArgumentsMatch(argumentsPredicate);
                return this;
            }

            public UnorderedCallAssertion MustHaveHappened(Repeated repeatConstraint)
            {
                return this.ParentConfiguration.MustHaveHappened(repeatConstraint);
            }

            public IAnyCallConfigurationWithReturnTypeSpecified<TMember> Where(Func<IFakeObjectCall, bool> predicate, Action<IOutputWriter> descriptionWriter)
            {
                this.ParentConfiguration.RuleBeingBuilt.ApplyWherePredicate(predicate, descriptionWriter);
                return this;
            }
        }

        private class RuleMatcher
            : ICallMatcher
        {
            private readonly RuleBuilder builder;

            public RuleMatcher(RuleBuilder builder)
            {
                this.builder = builder;
            }

            public bool Matches(IFakeObjectCall call)
            {
                Guard.AgainstNull(call, nameof(call));

                return this.builder.RuleBeingBuilt.IsApplicableTo(call) &&
                       ReferenceEquals(this.builder.manager.Object, call.FakedObject);
            }

            public override string ToString()
            {
                return this.builder.RuleBeingBuilt.ToString();
            }
        }
    }
}
