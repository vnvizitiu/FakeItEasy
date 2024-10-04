namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using FakeItEasy.Creation;

    /// <summary>
    /// Provides methods for generating fake objects.
    /// </summary>
    public static class A
    {
        private static FakeAndDummyManager FakeAndDummyManager =>
            ServiceLocator.Resolve<FakeAndDummyManager>();

        private static IFakeConfigurationManager ConfigurationManager =>
            ServiceLocator.Resolve<IFakeConfigurationManager>();

        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of fake.")]
        public static T Fake<T>() where T : class
        {
            return (T)FakeAndDummyManager.CreateFake(typeof(T), new LoopDetectingResolutionContext());
        }

        /// <summary>
        /// Creates a fake object of the type T.
        /// </summary>
        /// <typeparam name="T">The type of fake object to create.</typeparam>
        /// <param name="optionsBuilder">A function that specifies options for the fake object.</param>
        /// <returns>A fake object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of fake.")]
        public static T Fake<T>(Action<IFakeOptions<T>> optionsBuilder) where T : class
        {
            Guard.AgainstNull(optionsBuilder);

            return (T)FakeAndDummyManager.CreateFake(
                typeof(T),
                options => optionsBuilder((IFakeOptions<T>)options),
                new LoopDetectingResolutionContext());
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of fake.")]
        public static IList<T> CollectionOfFake<T>(int numberOfFakes) where T : class
        {
            return CollectionOfFake<T>(numberOfFakes, (o, i) => { });
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <param name="optionsBuilder">A function that specifies options for each fake object.</param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static IList<T> CollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder) where T : class
        {
            return CollectionOfFake<T>(numberOfFakes, (options, i) => optionsBuilder(options));
        }

        /// <summary>
        /// Creates a collection of fakes of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of fakes to create.</typeparam>
        /// <param name="numberOfFakes">The number of fakes in the collection.</param>
        /// <param name="optionsBuilder">
        /// A function that specifies options for each fake object;
        /// the second parameter of the function represents the 0-based index of the source element.
        /// </param>
        /// <returns>A collection of fake objects of the specified type.</returns>
        public static IList<T> CollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>, int> optionsBuilder)
            where T : class
        {
            return Enumerable.Range(0, numberOfFakes).Select(i => Fake<T>(options => optionsBuilder(options, i))).ToList();
        }

        /// <summary>
        /// Gets a dummy object of the specified type. The value of a dummy object
        /// should be irrelevant. Dummy objects should not be configured.
        /// </summary>
        /// <typeparam name="T">The type of dummy to return.</typeparam>
        /// <returns>
        /// A dummy object of the specified type.
        /// May be null, if a dummy factory is defined that returns null for dummies of type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of dummy.")]
        public static T Dummy<T>()
        {
            return (T)FakeAndDummyManager.CreateDummy(typeof(T), new LoopDetectingResolutionContext())!;
        }

        /// <summary>
        /// Creates a collection of dummies of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of dummies to create.</typeparam>
        /// <param name="numberOfDummies">The number of dummies in the collection.</param>
        /// <returns>
        /// A collection of dummy objects of the specified type.
        /// Individual dummies may be null, if a dummy factory is defined that returns null for dummies of type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Dummies of the specified type can not be created.</exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Used to specify the type of dummy.")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IList<T> CollectionOfDummy<T>(int numberOfDummies)
        {
            return Enumerable.Range(0, numberOfDummies).Select(_ => Dummy<T>()).ToList();
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <param name="callSpecification">An expression where the configured member is called.</param>
        /// <returns>A configuration object.</returns>
        public static IVoidArgumentValidationConfiguration CallTo(Expression<Action> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        /// <summary>
        /// Gets a configuration object allowing for further configuration of
        /// any call to the specified faked object.
        /// </summary>
        /// <param name="fake">
        /// The fake to configure.
        /// </param>
        /// <returns>
        /// A configuration object.
        /// </returns>
        public static IAnyCallConfigurationWithNoReturnTypeSpecified CallTo(object fake)
        {
            return ConfigurationManager.CallTo(fake);
        }

        /// <summary>
        /// Configures a call to a faked object.
        /// </summary>
        /// <typeparam name="T">The type of member on the faked object to configure.</typeparam>
        /// <param name="callSpecification">An expression where the configured member is called.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static IReturnValueArgumentValidationConfiguration<T> CallTo<T>(Expression<Func<T>> callSpecification)
        {
            return ConfigurationManager.CallTo(callSpecification);
        }

        /// <summary>
        /// Configures the setting of a property on a faked object.
        /// </summary>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="propertySpecification">An expression that calls the getter of the property to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
        public static IPropertySetterAnyValueConfiguration<TValue> CallToSet<TValue>(Expression<Func<TValue>> propertySpecification)
        {
            return ConfigurationManager.CallToSet(propertySpecification);
        }

        /// <summary>
        /// Configures subscription to or unsubscription from an event of a faked object.
        /// </summary>
        /// <param name="fake">The fake to configure.</param>
        /// <param name="action">An <see cref="EventAction"/> that represents the action to configure.</param>
        /// <returns>A configuration object.</returns>
        public static IAnyCallConfigurationWithVoidReturnType CallTo(object fake, EventAction action)
        {
            return CallTo(fake).MatchingEventAction(fake, action);
        }

        /// <summary>
        /// Obtains an object that can be used to capture and later examine <typeparamref name="TArgument"/> instances passed to
        /// a fake method call.
        /// </summary>
        /// <typeparam name="TArgument">The type of argument to capture.</typeparam>
        /// <returns>
        /// An object that can be used to capture and later examine arguments passed to a fake method call.
        /// </returns>
        /// <remarks>
        /// If captured arguments are modified by production or test code, a new
        /// argument capturer can be created from this instance to preserve the original state. See
        /// <see cref="Captured{TArgument}.FrozenBy{TCapture}(Func{TArgument, TCapture})"/>.
        /// </remarks>
        public static Captured<TArgument> Captured<TArgument>() => new();
    }
}
