namespace FakeItEasy.Expressions
{
    using System;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    /// <summary>
    /// An implementation of the <see cref="IFakeObjectCallRule" /> interface that uses
    /// expressions for evaluating if the rule is applicable to a specific call.
    /// </summary>
    internal class ExpressionCallRule
        : BuildableCallRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallRule"/> class.
        /// </summary>
        /// <param name="expressionMatcher">The expression matcher to use.</param>
        public ExpressionCallRule(ExpressionCallMatcher expressionMatcher)
        {
            Guard.AgainstNull(expressionMatcher);

            this.ExpressionMatcher = expressionMatcher;
            this.AddAction(this.ExpressionMatcher.PerformConstraintMatcherSideEffects);
            this.OutAndRefParametersValueProducer = expressionMatcher.GetOutAndRefParametersValueProducer();
        }

        /// <summary>
        /// Handles the instantiation of ExpressionCallRule instance.
        /// </summary>
        /// <param name="callSpecification">An expression specifying the call.</param>
        /// <returns>A rule instance.</returns>
        public delegate ExpressionCallRule Factory(ParsedCallExpression callSpecification);

        private ExpressionCallMatcher ExpressionMatcher { get; }

        public override void DescribeCallOn(IOutputWriter writer) => this.ExpressionMatcher.DescribeCallOn(writer);

        public override void UsePredicateToValidateArguments(Func<ArgumentCollection, bool> argumentsPredicate)
        {
            this.ExpressionMatcher.UsePredicateToValidateArguments(argumentsPredicate);
        }

        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall) =>
            this.ExpressionMatcher.Matches(fakeObjectCall);

        protected override BuildableCallRule CloneCallSpecificationCore() =>
            new ExpressionCallRule(this.ExpressionMatcher);
    }
}
