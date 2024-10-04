namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;
    using FakeItEasy.Creation;

    /// <summary>
    /// Represents a fake object that provides an API for configuring a faked object, exposed by the
    /// FakedObject-property.
    /// </summary>
    /// <typeparam name="T">The type of the faked object.</typeparam>
    public class Fake<T> : IHideObjectMembers where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Fake{T}"/> class.
        /// Creates a new fake object.
        /// </summary>
        public Fake()
        {
            this.FakedObject = (T)FakeAndDummyManager.CreateFake(typeof(T), new LoopDetectingResolutionContext());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Fake{T}"/> class.
        /// Creates a new fake object using options built by <paramref name="optionsBuilder"/>.
        /// </summary>
        /// <param name="optionsBuilder">
        /// Action that builds options used to create the fake object.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public Fake(Action<IFakeOptions<T>> optionsBuilder)
        {
            Guard.AgainstNull(optionsBuilder);

            this.FakedObject = CreateFake(optionsBuilder);
        }

        /// <summary>
        /// Gets the faked object.
        /// </summary>
        public T FakedObject { get; }

        /// <summary>
        /// Gets all calls made to the faked object.
        /// </summary>
        public IEnumerable<ICompletedFakeObjectCall> RecordedCalls => FakeItEasy.Fake.GetCalls(this.FakedObject);

        private static FakeAndDummyManager FakeAndDummyManager =>
            ServiceLocator.Resolve<FakeAndDummyManager>();

        private IStartConfiguration<T> StartConfiguration
        {
            get
            {
                var factory = ServiceLocator.Resolve<IStartConfigurationFactory>();
                return factory.CreateConfiguration<T>(FakeItEasy.Fake.GetFakeManager(this.FakedObject));
            }
        }

        /// <summary>
        /// Configures calls to the specified member.
        /// </summary>
        /// <param name="callSpecification">An expression specifying the call to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public IVoidArgumentValidationConfiguration CallsTo(Expression<Action<T>> callSpecification)
        {
            return this.StartConfiguration.CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures calls to the specified member.
        /// </summary>
        /// <typeparam name="TMember">The type of value the member returns.</typeparam>
        /// <param name="callSpecification">An expression specifying the call to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TMember>(Expression<Func<T, TMember>> callSpecification)
        {
            return this.StartConfiguration.CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures calls to the setter of the specified property.
        /// </summary>
        /// <typeparam name="TValue">The type of the property.</typeparam>
        /// <param name="propertySpecification">An expression specifying the property to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public IPropertySetterAnyValueConfiguration<TValue> CallsToSet<TValue>(Expression<Func<T, TValue>> propertySpecification)
        {
            return this.StartConfiguration.CallsToSet(propertySpecification);
        }

        /// <summary>
        /// Configures any call to the fake object.
        /// </summary>
        /// <returns>A configuration object.</returns>
        public IAnyCallConfigurationWithNoReturnTypeSpecified AnyCall()
        {
            return this.StartConfiguration.AnyCall();
        }

        /// <summary>
        /// Configures subscription to or unsubscription from an event of the fake object.
        /// </summary>
        /// <param name="action">An <see cref="EventAction"/> that represents the action to configure.</param>
        /// <returns>A configuration object.</returns>
        public IAnyCallConfigurationWithVoidReturnType CallsTo(EventAction action)
        {
            return this.AnyCall().MatchingEventAction(this.FakedObject, action);
        }

        private static T CreateFake(Action<IFakeOptions<T>> optionsBuilder)
        {
            Guard.AgainstNull(optionsBuilder);

            return (T)FakeAndDummyManager.CreateFake(
                typeof(T),
                options => optionsBuilder((IFakeOptions<T>)options),
                new LoopDetectingResolutionContext());
        }
    }
}
