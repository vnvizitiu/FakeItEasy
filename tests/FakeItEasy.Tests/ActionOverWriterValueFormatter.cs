namespace FakeItEasy.Tests
{
    using System;
    using System.Text;

    public class ActionOverWriterValueFormatter : ArgumentValueFormatter<Action<IOutputWriter>>
    {
        protected override string GetStringValue(Action<IOutputWriter> argumentValue)
        {
            Guard.AgainstNull(argumentValue);

            var writer = ServiceLocator.Resolve<StringBuilderOutputWriter.Factory>().Invoke();
            argumentValue.Invoke(writer);
            return writer.Builder.ToString();
        }
    }
}
