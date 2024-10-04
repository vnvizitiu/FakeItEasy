namespace FakeItEasy
{
    using System;
    using System.Text;
    using FakeItEasy.Configuration;
    using FakeItEasy.Core;

    internal static class ExceptionMessages
    {
        public static string ArgumentNameDoesNotExist =>
            "The specified argument name does not exist in the ArgumentList.";

        public static string NonConstructorExpression =>
            "Only expression of the type ExpressionType.New (constructor calls) are accepted.";

        public static string NowCalledDirectly =>
            "The Now-method on the event raise is not meant to be called directly, only use it to register to an event on a fake object that you want to be raised.";

        public static string NumberOfOutAndRefParametersDoesNotMatchCall =>
            "The number of values for out and ref parameters specified does not match the number of out and ref parameters in the call.";

        public static string WrongNumberOfArguments =>
            "The number of arguments does not match the number of parameters of the method.";

        public static string DummyCreationExceptionDefault =>
            "Unable to create dummy object.";

        public static string FakeCreationExceptionDefault =>
            "Unable to create fake object.";

        public static string ArgumentConstraintCannotBeNestedInArgument => "An argument constraint, such as That, Ignored, or _, cannot be nested in an argument.";

        public static string DelegateCannotCallBaseMethod => "Can not configure a delegate proxy to call a base method.";

        public static string NotAWrappingFake => "The configured fake is not a wrapping fake.";

        public static string CallBehaviorAlreadyDefined =>
            "The behavior for this call has already been defined";

        public static string OutAndRefBehaviorAlreadyDefined =>
            "How to assign out and ref parameters has already been defined for this call";

        public static string NumberOfTimesNotGreaterThanZero =>
            "The number of times to occur is not greater than zero.";

        public static string CallTargetIsNotFakeBeingConfigured =>
            "The target of this call is not the fake object being configured.";

        public static string CannotFindPreviousRule =>
            "The rule after which to add the new rule was not found in the list.";

        public static string NotAMethodCallOrPropertyGetter =>
            "The specified expression is not a method call or property getter.";

        public static string ArgumentConstraintCanOnlyBeUsedInCallSpecification =>
            "A<T>.Ignored, A<T>._, and A<T>.That can only be used in the context of a call specification with A.CallTo()";

        public static string GetAssemblyFileNamesToScanForExtensionsShouldntReturnNull =>
            "IBootstrapper.GetAssemblyFileNamesToScanForExtensions must not return null";

        public static string WrappingFakeCannotRaiseEvent =>
            "The fake cannot raise the event because it's a wrapping fake and the event subscription is managed by the wrapped object. The event can only be raised internally by the wrapped object.";

        public static string CannotRaiseEventWhenCallingBaseMethod =>
            "The fake cannot raise the event because event subscription calls the base implementation.";

        public static string HandleEventsOnStrictFakes =>
            $@"Strict fakes don't handle calls to event accessors unless configured to do so. If you want the fake to handle these calls, you can either:
- configure the calls explicitly;
- create the fake with the {nameof(StrictFakeOptions)}.{nameof(StrictFakeOptions.AllowEvents)} flag;
- or use {nameof(Manage)}.{nameof(Manage.Event)}(eventName).{nameof(IManageEventConfiguration.Of)}(fake) or {nameof(Manage)}.{nameof(Manage.AllEvents)}.{nameof(IManageEventConfiguration.Of)}(fake) to enable automatic event management.";

        public static string NoCapturedValues =>
            "No values were captured.";

        public static string WrongConstructorExpressionType(Type actualConstructorType, Type expectedConstructorType) =>
            $"Supplied constructor is for type {actualConstructorType}, but must be for {expectedConstructorType}.";

        public static string NotRecognizedAsAFake(object proxy, Type type) =>
            $"Object '{proxy}' of type {type} is not recognized as a fake object.";

        public static string CallToUnconfiguredMethodOfStrictFake(IFakeObjectCall call)
        {
            var callFormatter = ServiceLocator.Resolve<IFakeObjectCallFormatter>();
            return $"Call to unconfigured method of strict fake: {callFormatter.GetDescription(call)}.";
        }

        public static string ArgumentConstraintHasWrongType(Type constraintType, Type parameterType) =>
            $"Argument constraint is of type {constraintType}, but parameter is of type {parameterType}. No call can match this constraint.";

        public static string TooManyArgumentConstraints(IArgumentConstraint constraint) =>
            $"Too many argument constraints specified. First superfluous constraint is {constraint}.";

        public static string UserCallbackThrewAnException(string callbackDescription) =>
            $"{callbackDescription} threw an exception. See inner exception for details.";

        public static string CannotInterceptMember(string failReason, string memberType, string description) =>
            new StringBuilder()
                .AppendLine()
                .AppendLine()
                .Append("  ")
                .AppendLine($"The current proxy generator can not intercept the {memberType} {description} for the following reason:")
                .Append("    - ")
                .AppendLine(failReason)
                .AppendLine()
                .ToString();

        public static string ServiceNotRegistered<T>() where T : class =>
            $"The specified service {typeof(T)} was not registered.";

        public static string CallSignatureDoesNotMatchValueProducer(string nameOfFeature, string fakeSignature, string actionSignature) =>
            $"The faked method has the signature ({fakeSignature}), but {nameOfFeature} was used with ({actionSignature}).";

        public static string CallSignatureDoesNotMatchArguments(string fakeSignature, string actionSignature) =>
            $"The event has the signature ({fakeSignature}), but the provided arguments have types ({actionSignature}).";

        public static string PropertyHasNoSetter(string propertyName) =>
            $"The property {propertyName} does not have a setter.";

        public static string ExpressionIsIndexedPropertyWithoutSetter(string expressionDescription) =>
            $"Expression '{expressionDescription}' refers to an indexed property that does not have a setter.";

        public static string IsNotAGetter(string expressionDescription) =>
            $"Expression '{expressionDescription}' must refer to a property or indexer getter, but doesn't.";

        public static string UnableToCast(Type originalType, Type targetType) =>
            $"Unable to cast object of type '{originalType}' to type '{targetType}'.";

        public static string NotAnInterface(Type interfaceType) =>
            $"The specified type {interfaceType} is not an interface";

        public static string FailedToFakeValueType(Type fakeType) =>
            $"Failed to create Fake of type {fakeType} because it's a value type.";
    }
}
